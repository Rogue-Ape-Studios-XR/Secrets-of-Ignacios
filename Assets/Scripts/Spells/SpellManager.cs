using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [SerializeField] private SequenceManager _gestureManager;
        [SerializeField] private Material _rightHandMaterial;
        [SerializeField] private Material _leftHandMaterial;
        [SerializeField] private Spell[] _availableSpells;

        private Spell _currentSpell;
        private CancellationTokenSource _cancellationTokenSource;
        private Color defaultColor;
        private bool _canCast = false;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            defaultColor = _rightHandMaterial.GetColor("_MainColor");
        }

        private void OnEnable()
        {
            _gestureManager.OnSequenceCreated += ValidateSequence;
            _gestureManager.OnReset += HandleReset;
        }

        private void OnDestroy()
        {
            _gestureManager.OnSequenceCreated -= ValidateSequence;
            _gestureManager.OnReset -= HandleReset;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void SetSpell(Spell spell)
        {
            _currentSpell = spell;
        }

        public void ValidateSequence()
        {
            foreach (var spell in _availableSpells)
                if (spell._gestureSequence.Count == _gestureManager.ValidatedGestures.Count &&
                    spell._gestureSequence.SequenceEqual(_gestureManager.ValidatedGestures))
                {
                    SetSpell(spell);
                    Debug.Log("HEEEEEEEEEEEEEEEEEEY" + spell.name);
                }
                else
                {
                    _currentSpell = null;
                    _canCast = false;
                    SpellWrongIndication(_cancellationTokenSource.Token);

                }
        }

        private async void SpellWrongIndication(CancellationToken token)
        {
            try
            {
                float delay = 0.5f;
                int loopAmount = 2;

                for (int i = 0; i < loopAmount; i++)
                {
                    await UniTask.WaitForSeconds(delay, cancellationToken: token);

                    _rightHandMaterial.SetColor("_MainColor", Color.red);
                    _leftHandMaterial.SetColor("_MainColor", Color.red);
                    print("wrong");
                    await UniTask.WaitForSeconds(delay, cancellationToken: token);

                    _rightHandMaterial.SetColor("_MainColor", defaultColor);
                    _leftHandMaterial.SetColor("_MainColor", defaultColor);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("SpellWrongIndication was canceled");
            }
        }

        internal void HandleReset()
        {
            _currentSpell = null;
            _canCast = false;
        }
    }
}
