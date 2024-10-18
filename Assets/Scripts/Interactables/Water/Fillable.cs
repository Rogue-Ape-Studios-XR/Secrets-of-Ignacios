using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Water
{
    internal class Fillable : WaterInteractable
    {
        [SerializeField] internal bool _filled;

        [SerializeField] private float _contentMax;
        [SerializeField] private float _contentFilled;

        internal event Action<bool> onFilled;

        private async void Fill(CancellationToken token)
        {
            try
            {
                for (int i = 0; i < _contentMax && !_filled && _isSplashed; i++)
                {
                    _contentFilled++;
                    await UniTask.WaitForSeconds(_spellInterval, cancellationToken: token);
                    _isSplashed = false;
                    //Debug.Log(_contentFilled);
                }
                if (_contentFilled >= _contentMax)
                {
                    _filled = true;
                    onFilled?.Invoke(_filled);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Fill was Canceled");
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
