using System;
using InGameTime;
using UnityEngine;

namespace WorldGeneration._Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Burrow")]
    [Serializable]
    public class BurrowSettings : ScriptableObject
    {
        public InGameDate lifespan;
        [HideInInspector] public bool isOccupied;
    }
}
