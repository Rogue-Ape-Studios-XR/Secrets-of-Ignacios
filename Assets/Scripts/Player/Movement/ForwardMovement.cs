using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Player.Movement
{
    public class ForwardMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController _player;
        [SerializeField] private Transform _camera;
        [SerializeField] private float _speed;

        private bool _movementEnabled = false;

        private void FixedUpdate()
        {
            if (_movementEnabled)
                Move();
        }

        private void Move()
        {
            Vector3 moveDirection = _camera.forward;

            moveDirection.y = 0;

            moveDirection.Normalize();
            _player.Move(_speed * Time.deltaTime * moveDirection);
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
