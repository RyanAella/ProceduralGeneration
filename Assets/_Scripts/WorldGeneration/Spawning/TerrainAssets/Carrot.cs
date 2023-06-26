/*
* Copyright (c) mmq
*/

using _Scripts.InGameTime;
using _Scripts.ml_agents.Agents;
using _Scripts.WorldGeneration.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.WorldGeneration.Spawning.TerrainAssets
{
    public class Carrot : MonoBehaviour
    {
        #region Variables
        
        // public
        public PlantSettings settings;
        public float timeNeededForEating = 2f;
        public int nutritionValue = 5;

        // private
        public InGameDate birthDate;
        public InGameDate dayOfDeath;
        public InGameDate fertilityDate;
        private bool _getsEaten;
        private AssetManager _assetManager;
        private bool _reproductionChance;
        
        public bool isFertile;
        
        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            _assetManager = AssetManager.GetInstance();

            // Get the size and calculate the radius
            var size = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
            settings.radius = size.x >= size.z ? size.x : size.z;

            // Recalculate lifespan
            settings.lifespan = new InGameDate().CalcDate(settings.lifespan);
        }

        private void Start()
        {
            // Get the dates and check if they are in the correct form
            birthDate = TimeManager.GetCurrentDate();

            dayOfDeath = new InGameDate().CalcDate(birthDate.AddDates(settings.lifespan));

            InGameDate days = new InGameDate().CalcDays(settings.lifespan);
            days.day = (int)((settings.percentageAge / 100) * days.day);
            fertilityDate = new InGameDate().CalcDate(birthDate.AddDates(new InGameDate().CalcDate(days)));

            isFertile = false;
        }

        /// <summary>
        /// Check if the fertility age is reached and reproduce.
        /// Checks if the current date equals the day of death.
        /// If it's equal the carrot can be destroyed.
        /// </summary>
        public void Check()
        {
            if (dayOfDeath.Equals(TimeManager.GetCurrentDate())) Dying();
        
            // if (_fertilityDate.Equals(TimeManager.Instance.GetCurrentDate())) isFertile = true;
        
            // if (isFertile) Reproduce();
        }

        // private void Reproduce()
        // {
        //     if (this == null) return;
        //
        //     if (Random.Range(0, 101) <= (int)settings.reproductionChance)
        //     {
        //         _assetManager.Spawn(settings, transform.parent);
        //     }
        // }

        /// <summary>
        /// Delete and destroy the carrot.
        /// </summary>
        private void Dying()
        {
            try
            {
                if(settings.assets.Contains(gameObject))
                {
                    settings.assets.Remove(gameObject);
                    Destroy(gameObject);
                }
            }
            catch {
                Debug.LogWarning("Could not Destroy Carrot");
            }
        }

        /// <summary>
        /// The carrot can be eaten.
        /// </summary>
        /// <param name="agent">The agent which wants to eat the carrot</param>
        public void Eat(CustomAgent agent)
        {
            if (gameObject == null || !settings.assets.Contains(gameObject)) return;

            agent.BlockInteractionsForSeconds(timeNeededForEating);

            if ((agent.hunger - nutritionValue) >= -30)
            {
                agent.hunger -= nutritionValue;
            }
            else
            {
                agent.hunger = -30;
            }

            Dying();
        }
        
        #endregion
    }
}