using System;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Earth;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.EarthRoom
{
    public class EarthGateChecker : MonoBehaviour
    {
        [SerializeField] private Resizable[] _resizableBlocks;

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
            if (AreAllBlocksGrown())
                //Later implementation of Loes' animation
                Debug.Log("Open the gate");
        }

        private bool AreAllBlocksGrown()
        {
            foreach (var resizable in _resizableBlocks)
            {
                if (resizable == null || !resizable.Grown)
                {
                    Debug.Log("Not all are grown");
                    return false;
                }
            }

            return true;
        }
    }
}
