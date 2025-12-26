using System;
using System.Collections.Generic;
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

        private readonly List<Plot> _plots = new();
        private readonly GlobalExchange _exchange;

        // MVP Constant: AI sells power at 2 cents per Wh ($20/kWh) - Very expensive!
        public const long AI_POWER_PRICE_CENTS_PER_WH = 2;

        public Universe() : this(new GlobalExchange())
        {
        }

        public Universe(GlobalExchange exchange)
        {
            _exchange = exchange;
            CurrentTick = new TickIndex(0);
            Ledger = new Ledger();
            
            // Seed treasury so we can afford initial imports
            Ledger.Credit(TreasuryId, new MoneyCents(1_000_000_00)); // $1,000,000.00
        }

        public void AddPlot(Plot plot)
        {
            _plots.Add(plot);
        }

        public void Tick()
        {
            // 1. Collect Requests & Production
            EnergyWh totalDemand = new EnergyWh(0);
            EnergyWh totalSupply = new EnergyWh(0);

            foreach (var plot in _plots)
            {
                if (plot.Consumer != null)
                {
                    totalDemand += plot.Consumer.GetDemand(CurrentTick);
                }
                if (plot.Producer != null)
                {
                    totalSupply += plot.Producer.Produce(CurrentTick);
                }
            }

            // 2. Resolve Supply & Demand (MVP Phase 3: Local -> Exchange -> AI)
            long netValue = totalSupply.Value - totalDemand.Value;

            if (netValue > 0)
            {
                // Surplus: Export to Exchange
                // MVP: List at 1 cent (undercutting AI at 2 cents)
                _exchange.ListPower(Id, new EnergyWh(netValue), new MoneyCents(1));
            }
            else if (netValue < 0)
            {
                // Deficit: Import
                EnergyWh deficit = new EnergyWh(-netValue);
                MoneyCents aiPrice = new MoneyCents(AI_POWER_PRICE_CENTS_PER_WH);

                // Try Exchange first (cheaper), then AI (expensive fallback)
                if (!_exchange.TryBuyPower(deficit, aiPrice, out var offer))
                {
                    // Fallback to AI
                    MoneyCents cost = new MoneyCents(deficit.Value * aiPrice.Value);
                    Ledger.TryDebit(TreasuryId, cost);
                }
                else
                {
                    // Bought from Exchange
                    MoneyCents cost = new MoneyCents(deficit.Value * offer.PricePerUnit.Value);
                    Ledger.TryDebit(TreasuryId, cost);
                    // TODO: Credit seller (cross-universe settlement)
                }
            }

            // 3. Advance Tick
            CurrentTick++;
        }
    }
}