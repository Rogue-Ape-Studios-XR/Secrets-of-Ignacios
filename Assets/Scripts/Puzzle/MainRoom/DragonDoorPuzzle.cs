using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.MainRoom
{
    public class DragonDoorPuzzle : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Potion"))
            {
                other.gameObject.SetActive(false);
                gameObject.SetActive(false);
                _animator.SetTrigger("DoorOpen");
            }
        }
    }
}
