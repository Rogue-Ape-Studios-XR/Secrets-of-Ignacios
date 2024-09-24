using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Windows;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class GestureManager : MonoBehaviour
    {
        //[SerializeField] private Material _leftHandMaterial;
        //[SerializeField] private Material _rightHandMaterial;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private List<Gesture> _allGestures;

        private List<Gesture> _validatedGestures = new();
        private Gesture _gesture;
        private HandShape _leftHandShape;
        private HandShape _rightHandShape;
        private bool _leftHandActive = false;
        private bool _rightHandActive = false;
        private bool _gestureValidated = false;
        private CancellationTokenSource _cancellationTokenSource;

        public List<Gesture> ValidatedGestures => _validatedGestures;

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
            if (_leftHandActive && _rightHandActive && !_gestureValidated && _gesture != null)
                HandDistanceCheck(_gesture);
            if (_leftHandActive && _rightHand && _gestureValidated)
            {
                _gestureValidated = false;

                if (_validatedGestures.Count > 0 && _validatedGestures[^1] != _gesture)
                    _validatedGestures.Add(_gesture);
                else if (_validatedGestures.Count == 0)
                    _validatedGestures.Add(_gesture);

                _gesture = null;
            }
        }

        internal void GestureRecognised()
        {
            SetHandColor(_cancellationTokenSource.Token);
        }

        private async void SetHandColor(CancellationToken token)
        {
            try
            {
                //Color originalColor = _leftHandMaterial.color;
                //_leftHandMaterial.color = Color.green;
                //_rightHandMaterial.color = Color.green;
                await UniTask.WaitForSeconds(1, cancellationToken: token);
                //_leftHandMaterial.color = originalColor;
                //_rightHandMaterial.color = originalColor;
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("SetHandColor was canceled");
            }
        }

        public void CheckGestures()
        {
            if (_leftHandActive && _rightHandActive && _gesture == null)
                foreach (var gesture in _allGestures)
                    if (_leftHandShape == gesture._leftHandGesture && _rightHandShape == gesture._rightHandGesture)
                        _gesture = gesture;
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

        private void HandDistanceCheck(Gesture gesture)
        {
            float distanceX = Mathf.Abs(_leftHand.position.x - _rightHand.position.x);
            float distanceY = Mathf.Abs(_leftHand.position.y - _rightHand.position.y);
            float distanceZ = Mathf.Abs(_leftHand.position.z - _rightHand.position.z);

            if (gesture._xHandDistance > distanceX &&
                gesture._yHandDistance > distanceY &&
                gesture._zHandDistance > distanceZ)
            {
                _gestureValidated = true;
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
