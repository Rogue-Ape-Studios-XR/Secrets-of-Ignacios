using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Water
{
    internal class Fillable : WaterInteractable
    {
        [SerializeField] private bool _filled;

        [SerializeField] private float _contentMax;
        private float _contentFilled;

        private async void Fill(CancellationToken token)
        {
            try
            {
                for (int i = 0; i < _contentMax && !_filled && _isSplashed; i++)
                {
                    _contentFilled++;
                    await UniTask.WaitForSeconds(_spellInterval, cancellationToken: token);
                    _isSplashed = false;
                    Debug.Log(_contentFilled);
                }
                if (_contentFilled >= _contentMax)
                {
                    _filled = true;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Cleaning was Canceled");
            }
        }

        internal override void Splashed()
        {
            _isSplashed= true;
            Fill(_cancellationTokenSource.Token);
            Debug.Log("This is now being filled");
        }
    }
}
