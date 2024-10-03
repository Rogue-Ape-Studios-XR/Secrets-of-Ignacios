using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Water
{
    internal abstract class Fillable : WaterInteractable
    {
        [SerializeField] private bool _filled;

        private float _contentMax;
        private float _contentFilled;

        private async void Fill(CancellationToken token)
        {
            try
            {
                for (int i = 0; i < _contentMax && _filled == false; i++)
                {
                    _contentFilled++;
                    await UniTask.WaitForSeconds(1, cancellationToken: token);
                    //after tasks
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
