using RogueApeStudios.SecretsOfIgnacios.Interactables.Water;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.WaterRoom
{
    public class GatePuzzleCheck : MonoBehaviour
    {
        [SerializeField] private List<Fillable> _vases;
        [SerializeField] private Animator _animator;

        private void Awake()
        {

            foreach (var vase in _vases)
            {
                vase.OnSplashToggle += VaseCheck;
                
            }
        }

        private void OnDestroy()
        {
            foreach (var vase in _vases)
            {
                vase.OnSplashToggle -= VaseCheck;
            }
        }

        private void VaseCheck(bool filled)
        {
            int count = 0;
            foreach (var vase in _vases)
            {
                if (vase._filled /*&& vase.transform.localScale.x == 1f*/)
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
    }
}
