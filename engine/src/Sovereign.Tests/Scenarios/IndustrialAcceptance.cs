using System;
using Xunit;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using Sovereign.Core;
using Sovereign.Core.Primitives;
using Sovereign.Economy;

namespace Sovereign.Tests.Scenarios
{
    public class IndustrialAcceptance
    {
        [Fact]
        public void FullChain_ProductionSuccess()
        {
            var universe = new Universe();
            var ledger = universe.Ledger;
            var treasury = universe.TreasuryId;

            // 1. Setup Inputs
            // Power Plant
            var powerPlot = new Plot { State = PlotState.Active };
            powerPlot.Producer = new NuclearPlant(); // +50000 Power
            universe.AddPlot(powerPlot);

            // Water Pump
            var waterPlot = new Plot { State = PlotState.Active };
            waterPlot.Producer = new WaterPump(); // +1000 Water
            // Need 2 pumps for 2000 Water demand? SteelMill needs 2000.
            // Pump produces 1000. So we need 2 pumps.
            universe.AddPlot(waterPlot);
            var waterPlot2 = new Plot { State = PlotState.Active };
            waterPlot2.Producer = new WaterPump(); 
            universe.AddPlot(waterPlot2);

            // Iron Mine
            var minePlot = new Plot { State = PlotState.Active };
            var mine = new IronMine(); // +200 Iron, -2000 Power
            minePlot.Producer = mine;
            minePlot.Consumer = mine;
            universe.AddPlot(minePlot);

            // 2. Setup Steel Mill
            var millPlot = new Plot { State = PlotState.Active };
            millPlot.Consumer = new SteelMill(); // Demands: 5000 Power, 2000 Water, 150 Iron
            millPlot.Producer = (IProducer)millPlot.Consumer; // Same instance
            universe.AddPlot(millPlot);

            // 3. Tick
            universe.Tick();

            // 4. Assertions
            // Inputs Satisfied?
            Assert.True(millPlot.InputsSatisfied, "Mill should have inputs satisfied");
            
            // Production Happened?
            // SteelMill produces 100 Steel.
            long steelBalance = ledger.GetResourceBalance(universe.Id, ResourceType.Steel);
            Assert.Equal(100, steelBalance);
        }

        [Fact]
        public void FullChain_MissingIron_StopsProduction()
        {
            var universe = new Universe();
            // Drain treasury to prevent imports
            universe.Ledger.TryDebit(universe.TreasuryId, universe.Ledger.GetBalance(universe.TreasuryId));

            // Power & Water available
            var powerPlot = new Plot { State = PlotState.Active };
            powerPlot.Producer = new NuclearPlant();
            universe.AddPlot(powerPlot);

            var waterPlot = new Plot { State = PlotState.Active };
            waterPlot.Producer = new WaterPump();
            universe.AddPlot(waterPlot);
            var waterPlot2 = new Plot { State = PlotState.Active };
            waterPlot2.Producer = new WaterPump();
            universe.AddPlot(waterPlot2);

            // NO IRON MINE

            // Steel Mill
            var millPlot = new Plot { State = PlotState.Active };
            millPlot.Consumer = new SteelMill();
            millPlot.Producer = (IProducer)millPlot.Consumer;
            universe.AddPlot(millPlot);

            // Tick 1
            universe.Tick();

            // Assertions
            Assert.False(millPlot.InputsSatisfied, "Mill should fail due to missing Iron");
            
            // No Steel produced (Refinery Logic stops it NEXT tick, but since we start satisfied=true, 
            // the first tick produce() might run? 
            // Wait, logic in Universe.Tick:
            // 1. Collect Requests & Production (Producer produces if InputsSatisfied=true).
            // Initial InputsSatisfied is true.
            // So Tick 1: It produces 100 Steel.
            // Then resolution fails (missing Iron).
            // Then InputsSatisfied set to false.
            
            // So Tick 1 produces. Tick 2 stops.
            Assert.Equal(100, universe.Ledger.GetResourceBalance(universe.Id, ResourceType.Steel));

            // Tick 2
            universe.Tick();

            // Should NOT produce more. Balance stays 100.
            Assert.Equal(100, universe.Ledger.GetResourceBalance(universe.Id, ResourceType.Steel));
        }
    }
}
