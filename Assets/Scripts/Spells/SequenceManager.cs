using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SequenceManager : MonoBehaviour
    {
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private List<Gesture> _allGestures;
        [SerializeField] private bool _leftHandActive = false; // Only serialized for testing purposes
        [SerializeField] private bool _rightHandActive = false; // Only serialized for testing purposes

        private readonly List<Gesture> _validatedGestures = new();
        private CancellationTokenSource _cancellationTokenSource;
        private Gesture _currentGesture;
        private HandShape _leftHandShape;
        private HandShape _rightHandShape;
        private bool _gestureValidated = false;
        private bool _sequenceStarted = false;

        internal event Action OnSequenceCreated;
        internal event Action OnReset;

        internal List<Gesture> ValidatedGestures => _validatedGestures;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void FixedUpdate()
        {
            if (_leftHandActive && _rightHandActive && !_gestureValidated && _currentGesture != null)
                HandDistanceCheck(_currentGesture);
            else if (_leftHandActive && _rightHandActive && !_gestureValidated && _currentGesture == null)
                print(_currentGesture);
        }

        public void CheckGestures()
        {
            if (_leftHandActive && _rightHandActive && _currentGesture == null)
            {
                foreach (var gesture in _allGestures)
                    if (_leftHandShape == gesture._leftHandShape && _rightHandShape == gesture._rightHandShape)
                    {
                        _currentGesture = gesture;
                        break;
                    }
            }

            _gestureValidated = false;
        }

        private void OnDistanceValidated()
        {
            if (_currentGesture != null)
            {
                if (_currentGesture._name == "Start" && _leftHandActive && _rightHandActive)
                {
                    _validatedGestures.Clear();
                    OnReset?.Invoke();
                    _sequenceStarted = true;
                }

                if (_validatedGestures.Count == 0 && _sequenceStarted ||
                    _validatedGestures[^1] != _currentGesture && _sequenceStarted)
                {
                    _validatedGestures.Add(_currentGesture);
                    Debug.Log("Validated Gesture: " + _currentGesture._name);
                }

                if (_validatedGestures.Count == 3)
                {
                    _sequenceStarted = false;
                    OnSequenceCreated?.Invoke();
                }

                _currentGesture = null;
                _gestureValidated = false;
            }
        }

        private void HandDistanceCheck(Gesture gesture)
        {
            int distanceMultiplier = 10;
            float distanceX = Mathf.Abs(_leftHand.position.x - _rightHand.position.x) * distanceMultiplier;
            float distanceY = Mathf.Abs(_leftHand.position.y - _rightHand.position.y) * distanceMultiplier;
            float distanceZ = Mathf.Abs(_leftHand.position.z - _rightHand.position.z) * distanceMultiplier;

            if (gesture._xHandDistance > distanceX &&
                gesture._yHandDistance > distanceY &&
                gesture._zHandDistance > distanceZ)
            {
                _gestureValidated = true;
                OnDistanceValidated();
            }
        }

        public void SetRightHandShape(string handShape)
        {
            if (Enum.TryParse(handShape, true, out HandShape shape))
            {
                _rightHandShape = shape;
                _rightHandActive = true;
            }
        }

        public void SetLeftHandShape(string handShape)
        {
            if (Enum.TryParse(handShape, true, out HandShape shape))
            {
                _leftHandShape = shape;
                _leftHandActive = true;
            }
        }

        public void TurnOffRightHandShape()
        {
            _rightHandActive = false;
            _gestureValidated = false;
        }

        public void TurnOffLeftHandShape()
        {
            _leftHandActive = false;
            _gestureValidated = false;
        }
    }
}

