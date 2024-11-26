using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Puzzle;
using System.Threading;
using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Water
{
    internal class Washable : WaterInteractable 
    {
        [SerializeField] private Renderer _noise; //when used: _noise.material.[name of shader graph]
        [SerializeField] internal bool _cleaned;

        [SerializeField] private float _grimeMax;
        [SerializeField] private float _grimeAmountRemoved;

        private async void Wash(CancellationToken token)
        {
            try
            {
                for (int i = 0; i < _grimeMax && !_cleaned && _isSplashed; i++)
                {
                    //something with _noise
                    _grimeAmountRemoved++;
                    await UniTask.WaitForSeconds(_spellInterval, cancellationToken: token);
                    _isSplashed = false;
                }
                if (_grimeAmountRemoved >= _grimeMax)
                {
                    _cleaned = true;
                    gameObject.SetActive(false);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Cleaning was Canceled");
            }
        }

        internal override void Splashed()
        {
            _isSplashed = true;
            Wash(_cancellationTokenSource.Token);
            Debug.Log("This is now being cleaned");
        }
    }
}
