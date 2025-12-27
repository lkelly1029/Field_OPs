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

            var plot = new Plot { State = PlotState.Built };
            plot.Consumer = new Farm(); // Consumes 500 Water
            universe.AddPlot(plot);

            // Tick
            universe.Tick();

            // Assert Treasury Debit
            // 500 Water * 5 cents = 2500 cents
            var currentTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            Assert.Equal(initialTreasury - 2500, currentTreasury);
        }

        [Fact]
        public void WaterSubstitution_LocalPump()
        {
            var universe = new Universe();
            var initialTreasury = universe.Ledger.GetBalance(universe.TreasuryId);

            var farmPlot = new Plot { State = PlotState.Built };
            farmPlot.Consumer = new Farm(); 
            universe.AddPlot(farmPlot);

            var pumpPlot = new Plot { State = PlotState.Active };
            pumpPlot.Producer = new WaterPump(); // Produces 1000 Water
            universe.AddPlot(pumpPlot);

            universe.Tick();

            // Local supply (1000) > Demand (500). No imports.
            var currentTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            Assert.Equal(initialTreasury, currentTreasury);
        }
    }
}
