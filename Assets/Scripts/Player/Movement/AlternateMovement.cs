using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Player.Movement
{
    [Serializable]
    internal struct Hand
    {
        [SerializeField] internal Transform _thumbTip;
        [SerializeField] internal Transform _ancherPoint;
        [SerializeField] internal GameObject _visual;
    }

    internal class AlternateMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _player;
        [SerializeField] private Transform _rightThumbTip;
        [SerializeField] private Transform _leftThumbTip;
        [SerializeField] private Transform _rightAncherPoint;
        [SerializeField] private Transform _leftAncherPoint;
        [SerializeField] private Transform _camera;
        [SerializeField] private Hand _rightHand;
        [SerializeField] private Hand _leftHand;

        [Header("Visuals")]
        [SerializeField] private GameObject _rightVisual;
        [SerializeField] private GameObject _leftVisual;

        [Header("Variables")]
        [SerializeField] private float _maxBallDistance = 0.1f;
        [SerializeField] private float _speed = 3f;
        [SerializeField] private float _gravityValue = -0.1f;

        [Header("Movement Type")]
        [SerializeField] private bool _forwardMovement = true;

        private Vector3 _playerVelocity;
        private bool _movementEnabled = false;
        private bool _rightHandActive = false;
        private bool _leftHandActive = false;
        private bool _gravityActive = true;

        void Update()
        {

            if (_forwardMovement)
            {
                if (_movementEnabled)
                    ForwardMove();
            }
            else
            {
                if (_movementEnabled)
                    AlternateMove();
                else if (_rightVisual.activeSelf && _leftVisual.activeSelf || _rightVisual.activeSelf || _leftVisual.activeSelf)
                {
                    _rightVisual.SetActive(false);
                    _leftVisual.SetActive(false);
                }
            }

            if (_gravityActive)
                AddGravity();
        }

        private void AlternateMove()
        {
            Vector3 moveDirection;

            if (_rightHandActive || _rightHandActive && _leftHandActive)
            {
                _rightHand._visual.SetActive(true);
                moveDirection = _rightHand._thumbTip.position - _rightHand._ancherPoint.position;
            }
            else if (_leftHandActive)
            {
                _leftHand._visual.SetActive(true);
                moveDirection = _leftHand._thumbTip.position - _leftHand._ancherPoint.position;
            }
            else
            {
                _rightHand._visual.SetActive(false);
                _leftHand._visual.SetActive(false);
                moveDirection = Vector3.zero;
            }

            moveDirection.y = 0;

            moveDirection.Normalize();
            _player.Move(_speed * Time.deltaTime * moveDirection);
        }

        private void ForwardMove()
        {
            Vector3 moveDirection = _camera.forward;

            moveDirection.y = 0;

            moveDirection.Normalize();
            _player.Move(_speed * Time.deltaTime * moveDirection);
        }

        private void AddGravity()
        {
            if (!_player.isGrounded)
                _playerVelocity.y += _gravityValue * Time.deltaTime;
            else if (_player.isGrounded && _playerVelocity.y is not 0)
                _playerVelocity.y = 0f;

            _player.Move(_playerVelocity);
        }

        public void EnableMovementRight()
        {
            _movementEnabled = true;
            _rightHandActive = true;
        }

        public void EnableMovementLeft()
        {
            _movementEnabled = true;
            _leftHandActive = true;
        }

        public void DisableMovementRight()
        {
            _rightHandActive = false;
        }

        public void DisableMovementLeft()
        {
            _leftHandActive = false;
        }
    }
}

