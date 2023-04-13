using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration._Scripts.Spawning
{
    public abstract class AssetSpawner
    {
        public List<GameObject> Assets;
        public abstract void Spawn(GameObject spawningObject, Vector3 objectPos);

    }
}