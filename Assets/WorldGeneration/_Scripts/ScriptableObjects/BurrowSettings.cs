using System;
using System.Collections.Generic;
using InGameTime;
using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Burrow")]
    [Serializable]
    public class BurrowSettings : ScriptableObject
    {
        public GameObject assetPrefab;
        public List<Transform> parent;

        [Header("General Settings")] 
        public int minNumber;
        public float radius;
        public int initialInhabitants;

        public InGameDate lifespan;

        [HideInInspector] public List<GameObject> assets;
    }
}