using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Player.Movement
{
    internal class AlternateMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController _player;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;

        [Header("Visual")]
        [SerializeField] private Transform _rightAncherPoint;
        [SerializeField] private Transform _leftAncherPoint;
        [SerializeField] private Transform _rightBall;
        [SerializeField] private Transform _leftBall;

        [Header("Variables")]
        [SerializeField] private float _maxBallDistance = 0.1f;
        [SerializeField] private float _speed = 3f;

        private bool _movementEnabled = false;
        private bool _rightHandActive = false;
        private bool _leftHandActive = false;

        void Update()
        {
            if (_movementEnabled)
            {
                //MoveVisual();
                Move();
            }
        }

        private void MoveVisual()
        {
            if (_rightHandActive || _rightHandActive && _leftHandActive)
            {
                Vector3 rightHandDirection = _rightHand.position - _rightAncherPoint.position;
                rightHandDirection.y = 0;

                if (rightHandDirection.magnitude > _maxBallDistance)
                    rightHandDirection = rightHandDirection.normalized * _maxBallDistance;

                _rightBall.position = _rightAncherPoint.position + rightHandDirection;
            }
            else if (_leftHandActive)
            {
                Vector3 leftHandDirection = _leftHand.position - _leftAncherPoint.position;
                leftHandDirection.y = 0;

                if (leftHandDirection.magnitude > _maxBallDistance)
                    leftHandDirection = leftHandDirection.normalized * _maxBallDistance;

                _leftBall.position = _leftAncherPoint.position + leftHandDirection;
            }
        }

        private void Move()
        {
            Vector3 moveDirection;

            if (_rightHandActive || _rightHandActive && _leftHandActive)
                moveDirection = _rightHand.position - _rightAncherPoint.position;
            else if (_leftHandActive)
                moveDirection = _leftBall.position - _leftAncherPoint.position;
            else
                return;

            moveDirection.y = 0;

            moveDirection.Normalize();
            _player.Move(_speed * Time.deltaTime * moveDirection);
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

