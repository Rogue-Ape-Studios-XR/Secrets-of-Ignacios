using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.EarthRoom
{
    public class ShieldFailsafe : MonoBehaviour
    {
        [SerializeField] Transform _shieldSpawnPosition;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Shield"))
            {
                other.transform.position = _shieldSpawnPosition.position;   
            }
        }
    }
}
