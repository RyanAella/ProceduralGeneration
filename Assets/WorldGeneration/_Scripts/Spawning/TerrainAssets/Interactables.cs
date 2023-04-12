using UnityEngine;

namespace WorldGeneration._Scripts.Spawning.TerrainAssets
{
    public interface Interactables
    {
        public bool Interact(GameObject interactingObject);
    }
}