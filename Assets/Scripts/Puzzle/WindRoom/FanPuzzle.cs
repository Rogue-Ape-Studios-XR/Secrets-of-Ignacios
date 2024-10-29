using RogueApeStudios.SecretsOfIgnacios.Interactables.Wind;
using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.WindRoom
{
    public class FanPuzzle : MonoBehaviour
    {
        [SerializeField] private List<Spinnable> _fans;
        [SerializeField] private Animator _animator;

        private int _previousCount = 0;

        private void Awake()
        {
            foreach (var fan in _fans)
                fan.onSpinning += HandleOnSpinning;
        }

        private void OnDestroy()
        {
            foreach (var fan in _fans)
                fan.onSpinning -= HandleOnSpinning;
        }

        private void HandleOnSpinning(bool isSpinning)
        {
            int count = 0;

            foreach (var fan in _fans)
                if (fan.IsSpinning)
                    count++;

            if (count >= _fans.Count)
                _animator.SetTrigger("DoorOpen");
            else if (_previousCount is 3 && count < _fans.Count)
                _animator.SetTrigger("DoorClose");

            _previousCount = count;
        }
    }
}
