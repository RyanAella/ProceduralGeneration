using System;
using System.Collections.Generic;
using _Scripts.InGameTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.WorldGeneration.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Plant")]
    public class PlantSettings : ScriptableObject
    {
        public List<GameObject> assetPrefab;
        public Transform parent;

        [Header("General Settings")] 
        [Tooltip("The max number of burrows to spawn.")]
        public int maxNumber;
        public int numberOfPlacementAttempts;
        public float radius;

        [Header("Only for Grass")] 
        public LayerMask layerMask;
        
        public bool nearWater;
        public bool inMountainArea;

        [Header("Age and Reproduction")] 
        public InGameDate lifespan;
        public float percentageAge;
        public float reproductionChance;

        [HideInInspector] public List<GameObject> assets;
    }
}