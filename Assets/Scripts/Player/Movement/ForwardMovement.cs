using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Player.Movement
{
    public class ForwardMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController _player;
        [SerializeField] private Transform _camera;
        [SerializeField] private float _speed;
        [SerializeField] private float _gravityValue = -0.1f;

        private Vector3 _playerVelocity;
        private bool _movementEnabled = false;

        private void FixedUpdate()
        {
            if (_movementEnabled)
                Move();

            AddGravity();
        }

        private void Move()
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

        public void EnableMovement()
        {
            _movementEnabled = true;
        }

        public void DisableMovement()
        {
            _movementEnabled = false;
        }
    }
}
