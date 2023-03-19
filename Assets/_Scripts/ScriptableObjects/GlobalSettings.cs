using System;
using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    /**
     * This class stores the parameters for each generation step.
     */
    [CreateAssetMenu]
    [Serializable]
    public class GlobalSettings : ScriptableObject
    {
        // General
        public int durationPerMonthInMinutes = 1;
    }
}
