using System.Collections.Generic;
using InGameTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Plant")]
    public class PlantSettings : ScriptableObject
    {
        public List<GameObject> assetPrefab;
        public Transform parent;
        public int minNumber;
        
        public float radius = 0;

        public bool nearWater;
        
        [Header("Age and Fertility")]
        public InGameDate lifespan;
        public float percentageAge;
        public float reproductionChance;




        /*[HideInInspector] */public List<GameObject> assets;
    }
}