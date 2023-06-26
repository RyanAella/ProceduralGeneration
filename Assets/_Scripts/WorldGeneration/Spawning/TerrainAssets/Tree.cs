/*
* Copyright (c) mmq
*/

using _Scripts.WorldGeneration.ScriptableObjects;
using UnityEngine;

namespace _Scripts.WorldGeneration.Spawning.TerrainAssets
{
    public class Tree : MonoBehaviour
    {
        #region Variables
        
        public PlantSettings settings;

        // private
        // private InGameDate _birthDate;
        // private InGameDate _dayOfDeath;
        // private TimeManager _timer;
        
        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            // Get the size and calculate the radius
            var size = gameObject.GetComponent<Renderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x : size.z;
        }

        // private void Start()
        // {
        //     _timer = TimeManager.Instance;
        //     _birthDate = _timer.GetCurrentDate();
        //     settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
        //     _dayOfDeath = new InGameDate().CalcDate(_birthDate.AddDates(settings.lifespan));
        // }

        // private void Update()
        // {
        //     if (_dayOfDeath.Equals(_timer.GetCurrentDate())) Dying();
        // }

        // private void Dying()
        // {
        //     settings.assets.Remove(gameObject);
        //     Destroy(gameObject);
        // }
        
        #endregion
    }
}