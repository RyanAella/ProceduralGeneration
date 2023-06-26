using System;
using System.Collections.Generic;
using _Scripts.InGameTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.WorldGeneration.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Burrow")]
    [Serializable]
    public class BurrowSettings : ScriptableObject
    {
        public List<GameObject> burrowPrefabs;
        public List<Transform> parent;

        [Header("General Settings")]
        [Tooltip("The max number of burrows to spawn.")]
        public int maxNumber;
        public int numberOfPlacementAttempts;
        public float radius;
        public int initialInhabitants;

        public InGameDate lifespan;

        [HideInInspector] public List<GameObject> assets;
    }
}