using RogueApeStudios.SecretsOfIgnacios.Interactables.Fire;
using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.WaterRoom
{
    public class WaterTorchPuzzleCheck : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<PersistentFire> _torches;
        [SerializeField] private Animator _animator;
        
        [SerializeField] private List<GameObject> _areasToUnlock;

        [Header("Values")]
        [SerializeField] private int _torchAmount = 2;

        private void Awake()
        {

            foreach (var torch in _torches)
                torch.OnIgnitionToggle += TorchCheck;
        }

        private void OnDestroy()
        {
            foreach (var torch in _torches)
                torch.OnIgnitionToggle -= TorchCheck;
        }

        private void TorchCheck(bool onFire)
        {
            int count = 0;
            foreach (var torch in _torches)
                if (!torch._isOnFire)
                    count++;

            if (count == _torchAmount)
            {
                _animator.SetTrigger("GateOpen");
                UnlockAreas();
            }
        }
        
        private void UnlockAreas()
        {
            foreach (var area in _areasToUnlock)
            {
                if (area != null)
                {
                    ProgressionData progressionData = new()
                    {
                        Type = ProgressionType.AreaUnlock,
                        Data = new AreaUnlockData { Area = area }
                    };

                    ProgressionManager.TriggerProgressionEvent(progressionData);
                }
            }
        }
    }
}
