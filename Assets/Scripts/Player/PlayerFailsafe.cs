using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Player
{
    public class PlayerFailSafe : MonoBehaviour
    {
        [SerializeField] Transform _playerSpawnPos;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.position = _playerSpawnPos.position;   
            }
        }
    }
}
