using System.Collections.Generic;
using InGameTime;
using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Plant")]
    public class PlantSettings : ScriptableObject
    {
        public GameObject plantPrefab;
        public InGameDate lifespan;
        public int maxNumber;

        [HideInInspector] public List<GameObject> plants;
    }
}