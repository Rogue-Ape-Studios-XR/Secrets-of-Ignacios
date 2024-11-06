using System;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Earth;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.EarthRoom
{
    public class EarthGateChecker : MonoBehaviour
    {
        [SerializeField] private Resizable[] _resizableBlocks;
        [SerializeField] private Animator _animator;

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
                _animator.Play("BigGateOpen");
                Debug.Log("Open the gate");
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
    }
}
