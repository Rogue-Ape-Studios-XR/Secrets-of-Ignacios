using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SequenceManager _sequenceManager;

        [Header("Hand Objects")]
        [SerializeField] private Renderer _rightHandMaterial;
        [SerializeField] private Renderer _leftHandMaterial;

        [Header("Spells")]
        [SerializeField] private Spell[] _availableSpells;


        private Spell _currentSpell;
        private Spell _lastSpell;
        private CancellationTokenSource _cancellationTokenSource;
        private Color _defaultColor;

        public static event Action<bool> OnSpellValidation;

        internal Spell CurrentSpell => _currentSpell;
        internal Color DefaultColor => _defaultColor;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _defaultColor = _rightHandMaterial.materials[1].GetColor("_MainColor");
        }

        private void OnEnable()
        {
            _sequenceManager.OnSequenceCreated += ValidateSequence;
            _sequenceManager.OnReset += HandleReset;
            _sequenceManager.OnQuickCast += HandleOnQuickCast;
        }

        private void OnDestroy()
        {
            _sequenceManager.OnSequenceCreated -= ValidateSequence;
            _sequenceManager.OnReset -= HandleReset;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public void ValidateSequence()
        {
            bool spellFound = false;

            foreach (var spell in _availableSpells)
                if (spell._gestureSequence.Count == _sequenceManager.ValidatedGestures.Count &&
                    spell._gestureSequence.SequenceEqual(_sequenceManager.ValidatedGestures))
                {
                    SetSpell(spell);
                    spellFound = true;
                    break;
                }

            if (!spellFound)
            {
                _currentSpell = null;
                SpellWrongIndication(_cancellationTokenSource.Token);
            }

            OnSpellValidation?.Invoke(spellFound);
        }

        private void SetSpell(Spell spell)
        {
            _currentSpell = spell;
            _lastSpell = spell;

            _rightHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._handColor);
            _leftHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._handColor);
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

                    _rightHandMaterial.materials[1].SetColor("_MainColor", Color.red);
                    _leftHandMaterial.materials[1].SetColor("_MainColor", Color.red);

                    await UniTask.WaitForSeconds(delay, cancellationToken: token);

                    _rightHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                    _leftHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
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

            _rightHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
            _leftHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);

            OnSpellValidation?.Invoke(false);
        }

        private void HandleOnQuickCast()
        {
            _currentSpell = _lastSpell;

            _rightHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._handColor);
            _leftHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._handColor);

            OnSpellValidation?.Invoke(true);
        }
    }
}

