using UnityEngine;

namespace WorldGeneration._Scripts.Spawning.BurrowSpawner
{
    public class FoxBurrowSpawner : BurrowSpawner
    {
        public override void Spawn(GameObject burrow, Vector3 burrowPos)
        {
            // float xSize = mesh.
            // Vector3 newPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
            // GameObject octo = Instantiate(enemyPrefab, newPos, Quaternion.identity) as GameObject;
            
            Object.Instantiate(burrow, burrowPos, Quaternion.identity);
        }
    }
}
