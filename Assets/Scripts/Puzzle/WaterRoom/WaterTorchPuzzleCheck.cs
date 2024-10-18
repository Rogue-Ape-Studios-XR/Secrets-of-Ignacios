using RogueApeStudios.SecretsOfIgnacios.Interactables.Fire;
using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.WaterRoom
{
    public class WaterTorchPuzzleCheck : MonoBehaviour
    {
        [SerializeField] private List<PersistentFire> _torches;
        [SerializeField] private Animator _animator;

        private void Awake()
        {

            foreach (var torch in _torches)
            {
                torch.OnIgnitionToggle += TorchCheck;
            }
        }

        private void OnDestroy()
        {
            foreach (var torch in _torches)
            {
                torch.OnIgnitionToggle -= TorchCheck;
            }
        }

        private void TorchCheck(bool onFire)
        {
            int count = 0;
            foreach (var torch in _torches)
            {
                if (!torch._isOnFire)
                {
                    count++;
                }
            }

            if (count == 3)
            {
                Debug.Log("Door opens");
                _animator.SetTrigger("DoorOpen");
            }
        }
    }
}
