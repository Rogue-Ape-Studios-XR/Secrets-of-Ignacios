using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    public class CastScriptTester : MonoBehaviour
    {
        [SerializeField] private Cast _cast;
        [SerializeField] private bool _fire;
        [SerializeField] private GameObject _spell;
        [SerializeField] private float _fireRate = 0.2f;

        private CancellationTokenSource _cancellationTokenSource;

        private void OnEnable()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public async void Go()
        {
            string _spellName = _spell.name;
            while (_fire)
            {
                _cast.CastNoHands(transform, _spellName, _spell);
                await UniTask.WaitForSeconds(_fireRate, cancellationToken: _cancellationTokenSource.Token);
            }
        }
    }
}
