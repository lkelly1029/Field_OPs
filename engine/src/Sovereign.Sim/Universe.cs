using System;
using System.Collections.Generic;
using System.Linq;
using Sovereign.Core;
using Sovereign.Core.Primitives;
using Sovereign.Economy;
using Sovereign.Mods;
using Sovereign.Sim.Buildings;

namespace Sovereign.Sim
{
    public class Universe
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public TickIndex CurrentTick { get; private set; }
        public Ledger Ledger { get; }
        public Dictionary<ResourceType, long> TotalDemandLastTick { get; } = new();
        public Dictionary<ResourceType, long> TotalSupplyLastTick { get; } = new();
        public Guid TreasuryId { get; private set; } = Guid.NewGuid();
        public long NetTreasuryChangeLastTick { get; private set; }
        public MarketSnapshot LastMarketSnapshot { get; private set; }
        public GovernmentMod ActiveGovernment { get; private set; }

        private readonly List<Plot> _plots = new();
        private readonly GlobalExchange _exchange;

        // MVP Constant: AI sells power at 2 cents per Wh ($20/kWh) - Very expensive!
        public const long AI_POWER_PRICE_CENTS_PER_WH = 2;
        public static readonly Guid AI_MARKET_ID = Guid.Empty;
        public IReadOnlyList<Plot> Plots => _plots;

        public Universe() : this(new GlobalExchange())
        {
        }

        public Universe(GlobalExchange exchange)
        {
            _exchange = exchange;
            ActiveGovernment = GovernmentMod.Default;
            CurrentTick = new TickIndex(0);
            Ledger = new Ledger();
            
            // Seed treasury so we can afford initial imports
            Ledger.Credit(TreasuryId, new MoneyCents(1_000_000_00)); // $1,000,000.00
        }

        public Universe(GlobalExchange exchange, Ledger ledger)
        {
            _exchange = exchange;
            ActiveGovernment = GovernmentMod.Default;
            CurrentTick = new TickIndex(0);
            Ledger = ledger;
        }

        public void AddPlot(Plot plot)
        {
            _plots.Add(plot);
        }

