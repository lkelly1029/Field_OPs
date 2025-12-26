using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class MaterialFactory : MonoBehaviour
{
    // --- MAIN MENU BUTTON ---
    [MenuItem("Tools/Create Test Materials")]
    public static void CreateMaterials()
    {
        // 1. Gold
        CreateURPMaterial("Gold", new Color(1f, 0.84f, 0f), 1.0f, 0.9f);

        // 2. Red Plastic
        CreateURPMaterial("RedPlastic", Color.red, 0.0f, 0.5f);

        // 3. Glass (Blue Tint, Transparent)
        CreateURPMaterial("Glass", new Color(0f, 0.5f, 1f, 0.3f), 0.5f, 0.95f, true);

        // 4. Concrete (Grey, Matte, Rough)
        CreateURPMaterial("Concrete", Color.gray, 0.0f, 0.2f);
        
        // Refresh Unity so they appear
        AssetDatabase.Refresh();
    }

    // --- HELPER FUNCTION (The Definition) ---
    // This is the set of instructions the code above is looking for.
    private static void CreateURPMaterial(string name, Color color, float metallic, float smoothness, bool isTransparent = false)
    {
        // Find the URP shader
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) 
        {
            Debug.LogError("URP Shader not found. Check your render pipeline settings.");
            return;
        }

        // Create the material
        Material mat = new Material(shader);
        
        // Apply settings (URP uses _BaseColor)
        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);

        // Handle Transparency
        if (isTransparent)
        {
            mat.SetFloat("_Surface", 1); // 1 = Transparent
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
        }

        // Save file
        string path = $"Assets/_Testing/Generated/{name}.mat";
        AssetDatabase.CreateAsset(mat, path);
        Debug.Log($"Created Material: {path}");
    }
}