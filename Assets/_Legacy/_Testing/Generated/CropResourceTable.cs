using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCropResourceTable", menuName = "Game/Crop Resource Table")]
public class CropResourceTable : ScriptableObject
{
    [System.Serializable]
    public class CropData
    {
        public string id;
        public string displayName;
        public Sprite icon;
        public GameObject prefab;
        
        [Tooltip("Time in seconds for the crop to fully grow")]
        public float growthTime = 10f;
        
        public int seedCost = 10;
        public int sellPrice = 20;
    }

    public List<CropData> crops = new List<CropData>();

    public CropData GetCrop(string id)
    {
        return crops.Find(c => c.id == id);
    }
}