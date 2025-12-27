using System;
using System.Collections.Generic;
using System.Linq;
using Sovereign.Core;
using Sovereign.Core.Primitives;
using Sovereign.Economy;

namespace Sovereign.Sim
{
    public class Universe
    {
        public Guid Id { get; } = Guid.NewGuid();
        public TickIndex CurrentTick { get; private set; }
        public Ledger Ledger { get; }
        public Guid TreasuryId { get; } = Guid.NewGuid();

        public IReadOnlyList<Plot> Plots => _plots;

        private readonly List<Plot> _plots = new();
        private readonly GlobalExchange _exchange;

        public static readonly Guid AI_MARKET_ID = Guid.Empty;

        // Metrics for UI
        public Dictionary<ResourceType, long> TotalDemandLastTick { get; private set; } = new();
        public Dictionary<ResourceType, long> TotalSupplyLastTick { get; private set; } = new();
        public long NetTreasuryChangeLastTick { get; private set; }

        public Universe() : this(new GlobalExchange(), new Ledger())
        {
        }

        public Universe(GlobalExchange exchange, Ledger ledger = null)
        {
            _exchange = exchange;
            CurrentTick = new TickIndex(0);
            Ledger = ledger ?? new Ledger();
            
            // Seed treasury
            Ledger.Credit(TreasuryId, new MoneyCents(1_000_000_00)); // $1,000,000.00
        }

        public void AddPlot(Plot plot)
        {
            _plots.Add(plot);
        }

        public void Tick()
        {
            long treasuryStart = Ledger.GetBalance(TreasuryId).Value;

            var totalDemand = new Dictionary<ResourceType, long>();
            var localSupply = new Dictionary<ResourceType, long>();
            var imports = new Dictionary<ResourceType, long>();

            // Initialize dictionaries
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                totalDemand[type] = 0;
                localSupply[type] = 0;
                imports[type] = 0;
            }

            // 1. Collect Requests & Production
            foreach (var plot in _plots)
            {
                if (plot.Consumer != null)
                {
                    var demands = plot.Consumer.GetDemands(CurrentTick);
                    foreach (var d in demands)
                    {
                        plot.Demands[d.Type] = d.Value;
                        
                        // Calculate net request (Gross Demand - Storage)
                        long inStorage = plot.Storage.ContainsKey(d.Type) ? plot.Storage[d.Type] : 0;
                        long netRequest = Math.Max(0, d.Value - inStorage);
                        
                        totalDemand[d.Type] += netRequest;
                    }
                }
                if (plot.Producer != null && plot.InputsSatisfied)
                {
                    var products = plot.Producer.Produce(CurrentTick);
                    foreach (var p in products)
                    {
                        localSupply[p.Type] += p.Value;
                        // Ledger: Credit the producer's universe (self)
                        Ledger.CreditResource(Id, p);
                    }
                }
            }

            // 2. Resolve Supply & Demand per Resource
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                long netValue = localSupply[type] - totalDemand[type];

                if (netValue > 0)
                {
                    // Surplus: Export
                    _exchange.ListOffer(new ResourceOffer
                    {
                        SellerUniverseId = Id,
                        Quantity = new ResourceQuantity(type, netValue),
                        PricePerUnit = new MoneyCents(1)
                    });
                }
                else if (netValue < 0)
                {
                    // Deficit: Import
                    long deficitValue = -netValue;
                    long maxPrice = (type == ResourceType.Power) ? 2 : 5;

                    if (_exchange.TryBuy(new ResourceQuantity(type, deficitValue), new MoneyCents(maxPrice), out var offer))
                    {
                        MoneyCents baseCost = new MoneyCents(deficitValue * offer.PricePerUnit.Value);
                        
                        // Calculate Logistics
                        GlobalExchange.CalculateTransport(deficitValue, GlobalExchange.MVP_DISTANCE_KM, out var transportFee, out var loss);
                        
                        MoneyCents totalCost = new MoneyCents(baseCost.Value + transportFee.Value);

                        if (Ledger.TryDebit(TreasuryId, totalCost))
                        {
                            imports[type] = deficitValue - loss; // Receive less due to loss
                            
                            // Credit seller (Cross-Universe Settlement)
                            if (offer.SellerUniverseId != Guid.Empty)
                            {
                                Ledger.Credit(offer.SellerUniverseId, baseCost);
                            }
                            
                            // Transport fee is burned (paid to "logistics provider" / sink)
                        }
                    }
                }
            }

            // 3. Distribute & Update Plots
            foreach (var plot in _plots)
            {
                foreach (ResourceType type in plot.Demands.Keys)
                {
                    long available = localSupply[type] + imports[type];
                    long demand = totalDemand[type];
                    
                    if (available >= demand)
                    {
                        plot.Deliveries[type] = plot.Demands[type];
                    }
                    else if (demand > 0)
                    {
                        double ratio = (double)available / demand;
                        plot.Deliveries[type] = (long)(plot.Demands[type] * ratio);
                    }
                }

                plot.OnTick();
            }

            // 4. Update Metrics
            TotalDemandLastTick = new Dictionary<ResourceType, long>(totalDemand);
            // Supply = Local + Imports
            var totalSupply = new Dictionary<ResourceType, long>();
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                totalSupply[type] = localSupply[type] + imports[type];
            }
            TotalSupplyLastTick = totalSupply;

            long treasuryEnd = Ledger.GetBalance(TreasuryId).Value;
            NetTreasuryChangeLastTick = treasuryEnd - treasuryStart;

            CurrentTick++;
        }
    }
}