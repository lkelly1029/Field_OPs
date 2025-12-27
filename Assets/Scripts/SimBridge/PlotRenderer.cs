using UnityEngine;
using Sovereign.Sim;
using System.Collections.Generic;

namespace SovereignState.Unity.SimBridge
{
    public class PlotRenderer : MonoBehaviour
    {
        public SimulationRunner runner;
        public GameObject plotPrefab; 
        public int width = 10;
        public int height = 10;
        public float spacing = 1.1f;

        private Dictionary<Vector2Int, GameObject> _plotObjects = new Dictionary<Vector2Int, GameObject>();

        void Start()
        {
            if (runner != null)
            {
                InitializeUniverseGrid();
            }
            GenerateVisualGrid();
        }

        void InitializeUniverseGrid()
        {
            var universe = runner.GetUniverse();
            if (universe == null) return;

            // Clear any existing plots in universe to ensure match
            // (In a real scenario, we'd load existing plots)
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    universe.AddPlot(new Plot { X = x, Y = y, State = PlotState.Empty });
                }
            }
        }

        void GenerateVisualGrid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 pos = new Vector3(x * spacing, 0, y * spacing);
                    GameObject obj = plotPrefab != null 
                        ? Instantiate(plotPrefab, pos, Quaternion.identity, transform)
                        : GameObject.CreatePrimitive(PrimitiveType.Cube);
                    
                    if (plotPrefab == null)
                    {
                        obj.transform.position = pos;
                        obj.transform.parent = transform;
                    }

                    _plotObjects[new Vector2Int(x, y)] = obj;
                }
            }
        }

        void Update()
        {
            var universe = runner?.GetUniverse();
            if (universe == null) return;

            foreach (var plot in universe.Plots)
            {
                UpdatePlotVisual(new Vector2Int(plot.X, plot.Y), plot.State);
            }
        }
        
        public void UpdatePlotVisual(Vector2Int coord, PlotState state)
        {
            if (_plotObjects.TryGetValue(coord, out var obj))
            {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = GetColorForState(state);
                }
            }
        }

        private Color GetColorForState(PlotState state)
        {
            switch (state)
            {
                case PlotState.Empty: return Color.gray;
                case PlotState.Built: return Color.blue;
                case PlotState.Active: return Color.green;
                case PlotState.Slum: return Color.yellow;
                case PlotState.Abandoned: return Color.red;
                default: return Color.white;
            }
        }
    }
}