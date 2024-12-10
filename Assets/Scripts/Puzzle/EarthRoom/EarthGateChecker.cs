using System;
using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Earth;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.EarthRoom
{
    public class EarthGateChecker : MonoBehaviour
    {
        [SerializeField] private Resizable[] _resizableBlocks;
        [SerializeField] private Animator _animator;

        [SerializeField] private List<GameObject> _areasToUnlock;

        private void Awake()
        {
            foreach (var resizable in _resizableBlocks)
            {
                if (resizable != null)
                    resizable.onSizeChanged += CheckGateStatus;
            }
        }

        private void OnDestroy()
        {
            foreach (var resizable in _resizableBlocks)
            {
                if (resizable != null)
                    resizable.onSizeChanged -= CheckGateStatus;
            }
        }

        private void CheckGateStatus()
        {
            if (AreAllBlocksInState(ResizeState.Grown))
            {
                 _animator.Play("BigGateOpen");
                Debug.Log("Open the gate");
                UnlockAreas();   
            }
        }

        private bool AreAllBlocksInState(ResizeState requiredState)
        {
            foreach (var resizable in _resizableBlocks)
            {
                if (resizable == null || resizable.CurrentState != requiredState)
                {
                    Debug.Log("Not all blocks are in the required state: " + requiredState);
                    return false;
                }
            }

            return true;
        }

        private void UnlockAreas()
        {

            foreach (var area in _areasToUnlock)
            {
                if (area != null)
                {
                    ProgressionData progressionData = new ProgressionData
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
