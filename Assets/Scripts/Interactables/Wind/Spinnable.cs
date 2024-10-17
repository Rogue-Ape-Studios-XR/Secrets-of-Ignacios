using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Wind
{
    internal class Spinnable : WindInteractable
    {
        [Header("Object to rotate")]
        [SerializeField] private Transform _spinnableTransform;

        [Header("Spin settings")]
        [SerializeField] private float _maxSpeed = 100f;
        [SerializeField] private float _decelerationRate = 1f;
        [SerializeField] private float _hitForce = 20f;
        [SerializeField, Tooltip("Put any number in the axis you want the object to turn on")] private Vector3 _rotationAxis;

        private float _currentSpeed = 0f;
        private bool _isSpinning = false;

        internal bool IsSpinning => _isSpinning;

        internal event Action<bool> OnSpinning;

        void FixedUpdate()
        {
            if (!_isBlown && _currentSpeed > 0f)
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, _decelerationRate * Time.deltaTime);
                _spinnableTransform.Rotate(_rotationAxis, _currentSpeed * Time.deltaTime);

            }
            else if (_isBlown && _currentSpeed >= 0f)
            {
                Blown();
                _isBlown = false;
                _spinnableTransform.Rotate(_rotationAxis, _currentSpeed * Time.deltaTime);
            }
            else if (!_isBlown && _currentSpeed <= 0f && _isSpinning)
            {
                _isSpinning = false;
                OnSpinning?.Invoke(_isSpinning);
            }
        }

        internal override void Blown()
        {
            _currentSpeed = Mathf.Min(_currentSpeed + _hitForce, _maxSpeed);
            _isSpinning = true;
            OnSpinning?.Invoke(_isSpinning);
        }
    }
}
