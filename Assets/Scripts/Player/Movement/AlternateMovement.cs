using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Player.Movement
{
    internal class AlternateMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _player;
        [SerializeField] private Transform _rightThumbTip;
        [SerializeField] private Transform _leftThumbTip;
        [SerializeField] private Transform _rightAncherPoint;
        [SerializeField] private Transform _leftAncherPoint;

        [Header("Visuals")]
        [SerializeField] private GameObject _rightVisual;
        [SerializeField] private GameObject _leftVisual;

        [Header("Variables")]
        [SerializeField] private float _maxBallDistance = 0.1f;
        [SerializeField] private float _speed = 3f;

        private bool _movementEnabled = false;
        private bool _rightHandActive = false;
        private bool _leftHandActive = false;

        void Update()
        {
            if (_movementEnabled)
                Move();
            else if (_rightVisual.activeSelf && _leftVisual.activeSelf || _rightVisual.activeSelf || _leftVisual.activeSelf)
            {
                _rightVisual.SetActive(false);
                _leftVisual.SetActive(false);
            }
        }

        private void Move()
        {
            Vector3 moveDirection;

            if (_rightHandActive || _rightHandActive && _leftHandActive)
            {
                _rightVisual.SetActive(true);
                moveDirection = _rightThumbTip.position - _rightAncherPoint.position;
            }
            else if (_leftHandActive)
            {
                _leftVisual.SetActive(true);
                moveDirection = _leftThumbTip.position - _leftAncherPoint.position;
            }
            else
            {
                _rightVisual.SetActive(false);
                _leftVisual.SetActive(false);
                moveDirection = Vector3.zero;
            }

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

