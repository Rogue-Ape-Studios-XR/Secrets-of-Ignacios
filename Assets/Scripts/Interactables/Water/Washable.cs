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
        [SerializeField] private bool _cleaned;

        [SerializeField] private float _grimeMax;
        private float _grimeAmountRemoved;

        private async void Wash(CancellationToken token)
        {
            try
            {
                for (int i = 0; i < _grimeMax && _cleaned == false; i++)
                {
                    //something with _noise
                    _grimeAmountRemoved++;
                    await UniTask.WaitForSeconds(1, cancellationToken: token);
                    Debug.Log(_grimeAmountRemoved);
                }
                if (_grimeAmountRemoved >= _grimeMax)
                {
                    _cleaned = true;
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
