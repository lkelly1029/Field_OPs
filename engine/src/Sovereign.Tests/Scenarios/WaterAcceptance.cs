using System;
using Xunit;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using Sovereign.Core;
using Sovereign.Core.Primitives;
using Sovereign.Economy;

namespace Sovereign.Tests.Scenarios
{
    public class WaterAcceptance
    {
        [Fact]
        public void WaterDependency_AIOnly()
        {
            var universe = new Universe();
            var initialTreasury = universe.Ledger.GetBalance(universe.TreasuryId);

            var plot = new Plot { State = PlotState.Active };
            plot.Consumer = new Farm(); // Consumes 500 Water
            universe.AddPlot(plot);

            // Tick
            universe.Tick();

            // Assert Treasury Balance
            // Import Cost: 500 * 5 = 2500.
            // Sales Revenue: 500 * 5 = 2500.
            // Net: 0.
            var currentTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            Assert.Equal(initialTreasury, currentTreasury);
        }

        [Fact]
        public void WaterSubstitution_LocalPump()
        {
            var universe = new Universe();
            var initialTreasury = universe.Ledger.GetBalance(universe.TreasuryId);

            var farmPlot = new Plot { State = PlotState.Active };
            farmPlot.Consumer = new Farm(); 
            universe.AddPlot(farmPlot);

            var pumpPlot = new Plot { State = PlotState.Active };
            pumpPlot.Producer = new WaterPump(); // Produces 1000 Water
            universe.AddPlot(pumpPlot);

            universe.Tick();

            // Local supply (1000) > Demand (500). No imports.
            // Treasury pays Producer $50 (5000c).
            // Treasury sells to Consumer $25 (2500c).
            // Net: -$25.
            var currentTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            Assert.Equal(initialTreasury - new MoneyCents(2500), currentTreasury);
        }

        [Fact]
        public void WaterExport_Displacement()
        {
            var exchange = new GlobalExchange();
            var universeA = new Universe(exchange);

            // Universe A produces surplus water
            var pumpPlot = new Plot { State = PlotState.Active };
            pumpPlot.Producer = new WaterPump(); // 1000 Water
            universeA.AddPlot(pumpPlot);

            // Universe A ticks to list surplus
            universeA.Tick();

            // Universe B needs water. 
            // AI Price is 5.
            // Universe A lists at 1 (default logic in Universe.Tick for surplus).
            var bought = exchange.TryBuy(
                new ResourceQuantity(ResourceType.Water, 500), 
                new MoneyCents(5), // Max price (AI price)
                out var offer
            );

            Assert.True(bought, "Should buy from exchange");
            Assert.Equal(universeA.Id, offer.SellerUniverseId);
            Assert.Equal(4, offer.PricePerUnit.Value);
        }
    }
}
