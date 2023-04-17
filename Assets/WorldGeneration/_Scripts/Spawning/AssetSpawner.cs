using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration._Scripts.Spawning
{
    public class AssetSpawner
    {
        // public List<GameObject> Assets;

        /// <summary>
        /// Spawn a GameObject at the given position.
        /// </summary>
        /// <param name="spawningObject">The asset that has to be instantiated.</param>
        /// <param name="objectPos">The position at which the asset has to be instantiated.</param>
        /// <returns>The instantiated GameObject / asset.</returns>
        public GameObject Spawn(GameObject spawningObject, Vector3 objectPos)
        {
            return Object.Instantiate(spawningObject, objectPos,
                Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        }
    }
}