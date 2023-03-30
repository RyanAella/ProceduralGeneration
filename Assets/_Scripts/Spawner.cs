using UnityEngine;

namespace _Scripts
{
    public class Spawner
    {
        public void SpawnBurrow(GameObject burrow, Vector3 burrowPos)
        {
            // float xSize = mesh.
            // Vector3 newPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
            // GameObject octo = Instantiate(enemyPrefab, newPos, Quaternion.identity) as GameObject;
            
            Object.Instantiate(burrow, burrowPos, Quaternion.identity);
        }
        
        public void SpawnCarrot(GameObject carrot, Vector3 carrotPos)
        {
            // float xSize = mesh.
            // Vector3 newPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
            // GameObject octo = Instantiate(enemyPrefab, newPos, Quaternion.identity) as GameObject;
            
            Object.Instantiate(carrot, carrotPos, Quaternion.identity);
        }
        
    }
}