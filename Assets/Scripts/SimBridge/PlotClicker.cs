using UnityEngine;
using UnityEngine.InputSystem;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using System.Linq;

namespace SovereignState.Unity.SimBridge
{
    public class PlotClicker : MonoBehaviour
    {
        public SimulationRunner runner;
        public Camera mainCamera;

        void Start()
        {
            if (mainCamera == null) mainCamera = Camera.main;
        }

        void Update()
        {
            if (Mouse.current == null) return;

            HandleHover();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleClick();
            }
        }

        void HandleHover()
        {
            if (TooltipManager.Instance == null) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var renderer = FindFirstObjectByType<PlotRenderer>();
                if (renderer == null) return;

                float spacing = renderer.spacing;
                Vector3 localPos = hit.transform.position; 
                
                int x = Mathf.RoundToInt(localPos.x / spacing);
                int y = Mathf.RoundToInt(localPos.z / spacing);

                var universe = runner.GetUniverse();
                if (universe != null)
                {
                    TooltipManager.Instance.HoveredPlot = universe.Plots.FirstOrDefault(p => p.X == x && p.Y == y);
                    return;
                }
            }
            
            // Clear if nothing hit
            TooltipManager.Instance.HoveredPlot = null;
        }

        void HandleClick()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var renderer = FindFirstObjectByType<PlotRenderer>();
                if (renderer == null) return;

                float spacing = renderer.spacing;
                Vector3 localPos = hit.transform.position;
                
                int x = Mathf.RoundToInt(localPos.x / spacing);
                int y = Mathf.RoundToInt(localPos.z / spacing);

                var universe = runner.GetUniverse();
                if (universe == null) return;

                var plot = universe.Plots.FirstOrDefault(p => p.X == x && p.Y == y);
                if (plot != null)
                {
                    string selected = BuildingManager.Instance?.SelectedBuilding ?? "None";
                    
                    if (selected == "None") return;

                    // Building Logic
                    if (selected == "Clear")
                    {
                        plot.State = PlotState.Empty;
                        plot.Consumer = null;
                        plot.Producer = null;
                    }
                    else
                    {
                        plot.State = PlotState.Active;
                        plot.Consumer = null;
                        plot.Producer = null;

                        switch (selected)
                        {
                            case "House": plot.Consumer = new House(); break;
                            case "Farm": plot.Consumer = new Farm(); break;
                            case "WaterPump": plot.Producer = new WaterPump(); break;
                            case "IronMine": var im = new IronMine(); plot.Producer = im; plot.Consumer = im; break;
                            case "SteelMill": var sm = new SteelMill(); plot.Producer = sm; plot.Consumer = sm; break;
                            case "NuclearPlant": plot.Producer = new NuclearPlant(); break;
                        }
                    }
                    
                    Debug.Log($"Built {selected} at ({x}, {y})");
                }
            }
        }
    }
}
