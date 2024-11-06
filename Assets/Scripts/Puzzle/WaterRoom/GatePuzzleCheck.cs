using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Water;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Earth;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.WaterRoom
{
    public class GatePuzzleCheck : MonoBehaviour
    {
        [SerializeField] private List<Fillable> _vases;
        [SerializeField] private List<Resizable> _resizableVases;
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            foreach (var vase in _vases)
            {
                vase.onFilled += VaseCheck;
            }

            foreach (var vase in _resizableVases)
            {
                vase.onSizeChanged += VaseCheck;
            }
        }

        private void OnDestroy()
        {
            foreach (var vase in _vases)
            {
                vase.onFilled -= VaseCheck;
            }

            foreach (var vase in _resizableVases)
            {
                vase.onSizeChanged -= VaseCheck;
            }
        }

        //This is literally only because the event is a bool...... it's not even used.
        private void VaseCheck(bool filled)
        {
            CheckVaseConditions();
        }

        private void VaseCheck()
        {
            CheckVaseConditions();
        }

        private void CheckVaseConditions()
        {
            int count = 0;

            foreach (var vase in _vases)
            {
                if (vase._filled && IsVaseGrown(vase))
                {
                    count++;
                }
            }

            if (count == 2)
            {
                Debug.Log("Gate opens");
                _animator.SetTrigger("GateOpen");
            }
        }
        
        private bool IsVaseGrown(Fillable vase)
        {
            foreach (var resizable in _resizableVases)
            {
                if (vase.gameObject == resizable.gameObject && resizable.CurrentState == ResizeState.Grown)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
