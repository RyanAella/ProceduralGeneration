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
                switch (other.gameObject.tag)
                {
                    case "Rabbit":
                        Checker.RabbitCounter--;
                        break;
                    case "Fox":
                        Checker.FoxCounter--;
                        break;
                }

                Destroy(other.gameObject);
            }
        }
    }
}