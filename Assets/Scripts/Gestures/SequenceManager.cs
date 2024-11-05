using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace RogueApeStudios.SecretsOfIgnacios.Gestures
{
    internal class SequenceManager : MonoBehaviour
    {
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Renderer _rightHandMaterial;
        [SerializeField] private Renderer _leftHandMaterial;
        [SerializeField] private List<Gesture> _allGestures;
        [SerializeField] private bool _leftHandActive = false; // Only serialized for testing purposes
        [SerializeField] private bool _rightHandActive = false; // Only serialized for testing purposes

        private readonly List<Gesture> _validatedGestures = new();
        private CancellationTokenSource _cancellationTokenSource;
        private Gesture _currentGesture;
        private Color _defaultColor;
        private HandShape _leftHandShape;
        private HandShape _rightHandShape;
        private bool _gestureValidated = false;
        private bool _sequenceStarted = false;
        [SerializeField] private bool _canQuickCast = false;

        internal event Action OnSequenceCreated;
        internal event Action OnReset;
        internal event Action OnQuickCast;
        internal event Action<Gesture> OnGestureRecognised;
        internal event Action<Gesture> OnElementValidated; 

        internal List<Gesture> ValidatedGestures => _validatedGestures;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            SpellManager.OnSpellValidation += HandleOnSpellValidated;
            _defaultColor = _rightHandMaterial.materials[1].GetColor("_MainColor");
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
                if (_currentGesture._name == "Start" && _leftHandActive && _rightHandActive &&
                    _currentGesture._leftHandShape == HandShape.Start && _currentGesture._rightHandShape == HandShape.Start)
                {
                    _validatedGestures.Clear();
                    OnReset?.Invoke();
                    _sequenceStarted = true;
                    _canQuickCast = false;
                    
                    OnElementValidated?.Invoke(_currentGesture);
                }
                else if (_canQuickCast && _currentGesture._name == "Quick Cast" && _leftHandActive &&
                    _rightHandActive && _currentGesture._leftHandShape == HandShape.QuickCast &&
                    _currentGesture._rightHandShape == HandShape.QuickCast)
                {
                    _validatedGestures.Clear();
                    OnQuickCast?.Invoke();
                    
                    OnElementValidated?.Invoke(_currentGesture);
                }

                if (_sequenceStarted && _validatedGestures.Count == 0 ||
                    _sequenceStarted && _validatedGestures[^1] != _currentGesture &&
                    _rightHandShape != HandShape.QuickCast && _leftHandShape != HandShape.QuickCast)
                {
                    _validatedGestures.Add(_currentGesture);
                    ChangeColor(_cancellationTokenSource.Token);
                    OnGestureRecognised?.Invoke(_currentGesture);
                }

                if (_validatedGestures.Count == 2)
                {
                    OnElementValidated?.Invoke(_currentGesture);
                }
                
                if (_validatedGestures.Count == 3)
                {
                    OnSequenceCreated?.Invoke();
                    _sequenceStarted = false;
                    _validatedGestures.Clear();
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

        private async void ChangeColor(CancellationToken token)
        {
            try
            {
                if (_validatedGestures.Count < 3 && _validatedGestures.Count > 0)
                {
                    float delay = 1;

                    _rightHandMaterial.materials[1].SetColor("_MainColor", _currentGesture._color);
                    _leftHandMaterial.materials[1].SetColor("_MainColor", _currentGesture._color);

                    await UniTask.WaitForSeconds(delay, cancellationToken: token);

                    _rightHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                    _leftHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                }
                else
                {
                    _rightHandMaterial.materials[1].SetColor("_MainColor", _currentGesture._color);
                    _leftHandMaterial.materials[1].SetColor("_MainColor", _currentGesture._color);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("ChangeColor was canceled");
            }
        }

        private void HandleOnSpellValidated(bool value)
        {
            _canQuickCast = value;
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

