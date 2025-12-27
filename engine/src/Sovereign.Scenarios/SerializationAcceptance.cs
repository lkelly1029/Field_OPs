using System;
using Xunit;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using Sovereign.Serialization;
using Sovereign.Core;
using System.Linq;

namespace Sovereign.Tests.Scenarios
{
    public class SerializationAcceptance
    {
        [Fact]
        public void SaveAndLoad_RestoresState()
        {
            // 1. Setup a Universe with state
            var original = new Universe();
            var plot = new Plot { X = 5, Y = 5, State = PlotState.Active };
            plot.Producer = new IronMine();
            plot.Consumer = (IConsumer)plot.Producer;
            original.AddPlot(plot);

            // Advance time to change tick and build up storage/money
            original.Tick(); 
            // IronMine produces 200 Iron. 
            // Consumes 2000 Power (failed, so no production actually? Wait, we verified this in IndustrialAcceptance).
            // If inputs fail, no production.
            // But we want to test STORAGE restoration.
            // Let's manually inject storage.
            plot.Storage[ResourceType.Iron] = 500;
            
            // 2. Serialize
            string json = UniverseSerializer.Serialize(original);
            Assert.False(string.IsNullOrEmpty(json));

            // 3. Deserialize
            var restored = UniverseSerializer.Deserialize(json);

            // 4. Verify
            Assert.Equal(original.Id, restored.Id);
            Assert.Equal(original.CurrentTick.Value, restored.CurrentTick.Value);
            Assert.Equal(original.TreasuryId, restored.TreasuryId);
            
            var restoredPlot = restored.Plots.FirstOrDefault(p => p.X == 5 && p.Y == 5);
            Assert.NotNull(restoredPlot);
            Assert.Equal(PlotState.Active, restoredPlot.State);
            Assert.Equal(500, restoredPlot.Storage[ResourceType.Iron]);
            Assert.IsType<IronMine>(restoredPlot.Producer);
        }
    }
}
