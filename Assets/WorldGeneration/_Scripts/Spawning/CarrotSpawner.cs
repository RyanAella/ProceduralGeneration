using UnityEngine;

namespace WorldGeneration._Scripts.Spawning
{
    public class CarrotSpawner : AssetSpawner
    {
        public override void Spawn(GameObject carrot, Vector3 carrotPos)
        {
            // Vector3 newPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
            // GameObject octo = Instantiate(enemyPrefab, newPos, Quaternion.identity) as GameObject;
            
            // float xSize = mesh.
            // Vector3 newPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
            // GameObject octo = Instantiate(enemyPrefab, newPos, Quaternion.identity) as GameObject;
            
            Object.Instantiate(carrot, carrotPos, Quaternion.identity);
        }
    }
}
