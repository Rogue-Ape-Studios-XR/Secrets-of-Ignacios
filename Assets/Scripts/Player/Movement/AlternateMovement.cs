//using UnityEngine;
//using static TMPro.TMP_Compatibility;

//namespace RogueApeStudios.SecretsOfIgnacios.Player.Movement
//{
//    internal class AlternateMovement : MonoBehaviour
//    {
//        [SerializeField] private CharacterController _player;
//        [SerializeField] private Transform _rightHand;  
//        [SerializeField] private Transform _leftHand;  
//        [Header("Visual")]
//        [SerializeField] private Transform _rightAncherPoint;  
//        [SerializeField] private Transform _leftAncherPoint;  
//        [SerializeField] private Transform _rightBall;
//        [SerializeField] private Transform _leftBall;
//        [Header("Variables")]
//        [SerializeField] private float _maxBallDistance = 0.1f; 
//        [SerializeField] private float _speed = 3f;       

//        private bool _movementEnabled = false;
//        private bool _rightHandActive = false;
//        private bool _leftHandActive = false;

//        void Update()
//        {
//            if (_movementEnabled)
//            {
//                MoveVisual(); 
//                Move();       
//            }
//        }

//        private void MoveVisual()
//        {
//            Vector3 handDirection;
//            Vector3 anchorPosition;

//            if (_rightHandActive && !_leftHandActive)
//            {
//                handDirection = _rightHand.position - _rightAncherPoint.position;
//                anchorPosition = _rightAncherPoint.position;
//            }
//            else if (_leftHandActive && !_rightHandActive)
//            {
//                handDirection = _leftHand.position - _leftAncherPoint.position;
//                anchorPosition = _leftAncherPoint.position;
//            }
//            else if (_rightHandActive && _leftHandActive)
//            {
//                handDirection = _rightHand.position - _rightAncherPoint.position;
//                anchorPosition = _rightAncherPoint.position;
//            }
//            else
//                return;  

//            handDirection.y = 0;

//            if (handDirection.magnitude > _maxBallDistance)
//                handDirection = handDirection.normalized * _maxBallDistance;

//            _rightBall.position = anchorPosition + handDirection;
//        }


//        private void Move()
//        {
//            Vector3 moveDirection;

//            if (_rightHandActive && !_leftHandActive)
//            moveDirection = _rightBall.position - _rightAncherPoint.position;
//            else if (_leftHandActive && !_rightHandActive)
//                moveDirection = _rightBall.position - _leftAncherPoint.position;
//            else if (_rightHandActive && _leftHandActive)
//                moveDirection = _rightBall.position - _rightAncherPoint.position;
//            else
//                return;

//            moveDirection.y = 0;

//            moveDirection.Normalize();
//            _player.Move(_speed * Time.deltaTime * moveDirection);
//        }

//        public void EnableMovementRight()
//        {
//            _movementEnabled = true;
//            _rightHandActive = true;
//        } 

//        public void EnableMovementLeft()
//        {
//            _movementEnabled = true;
//            _rightHandActive = true;
//        }

//        public void DisableMovementRight()
//        {
//            _movementEnabled = false;
//            _rightHandActive = false;
//        }

//        public void DisableMovementLeft()
//        {
//            _movementEnabled = false;
//            _leftHandActive = false;
//        }

//    }
//}

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
                MoveVisual();
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
                moveDirection = _rightBall.position - _rightAncherPoint.position;
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

