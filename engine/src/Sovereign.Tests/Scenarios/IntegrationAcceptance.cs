using System;
using Xunit;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using Sovereign.Core;
using Sovereign.Core.Primitives;
using Sovereign.Economy;

namespace Sovereign.Tests.Scenarios
{
    public class IntegrationAcceptance
    {
        [Fact]
        public void RefineryLogic_StopOnShortage()
        {
            var universe = new Universe();
            // Drain treasury so it can't import inputs
            universe.Ledger.TryDebit(universe.TreasuryId, universe.Ledger.GetBalance(universe.TreasuryId));

            var millPlot = new Plot { State = PlotState.Active };
            var mill = new SteelMill(); // Needs Power+Water
            millPlot.Consumer = mill;
            millPlot.Producer = mill;
            universe.AddPlot(millPlot);

            // Tick 1: Mill requests inputs. Shortage happens.
            universe.Tick();
            Assert.False(millPlot.InputsSatisfied);
            
            // Check production (should have happened in Tick 1 because initial state is Satisfied=true)
            // But we want to see it STOP in Tick 2.
            Assert.Equal(100, universe.Ledger.GetResourceBalance(universe.Id, ResourceType.Steel));

            // Tick 2: Mill should NOT produce because InputsSatisfied was false at end of Tick 1.
            universe.Tick();
            
            // Balance remains 100 (no new production)
            Assert.Equal(100, universe.Ledger.GetResourceBalance(universe.Id, ResourceType.Steel));
        }

        [Fact]
        public void RefineryLogic_ResumeOnSatisfaction()
        {
            var universe = new Universe();
            var millPlot = new Plot { State = PlotState.Active };
            var mill = new SteelMill();
            millPlot.Consumer = mill;
            millPlot.Producer = mill;
            universe.AddPlot(millPlot);

            // Tick 1: Has money, satisfies inputs, produces.
            universe.Tick();
            Assert.True(millPlot.InputsSatisfied);
            Assert.Equal(100, universe.Ledger.GetResourceBalance(universe.Id, ResourceType.Steel));

            // Tick 2: Produces again.
            universe.Tick();
            Assert.Equal(200, universe.Ledger.GetResourceBalance(universe.Id, ResourceType.Steel));
        }
    }
}
