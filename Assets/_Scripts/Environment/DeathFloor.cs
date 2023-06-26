using _Scripts.WorldGeneration.Helper;
using UnityEngine;

namespace _Scripts.Environment
{
    public class DeathFloor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Rabbit") || other.gameObject.tag.Equals("Fox"))
            {
                if (other.gameObject.tag.Equals("Rabbit"))
                {
                    Checker.RabbitCounter--;
                }

                if (other.gameObject.tag.Equals("Fox"))
                {
                    Checker.FoxCounter--;
                }

                Debug.Log(other.gameObject.tag + " killed by DeathFloor");
                Destroy(other.gameObject);
            }
        }
    }
}