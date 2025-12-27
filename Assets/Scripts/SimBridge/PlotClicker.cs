using UnityEngine;
using Sovereign.Sim;
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
            HandleHover();

            if (Input.GetMouseButtonDown(0)) // Left Click
            {
                HandleClick();
            }
        }

        void HandleHover()
        {
            if (TooltipManager.Instance == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Assuming PlotRenderer creates objects named or tagged in a way we can identify,
                // OR we just use the position to calculate grid coordinates.
                // Since PlotRenderer places them at (x * spacing, 0, y * spacing)
                
                // Let's rely on PlotRenderer structure. The hit.collider.gameObject is the cube.
                // We need to find which X,Y this corresponds to.
                // For MVP, simple math based on position is robust.
                
                var renderer = FindFirstObjectByType<PlotRenderer>();
                if (renderer == null) return;

                float spacing = renderer.spacing;
                Vector3 localPos = hit.transform.position; // Assuming parent is at (0,0,0) or we convert
                
                int x = Mathf.RoundToInt(localPos.x / spacing);
                int y = Mathf.RoundToInt(localPos.z / spacing);

                // Now find the engine plot
                var universe = runner.GetUniverse();
                if (universe == null) return;

                var plot = universe.Plots.FirstOrDefault(p => p.X == x && p.Y == y);
                if (plot != null)
                {
                    // TODO: Implement BuildingManager and Commands
                    // string selected = BuildingManager.Instance?.SelectedBuilding ?? "None";
                    // 
                    // if (selected == "None") return;
                    //
                    // PlotState nextState = selected == "Clear" ? PlotState.Empty : PlotState.Active;
                    // 
                    // var cmd = new BuildCommand(x, y, nextState, selected);
                    // universe.ProcessCommand(cmd);
                    // 
                    // Debug.Log($"Sent BuildCommand for ({x}, {y}) -> {selected}");
                }
            }
        }
    }
}
