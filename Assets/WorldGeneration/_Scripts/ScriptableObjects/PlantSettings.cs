using System.Collections.Generic;
using InGameTime;
using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Plant")]
    public class PlantSettings : ScriptableObject
    {
        public List<GameObject> assetPrefab;
        public Transform parent;

        [Header("General Settings")] public int minNumber;
        public float radius;
        
        public bool nearWater;

        [Header("Age and Reproduction")] 
        public InGameDate lifespan;
        public float percentageAge;
        public float reproductionChance;

        [HideInInspector] 
        public List<GameObject> assets;
    }
}