using RogueApeStudios.SecretsOfIgnacios.Player.SpellMagicCircle;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Gestures
{
    internal class SequenceManager : MonoBehaviour
    {
        [SerializeField] private HandVfxManager _handVfxManager;

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
        private bool _canQuickCast = false;
        private bool _finalSpellAvailable;
        private bool _finalSpellCast = false;

        internal event Action onSequenceCreated;
        internal event Action onReset;
        internal event Action onQuickCast;
        internal event Action<List<Gesture>> onGestureRecognised;
        internal event Action<List<Gesture>> onFinalGestureRecognised;
        internal event Action<Gesture> onElementValidated;
        internal event Action onSpellFailedVFX;

        public static event Action onFinalSpellValidation;
        //ignacios model will subscribe to this
        public static event Action onFinalSpellCompleted;
        internal List<Gesture> ValidatedGestures => _validatedGestures;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            SpellManager.onSpellValidation += HandleOnSpellValidated;
            SpellManager.onNoSpellMatch += HandleOnSpellFailed;
            FinalSpellTrigger.onFinalSpellUnlockStateChange += HandleFinalSpellUnlockState;
        }

        private void OnDestroy()
        {
            SpellManager.onSpellValidation -= HandleOnSpellValidated;
            FinalSpellTrigger.onFinalSpellUnlockStateChange -= HandleFinalSpellUnlockState;
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
            if (_currentGesture == null) return;

            HandleGesture(_currentGesture);

            if (_sequenceStarted)
            {
                if (_finalSpellAvailable && !_finalSpellCast &&
                    (_validatedGestures.Count == 0 || _validatedGestures[^1] != _currentGesture))
                {
                    if (_currentGesture.name == "Cancel")
                    {
                        //Not pretty but otherwise for some reason it overrides the magic circles due to order I can't really change
                        _currentGesture = null;
                        _gestureValidated = false;
                        return;
                    }
                    Debug.LogWarning(_validatedGestures.Count);
                    _validatedGestures.Add(_currentGesture);
                    //won't let me do touch
                    onFinalGestureRecognised?.Invoke(_validatedGestures);
                    _handVfxManager.ChangeColorOnGesture(_currentGesture);
                    if (_validatedGestures.Count == 2)
                    {
                        _handVfxManager.HandleElementRecognized(_currentGesture);
                    }
                    onFinalSpellValidation?.Invoke();
                }
                else
                {
                    if (_validatedGestures.Count == 0 ||
                        _validatedGestures[^1] != _currentGesture)
                    {
                        _validatedGestures.Add(_currentGesture);
                        _handVfxManager.ChangeColorOnGesture(_currentGesture);

                        if (_currentGesture._name != "Quick Cast")
                            onGestureRecognised?.Invoke(_validatedGestures);

                        if (_validatedGestures.Count == 2)
                            _handVfxManager.HandleElementRecognized(_currentGesture);
                    }
                }

            }

            _currentGesture = null;
            _gestureValidated = false;
        }
        
        private void HandleFinalSpellUnlockState(bool available)
        {
            _finalSpellAvailable = available;
            print("final spell is now " + _finalSpellAvailable);
        }
        
        private void HandleGesture(Gesture currentGesture)
        {
            if (_leftHandActive && _rightHandActive)
                switch (currentGesture._name)
                {
                    case "Start":
                        _sequenceStarted = true;
                        _canQuickCast = false;
                        _handVfxManager.HandleElementRecognized(_currentGesture);
                        break;
                    case "Quick Cast":
                        if (_canQuickCast)
                        {
                            _validatedGestures.Clear();
                            onQuickCast?.Invoke();
                        }
                        break;
                    case "Cancel":
                        if (_validatedGestures.Count > 0)
                        {
                            _validatedGestures.Clear();
                            onReset?.Invoke();
                            _canQuickCast = false;
                            _handVfxManager.HandleElementRecognized(_currentGesture);
                        }
                        break;
                    default:
                        return;
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

        private void HandleOnSpellValidated()
        {
            if (_finalSpellAvailable)
            {
                _canQuickCast = false;
                _validatedGestures.Clear();
                _sequenceStarted = false;
                Debug.LogWarning("triggered final spell validation");
                if (!_finalSpellCast)
                {
                    // Ignacios
                    
                    onFinalSpellCompleted?.Invoke();
                    _finalSpellCast = true;
                    return;
                }
            }
            Debug.LogWarning("spell validation");
            _canQuickCast = true;
            _validatedGestures.Clear();
            _sequenceStarted = false;
        }

        private void HandleOnSpellFailed()
        {
            _validatedGestures.Clear();
            _sequenceStarted = false;
            onSpellFailedVFX?.Invoke(); // Invoke the event to disable VFX on hands
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

