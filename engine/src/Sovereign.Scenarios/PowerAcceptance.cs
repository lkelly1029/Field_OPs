using System;
using Xunit;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using Sovereign.Core;
using Sovereign.Core.Primitives;
using Sovereign.Economy;

namespace Sovereign.Tests.Scenarios
{
    public class PowerAcceptance
    {
        [Fact]
        public void DependencyEra_AIOnly()
        {
            var universe = new Universe();
            var initialTreasury = universe.Ledger.GetBalance(universe.TreasuryId);

            var plot = new Plot { State = PlotState.Built };
            plot.Consumer = new House(); 
            universe.AddPlot(plot);

            int ticks = 10;
            for (int i = 0; i < ticks; i++)
            {
                universe.Tick();
            }

            var currentTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            Assert.True(currentTreasury < initialTreasury, "Treasury should decrease due to imports");

            // Power: 1000 Wh * 2 cents = 2000
            // Water: 100 Liters * 5 cents = 500
            // Food: 50 kg * 10 cents = 500
            // Total: 3000 per tick * 10 ticks = 30000 cents
            var expectedCost = new MoneyCents(3000 * ticks);
            Assert.Equal(initialTreasury - expectedCost, currentTreasury);
        }

        [Fact]
        public void ImportSubstitution_LocalPlant()
        {
            var universe = new Universe();
            var initialTreasury = universe.Ledger.GetBalance(universe.TreasuryId);

            var housePlot = new Plot { State = PlotState.Built };
            housePlot.Consumer = new House(); 
            universe.AddPlot(housePlot);

            var plantPlot = new Plot { State = PlotState.Active };
            plantPlot.Producer = new NuclearPlant(); 
            universe.AddPlot(plantPlot);

            universe.Tick();
            universe.Tick();

            var currentTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            
            // Power is local (free), but Water and Food are still imported.
            // Water: 100 Liters * 5 cents = 500 per tick.
            // Food: 50 kg * 10 cents = 500 per tick.
            // 2 ticks = 2000 cents.
            Assert.Equal(initialTreasury - 2000, currentTreasury);
        }

        [Fact]
        public void ExportTycoon_Displacement()
        {
            var exchange = new GlobalExchange();
            var sharedLedger = new Ledger();

            // Universe A shares the ledger
            var universeA = new Universe(exchange, sharedLedger);
            
            var plantPlot = new Plot { State = PlotState.Active };
            plantPlot.Producer = new NuclearPlant(); 
            universeA.AddPlot(plantPlot);

            // Universe B shares the ledger
            var universeB = new Universe(exchange, sharedLedger);
            
            var housePlot = new Plot { State = PlotState.Built };
            housePlot.Consumer = new House(); 
            universeB.AddPlot(housePlot);

            // Capture initial treasuries
            var initialTreasuryA = universeA.Ledger.GetBalance(universeA.TreasuryId);
            var initialTreasuryB = universeB.Ledger.GetBalance(universeB.TreasuryId);

            // Universe A ticks and lists offer
            universeA.Tick();

            // Universe B ticks
            universeB.Tick();
            
            var currentTreasuryA = universeA.Ledger.GetBalance(universeA.TreasuryId);
            var currentTreasuryB = universeB.Ledger.GetBalance(universeB.TreasuryId);

            // Power from A: 1000 Wh * 1 cent = 1000
            // Water from AI: 100 Liters * 5 cents = 500
            // Food from AI: 50 kg * 10 cents = 500
            // Total cost for B: 2000 cents
            Assert.Equal(initialTreasuryB - 2000, currentTreasuryB);
            
            // Settlement Check:
            // Universe A sold 1000 Wh at 1 cent = 1000 revenue.
            // Universe A should have gained 1000 cents.
            Assert.Equal(initialTreasuryA + 1000, currentTreasuryA);
        }

        [Fact]
        public void FailureEra_BankruptcyDecay()
        {
            var universe = new Universe();
            universe.Ledger.TryDebit(universe.TreasuryId, universe.Ledger.GetBalance(universe.TreasuryId));
            Assert.Equal(0, universe.Ledger.GetBalance(universe.TreasuryId).Value);

            var plot = new Plot { State = PlotState.Active }; 
            plot.Consumer = new House();
            universe.AddPlot(plot);

            universe.Tick();

            Assert.Equal(0, plot.Deliveries[ResourceType.Power]);
            Assert.Equal(95.0, plot.Stability);

            for (int i = 0; i < 20; i++)
            {
                universe.Tick();
            }
            Assert.Equal(PlotState.Slum, plot.State);

            for (int i = 0; i < 50; i++)
            {
                universe.Tick();
            }
            Assert.Equal(PlotState.Abandoned, plot.State);
            Assert.Null(plot.Consumer); 
        }
    }
}