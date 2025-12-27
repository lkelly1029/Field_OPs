using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using Xunit;

namespace Sovereign.Tests
{
    public class PlotDecayTests
    {
        [Fact]
        public void Plot_Decays_And_Becomes_Slum_Then_Abandoned_When_Unpowered()
        {
            // Arrange
            var universe = new Universe();
            var plot = new Plot { State = PlotState.Active, Consumer = new House() };
            universe.AddPlot(plot);

            double initialStability = plot.Stability;

            // Act & Assert: Decay to Slum
            // Stability starts at 100 and decays by 5 each tick of shortage.
            // It should become a slum after 20 ticks (100 / 5).
            for (int i = 0; i < 20; i++)
            {
                Assert.Equal(PlotState.Active, plot.State);
                universe.Tick();
            }

            Assert.True(plot.Stability < initialStability);
            Assert.True(plot.Stability <= 0);
            Assert.Equal(PlotState.Slum, plot.State);

            // Act & Assert: Decay to Abandoned
            // It takes 50 ticks of shortage while in Slum state to become Abandoned.
            for (int i = 0; i < 50; i++)
            {
                Assert.Equal(PlotState.Slum, plot.State);
                universe.Tick();
            }

            Assert.Equal(PlotState.Abandoned, plot.State);
            Assert.Null(plot.Consumer); // Consumer should be removed when abandoned.
        }
    }
}