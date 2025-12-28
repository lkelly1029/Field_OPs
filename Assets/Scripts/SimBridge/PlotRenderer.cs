using UnityEngine;
using Sovereign.Sim;
using Sovereign.Sim.Buildings;
using System.Collections.Generic;

namespace SovereignState.Unity.SimBridge
{
    public class PlotRenderer : MonoBehaviour
    {
        public SimulationRunner runner;
        public GameObject plotPrefab; 
        public float spacing = 1.1f;

        private Dictionary<Vector2Int, GameObject> _plotObjects = new Dictionary<Vector2Int, GameObject>();
        private bool _gridGenerated = false;

        void Start()
        {
            if (runner != null)
            {
                runner.OnUniverseLoaded += OnUniverseReset;
            }
        }

        void OnDestroy()
        {
            if (runner != null)
            {
                runner.OnUniverseLoaded -= OnUniverseReset;
            }
        }

        private void OnUniverseReset()
        {
            ClearVisuals();
            _gridGenerated = false;
        }

        private void ClearVisuals()
        {
            foreach (var kvp in _plotObjects)
            {
                if (kvp.Value != null) Destroy(kvp.Value);
            }
            _plotObjects.Clear();
        }

        void GenerateVisualGrid()
        {
            var universe = runner.GetUniverse();
            if (universe == null) return;

            foreach (var plot in universe.Plots)
            {
                Vector3 pos = new Vector3(plot.X * spacing, 0, plot.Y * spacing);
                GameObject obj = plotPrefab != null 
                    ? Instantiate(plotPrefab, pos, Quaternion.identity, transform)
                    : GameObject.CreatePrimitive(PrimitiveType.Cube);
                
                if (plotPrefab == null)
                {
                    obj.transform.position = pos;
                    obj.transform.parent = transform;
                }

                _plotObjects[new Vector2Int(plot.X, plot.Y)] = obj;
            }
            _gridGenerated = true;
        }

        void Update()
        {
            var universe = runner?.GetUniverse();
            if (universe == null) return;

            if (!_gridGenerated)
            {
                GenerateVisualGrid();
            }

            foreach (var plot in universe.Plots)
            {
                UpdatePlotVisual(plot);
            }
        }
        
        public void UpdatePlotVisual(Plot plot)
        {
            var coord = new Vector2Int(plot.X, plot.Y);
            if (_plotObjects.TryGetValue(coord, out var obj))
            {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = GetColorForPlot(plot);
                }
            }
        }

        private Color GetColorForPlot(Plot plot)
        {
            if (plot.State == PlotState.Empty) return Color.gray;
            if (plot.State == PlotState.Slum) return new Color(0.6f, 0.4f, 0.2f); // Brown
            if (plot.State == PlotState.Abandoned) return Color.red;

            // Active/Built: Check content
            if (plot.Producer is NuclearPlant) return Color.magenta;
            if (plot.Producer is WaterPump) return Color.cyan;
            if (plot.Producer is IronMine) return new Color(0.8f, 0.5f, 0.5f); // Rust
            if (plot.Producer is SteelMill) return Color.black;
            
            if (plot.Consumer is House) return Color.green;
            if (plot.Consumer is Farm) return new Color(1f, 0.8f, 0.4f); // Wheat

            return Color.white; // Generic
        }
    }
}