        public void Tick()
        {
            _exchange.Tick(); // Update Market Prices

            long startTreasury = Ledger.GetBalance(TreasuryId).Value;

            TotalDemandLastTick.Clear();
            TotalSupplyLastTick.Clear();

            // 1. Collect Production & Demands, Pay for Production & Apply Taxes
            foreach (var plot in _plots)
            {
                if (plot.Consumer != null) // Collect Demands
                {
                    var demands = plot.Consumer.GetResourceDemands(CurrentTick);
                    foreach (var demand in demands)
                    {
                        plot.Demands[demand.Key] = demand.Value;
                        if (!TotalDemandLastTick.ContainsKey(demand.Key))
                            TotalDemandLastTick[demand.Key] = 0;
                        TotalDemandLastTick[demand.Key] += demand.Value;
                    }
                }
                if (plot.Producer != null && plot.InputsSatisfied) // Produce if inputs were met
                {
                    var supplies = plot.Producer.GetProduction(CurrentTick);
                    long grossIncome = 0;
                    foreach (var supply in supplies)
                    {
                        if (!TotalSupplyLastTick.ContainsKey(supply.Key))
                            TotalSupplyLastTick[supply.Key] = 0;
                        TotalSupplyLastTick[supply.Key] += supply.Value;
                        Ledger.CreditResource(Id, supply.Key, supply.Value);
                        
                        // NEW: Calculate income from production
                        grossIncome += supply.Value * _exchange.GetAiPrice(supply.Key);
                    }

                    // NEW: Pay producer and collect taxes
                    if (grossIncome > 0)
                    {
                        plot.LastTickIncome = grossIncome; // Track stats

                        // The Universe Treasury buys all production for now. This is a simplification.
                        if (Ledger.TryDebit(TreasuryId, new MoneyCents(grossIncome)))
                        {
                            Ledger.Credit(plot.OwnerId, new MoneyCents(grossIncome));

                            // Apply corporate tax to producers
                            if (ActiveGovernment.CorporateTaxRate > 0)
                            {
                                long taxAmount = (long)(grossIncome * ActiveGovernment.CorporateTaxRate);
                                if (Ledger.TryDebit(plot.OwnerId, new MoneyCents(taxAmount)))
                                {
                                    Ledger.Credit(TreasuryId, new MoneyCents(taxAmount));
                                    plot.LastTickTaxPaid = taxAmount; // Track stats
                                }
                            }
                            else
                            {
                                plot.LastTickTaxPaid = 0;
                            }
                        }
                    }
                    else
                    {
                        plot.LastTickIncome = 0;
                        plot.LastTickTaxPaid = 0;
                    }
                }
                else
                {
                    // Reset stats if not producing
                    if (plot.Producer != null)
                    {
                        plot.LastTickIncome = 0;
                        plot.LastTickTaxPaid = 0;
                    }
                }
            }

            // NEW PHASE: Fiscal Policy (UBI)
            if (ActiveGovernment.UniversalBasicIncomeCents > 0)
            {
                foreach (var plot in _plots.Where(p => p.Consumer is House))
                {
                    var ubi = new MoneyCents(ActiveGovernment.UniversalBasicIncomeCents);
                    if (Ledger.TryDebit(TreasuryId, ubi))
                    {
                        Ledger.Credit(plot.OwnerId, ubi);
                    }
                }
            }

            // 2. Resolve Deficits via Import
            var allResourceTypes = TotalDemandLastTick.Keys.ToList();
            var demandMet = new Dictionary<ResourceType, bool>();

            foreach (var resource in allResourceTypes)
            {
                long totalDemand = TotalDemandLastTick.ContainsKey(resource) ? TotalDemandLastTick[resource] : 0;
                long localSupply = Ledger.GetResourceBalance(Id, resource);
                long net = localSupply - totalDemand;

                if (net < 0)
                {
                    // Deficit: try to import
                    long deficitAmount = -net;
                    var deficit = new ResourceQuantity(resource, deficitAmount);
                    MoneyCents maxPrice = new MoneyCents(_exchange.GetAiPrice(resource)); // MVP: Willing to pay up to AI price

                    if (_exchange.TryBuy(deficit, maxPrice, out var offer))
                    {
                        MoneyCents cost = new MoneyCents(deficitAmount * offer.PricePerUnit.Value);
                        if (Ledger.TryDebit(TreasuryId, cost))
                        {
                            // Bought from Exchange or AI, credit to universe account
                            Ledger.CreditResource(Id, resource, deficitAmount);
                            demandMet[resource] = true;
                        }
                        else
                        {
                            demandMet[resource] = false; // Cannot afford import
                        }
                    }
                    else
                    {
                        demandMet[resource] = false; // No offer available
                    }
                }
                else
                {
                    demandMet[resource] = true; // Local supply is sufficient
                }
            }

            // 3. Distribute & Update Plots
            foreach (var plot in _plots)
            {
                foreach (var demand in plot.Demands)
                {
                    if (demandMet.TryGetValue(demand.Key, out bool met) && met)
                    {
                        if (Ledger.TryDebitResource(Id, demand.Key, demand.Value))
                        {
                            // NEW: Consumer Pays Treasury
                            long price = _exchange.GetAiPrice(demand.Key);
                            long costValue = demand.Value * price;
                            var cost = new MoneyCents(costValue);

                            // Force Debit Consumer (Allow Debt) -> Credit Treasury
                            Ledger.ForceDebit(plot.OwnerId, cost);
                            Ledger.Credit(TreasuryId, cost);

                            plot.Deliveries[demand.Key] = demand.Value;
                            if (!plot.Storage.ContainsKey(demand.Key)) plot.Storage[demand.Key] = 0;
                            plot.Storage[demand.Key] += demand.Value;
                        }
                    }
                }
                plot.OnTick();
            }

            // 4. Publish Surplus to Global Exchange
            foreach (var type in System.Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>())
            {
                long balance = Ledger.GetResourceBalance(Id, type);
                if (balance > 0)
                {
                    // List surplus. Price? Undercut AI slightly? Or match?
                    // MVP: List at 1 cent below AI price to guarantee sales.
                    long marketPrice = _exchange.GetAiPrice(type);
                    long offerPrice = Math.Max(1, marketPrice - 1); 
                    
                    var offer = new ResourceOffer
                    {
                        SellerUniverseId = Id,
                        Quantity = new ResourceQuantity(type, balance),
                        PricePerUnit = new MoneyCents(offerPrice)
                    };
                    _exchange.ListOffer(offer);
                }
            }

            // 5. Advance Tick
            long endTreasury = Ledger.GetBalance(TreasuryId).Value;
            NetTreasuryChangeLastTick = endTreasury - startTreasury;
            LastMarketSnapshot = _exchange.GetSnapshot(CurrentTick.Value);
            CurrentTick++;
        }

        public MarketSnapshot GetMarketSnapshot()
        {
            return LastMarketSnapshot ?? _exchange.GetSnapshot(CurrentTick.Value);
        }

        public IReadOnlyList<Plot> GetPlots() => _plots;

        public void LoadState(Guid id, long currentTick, Guid treasuryId)
        {
            Id = id;
            CurrentTick = new TickIndex(currentTick);
            TreasuryId = treasuryId;
            ActiveGovernment = GovernmentMod.Default;
        }
    }
}
