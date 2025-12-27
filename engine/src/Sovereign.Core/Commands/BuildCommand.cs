using Sovereign.Sim;
using System.Linq;

namespace Sovereign.Core.Commands
{
    public class BuildCommand : ICommand
    {
        public int X { get; }
        public int Y { get; }
        public PlotState TargetState { get; }
        public string BuildingType { get; }

        public BuildCommand(int x, int y, PlotState targetState, string buildingType = null)
        {
            X = x;
            Y = y;
            TargetState = targetState;
            BuildingType = buildingType;
        }

        public void Execute(Universe universe)
        {
            var plot = universe.Plots.FirstOrDefault(p => p.X == X && p.Y == Y);
            if (plot != null)
            {
                plot.State = TargetState;
                
                // Factory logic for building type
                if (!string.IsNullOrEmpty(BuildingType))
                {
                    // Clear old
                    plot.Consumer = null;
                    plot.Producer = null;

                    if (BuildingType == "House") plot.Consumer = new Sovereign.Sim.Buildings.House();
                    else if (BuildingType == "Farm") plot.Consumer = new Sovereign.Sim.Buildings.Farm();
                    else if (BuildingType == "WaterPump") plot.Producer = new Sovereign.Sim.Buildings.WaterPump();
                    else if (BuildingType == "IronMine") { var m = new Sovereign.Sim.Buildings.IronMine(); plot.Producer = m; plot.Consumer = m; }
                    else if (BuildingType == "SteelMill") { var m = new Sovereign.Sim.Buildings.SteelMill(); plot.Producer = m; plot.Consumer = m; }
                    else if (BuildingType == "NuclearPlant") plot.Producer = new Sovereign.Sim.Buildings.NuclearPlant();
                    
                    if (BuildingType == "Clear")
                    {
                        plot.State = PlotState.Empty;
                        plot.Consumer = null;
                        plot.Producer = null;
                    }
                }
            }
        }
    }
}
