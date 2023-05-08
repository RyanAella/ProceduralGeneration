using System;
using System.Collections.Generic;
using InGameTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Burrow")]
    [Serializable]
    public class BurrowSettings : ScriptableObject
    {
        public GameObject assetPrefab;
        public List<Transform> parent;
        public InGameDate lifespan;
        public int minNumber;

        public float radius;

        [HideInInspector] public List<GameObject> assets;
    }
}