using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Player.Movement
{
    internal class AlternateMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _player;
        [SerializeField] private Transform _camera;
        [SerializeField] private Hand _rightHand;
        [SerializeField] private Hand _leftHand;

        [Header("Movement Settings")]
        [SerializeField] private float _speed = 3f;
        [SerializeField] private float _gravityValue = -0.1f;

        [Header("Movement Type")]
        [SerializeField] private bool _forwardMovement = true;

        private Vector3 _playerVelocity;
        private bool _movementEnabled = false;

        private void Update()
        {
            if (_movementEnabled)
            {
                if (_forwardMovement)
                    ForwardMove();
                else
                    AlternateMove();
            }
            else if (_rightHand._joystickVisual.activeSelf || _leftHand._joystickVisual.activeSelf)
                DisableHandVisuals();

            AddGravity();
        }

        private void AlternateMove()
        {
            Vector3 moveDirection = Vector3.zero;

            if (_rightHand._active || _rightHand._active && _leftHand._active)
            {
                _rightHand.ShowVisual();
                moveDirection = _rightHand._thumbTip.position - _rightHand._ancherPoint.position;
            }
            else if (_leftHand._active)
            {
                _leftHand.ShowVisual();
                moveDirection = _leftHand._thumbTip.position - _leftHand._ancherPoint.position;
            }
            else
                DisableHandVisuals();

            moveDirection.y = 0;
            _player.Move(_speed * Time.deltaTime * moveDirection.normalized);
        }

        private void ForwardMove()
        {
            Vector3 moveDirection = _camera.forward;
            moveDirection.y = 0;
            _player.Move(_speed * Time.deltaTime * moveDirection.normalized);
        }

        private void AddGravity()
        {
            if (!_player.isGrounded)
                _playerVelocity.y += _gravityValue * Time.deltaTime;
            else if (_player.isGrounded && _playerVelocity.y != 0)
                _playerVelocity.y = 0f;

            _player.Move(_playerVelocity);
        }

        public void EnableMovementRight()
        {
            if (!_forwardMovement)
            {
                _movementEnabled = true;
                _rightHand._active = true;
            }
        }

        public void EnableMovementLeft()
        {
            if (!_forwardMovement)
            {
                _movementEnabled = true;
                _leftHand._active = true;
            }
        }

        public void DisableMovementRight()
        {
            _rightHand._active = false;

            if (!_leftHand._active)
                _movementEnabled = false;
        }
        public void DisableMovementLeft()
        {
            _leftHand._active = false;

            if (!_rightHand._active)
                _movementEnabled = false;
        }

        public void EnableMovementForward()
        {
            if (_forwardMovement)
                _movementEnabled = true;
        }

        public void DisableMovementForward()
        {
            if (_forwardMovement)
                _movementEnabled = false;
        }

        private void DisableHandVisuals()
        {
            _rightHand.HideVisual();
            _leftHand.HideVisual();
        }

        [Serializable]
        private class Hand
        {
            [SerializeField] internal Transform _thumbTip;
            [SerializeField] internal Transform _ancherPoint;
            [SerializeField] internal GameObject _joystickVisual;

            internal bool _active = false;

            internal void ShowVisual() => _joystickVisual.SetActive(true);
            internal void HideVisual() => _joystickVisual.SetActive(false);
        }
    }
}
