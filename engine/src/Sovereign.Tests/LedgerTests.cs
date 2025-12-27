using System;
using Xunit;
using Sovereign.Economy;
using Sovereign.Core.Primitives;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using Sovereign.Core;

namespace Sovereign.Tests
{
    public class LedgerTests
    {
        [Fact]
        public void Ledger_Reconciles_PerTick()
        {
            // Arrange
            var exchange = new GlobalExchange();
            var universe = new Universe(exchange);
            
            // Setup a scenario where money moves.
            // We need a deficit so the universe buys from AI.
            // Add a consumer (House) but no producer.
            var plot = new Plot { State = PlotState.Active, Consumer = new House() };
            universe.AddPlot(plot);

            // Initial state
            var initialTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            
            // Act
            universe.Tick();

            // Assert
            var finalTreasury = universe.Ledger.GetBalance(universe.TreasuryId);
            var netChange = universe.NetTreasuryChangeLastTick;

            // 1. The ledger balance should match the tracked net change.
            Assert.Equal(initialTreasury.Value + netChange, finalTreasury.Value);

            // 2. Verify specific logic for this scenario (Importing from AI)
            // House needs 1000 Power. 
            // AI Price is 2 cents/Wh.
            // Cost = 1000 * 2 = 2000 cents.
            long expectedCost = 1000 * Universe.AI_POWER_PRICE_CENTS_PER_WH;
            
            Assert.Equal(-expectedCost, netChange);
            Assert.Equal(initialTreasury.Value - expectedCost, finalTreasury.Value);
            
            // 3. Verify the resource side of the transaction (Demand was met)
            Assert.True(plot.InputsSatisfied, "Plot inputs should be satisfied by the purchase.");
        }
    }
}