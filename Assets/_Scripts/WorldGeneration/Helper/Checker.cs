using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.WorldGeneration.Helper
{
    /// <summary>
    ///     This is an independent class with independent parameters.
    /// </summary>
    public static class Checker
    {
        // IDs for rabbit and fox
        public static int RabbitID = 0;
        public static int FoxID = 0;

        // Counter for rabbit and fox
        public static int RabbitCounter = 0;
        public static int FoxCounter = 0;

        public static List<GameObject> BurrowList;

        // A boolean which is true if the simulation is running.
        public static bool Running { get; set; }

        public static bool IsStartingFromMenu = false;

        // public static void CheckCarrots(Plants plants)
        // {
        //     foreach (var plantAssets in from plant in plants.PlantsList
        //              where plant.name.Contains("Carrot") select plant.assets)
        //     {
        //         for (int i = 0; i < plantAssets.Count; i++)
        //         {
        //             plantAssets[i].TryGetComponent(out Carrot carrot);
        //             carrot.Check();
        //         }
        //     }
        // }
    }
}