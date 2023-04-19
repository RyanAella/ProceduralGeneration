using System.Collections.Generic;
using InGameTime;
using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Plant")]
    public class AssetSettings : ScriptableObject
    {
        public GameObject assetPrefab;
        public Transform parent;
        public InGameDate lifespan;
        public int maxNumber;

        public List<GameObject> assets;
    }
}