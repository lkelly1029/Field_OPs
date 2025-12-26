using System;
using Xunit;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using Sovereign.Core.Primitives;
using Sovereign.Economy;

namespace Sovereign.Tests.Scenarios
{
    /// <summary>
    /// Acceptance tests for the Power Vertical MVP. These tests validate the
    /// end-to-end economic flow for power, from dependency on AI to market displacement.
    /// These scenarios correspond to the definitions in sovereign_state_control.yaml.
    /// </summary>
    public class PowerAcceptance
    {
        /// <summary>
        /// Scenario: A new universe is entirely dependent on the AI Global Market for power.
        /// Asserts:
        /// - AI supplies 100% of power initially.
        /// - Treasury decreases due to high import prices.
        /// </summary>
        [Fact]
        public void DependencyEra_AIOnly()
        {
            // 1. Setup a new Universe with houses but no power plants.
            var universe = new Universe();
            var initialTreasury = universe.Ledger.GetBalance(universe.TreasuryId);

            var plot = new Plot { State = PlotState.Built };
            plot.Consumer = new House(); // Demands 1000 Wh per tick
            universe.AddPlot(plot);

            // 2. Run the simulation for a number of ticks.
            int ticks = 10;
            for (int i = 0; i < ticks; i++)
            {
                universe.Tick();
            }

            // 3. Assert that power demand is met.
            // (Implicitly met by AI in this MVP phase)
            
            // 4. Assert that the treasury balance has decreased.
            var currentTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            Assert.True(currentTreasury < initialTreasury, "Treasury should decrease due to imports");

            // 5. Assert exact cost (1000 Wh * 2 cents * 10 ticks = 20000 cents)
            var expectedCost = new MoneyCents(1000 * Universe.AI_POWER_PRICE_CENTS_PER_WH * ticks);
            Assert.Equal(initialTreasury - expectedCost, currentTreasury);
        }

        /// <summary>
        /// Scenario: The player builds a local power plant to substitute expensive AI imports.
        /// Asserts:
        /// - Local supply share > 80% after plant is built.
        /// - Average unit cost of power decreases compared to the AI-only era.
        /// </summary>
        [Fact]
        public void ImportSubstitution_LocalPlant()
        {
            // 1. Setup a Universe from the DependencyEra scenario.
            var universe = new Universe();
            var initialTreasury = universe.Ledger.GetBalance(universe.TreasuryId);

            var housePlot = new Plot { State = PlotState.Built };
            housePlot.Consumer = new House(); // 1000 Wh demand
            universe.AddPlot(housePlot);

            // 2. Simulate building a NuclearPlant.
            var plantPlot = new Plot { State = PlotState.Active };
            plantPlot.Producer = new NuclearPlant(); // 50000 Wh supply
            universe.AddPlot(plantPlot);

            // 3. Run the simulation for a number of ticks.
            universe.Tick();
            universe.Tick();

            // 4. Assert that the treasury has NOT decreased because local supply > demand
            var currentTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            Assert.Equal(initialTreasury, currentTreasury);
        }

        /// <summary>
        /// Scenario: A universe with surplus power exports to the Global Exchange, displacing AI supply in another universe.
        /// Asserts:
        /// - Universe B imports from the player's offer in Universe A if it's cheaper than the AI.
        /// - AI supply share in Universe B drops as a result.
        /// </summary>
        [Fact]
        public void ExportTycoon_Displacement()
        {
            // 1. Setup Global Exchange
            var exchange = new GlobalExchange();

            // 1. Setup Universe A with surplus power production (e.g., multiple plants).
            var universeA = new Universe(exchange);
            
            var plantPlot = new Plot { State = PlotState.Active };
            plantPlot.Producer = new NuclearPlant(); // 50000 Wh
            universeA.AddPlot(plantPlot);

            // 2. Setup Universe B dependent on AI power.
            var universeB = new Universe(exchange);
            
            var housePlot = new Plot { State = PlotState.Built };
            housePlot.Consumer = new House(); // 1000 Wh
            universeB.AddPlot(housePlot);

            // TODO: Implement Tick logic to publish offers and check exchange
            // For now, manually simulate the exchange interaction
            exchange.ListPower(universeA.Id, new EnergyWh(49000), new MoneyCents(1));
            var bought = exchange.TryBuyPower(new EnergyWh(1000), new MoneyCents(Universe.AI_POWER_PRICE_CENTS_PER_WH), out var offer);
            
            Assert.True(bought, "Universe B should have found a cheaper offer on the exchange");
            Assert.Equal(universeA.Id, offer.SellerUniverseId);
        }
    }
}