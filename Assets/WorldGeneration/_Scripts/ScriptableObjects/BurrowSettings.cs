using System;
using _Scripts.Time;
using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/Burrow")]
    [Serializable]
    public class BurrowSettings : ScriptableObject
    {
        public InGameDate lifespan;
        [HideInInspector] public bool isOccupied;
    }
}
