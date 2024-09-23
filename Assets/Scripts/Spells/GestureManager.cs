using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class GestureManager : MonoBehaviour
    {
        //[SerializeField] private Material _leftHandMaterial;
        //[SerializeField] private Material _rightHandMaterial;
        [SerializeField] private List<Gesture> _allGestures;

        private Gesture _gesture;
        private Gesture _leftHandGesture;
        private Gesture _rightHandGesture;
        private bool _leftHandActive = false;
        private bool _rightHandActive = false;
        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
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

        internal void CheckGestures()
        {
            
        }


        public void SetRightHandShape(string handShape)
        {
            foreach (var gesture in _allGestures)
            {
                if (gesture._rightHandGesture.ToString() == handShape)
                {
                    _rightHandActive = true;
                    _rightHandGesture = gesture;
                    continue;
                }
            }
        }
    }
}
