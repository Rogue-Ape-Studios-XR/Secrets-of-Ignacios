using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Earth
{
    internal class Resizable : EarthInteractable
    {
        [SerializeField] private Vector3 _shrinkSize;
        [SerializeField] private Vector3 _defaultSize;
        [SerializeField] private Vector3 _growSize;

        [SerializeField] private Transform _targetObject;

        [SerializeField] private ResizeState _currentState = ResizeState.Default;

        internal event Action onSizeChanged;

        public ResizeState CurrentState => _currentState;

        internal override void Awake()
        {
            base.Awake();
            SetObjectScale();
        }

        internal override void Touched()
        {
            if (_isGrowSpellActive)
            {
                switch (_currentState)
                {
                    case ResizeState.Default:
                        ChangeState(ResizeState.Grown, _growSize);
                        break;

                    case ResizeState.Shrunk:
                        ChangeState(ResizeState.Default, _defaultSize);
                        break;

                }
            }
            else if (_isShrinkSpellActive)
            {
                switch (_currentState)
                {
                    case ResizeState.Default:
                        ChangeState(ResizeState.Shrunk, _shrinkSize);
                        break;

                    case ResizeState.Grown:
                        ChangeState(ResizeState.Default, _defaultSize);
                        break;
                }
            }
               
        }

        private void ChangeState(ResizeState newState, Vector3 newSize)
        {
            _targetObject.localScale = newSize;
            _currentState = newState;
            onSizeChanged?.Invoke();

            _isGrowSpellActive = false;
            _isShrinkSpellActive = false;
        }

        private void SetObjectScale()
        {
            _targetObject.localScale = _currentState switch
            {
                ResizeState.Shrunk => _shrinkSize,
                ResizeState.Grown => _growSize,
                _ => _defaultSize,
            };
        }
    }
}
