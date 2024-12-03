using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using System;
using System.Collections.Generic;
using System.Threading;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using RogueApeStudios.SecretsOfIgnacios.Services;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SequenceManager _sequenceManager;
        [SerializeField] private ServiceLocator _serviceLocator;
        [Header("Hand Objects")]
        [SerializeField] private Renderer _rightHandMaterial;
        [SerializeField] private Renderer _leftHandMaterial;

        [Header("Spells")]
        [SerializeField] private Spell[] _availableSpells;

        private Spell _currentSpell;
        private Spell _lastSpell;
        private CancellationTokenSource _cancellationTokenSource;
        private Color _defaultColor;

        public static event Action<bool> onSpellValidation;
        public static event Action onNoSpellMatch;

        internal Spell CurrentSpell => _currentSpell;
        internal Color DefaultColor => _defaultColor;

        private void Awake()
        {
            _serviceLocator.RegisterService(this);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _defaultColor = _rightHandMaterial.materials[1].GetColor("_MainColor");
        }

        private void OnEnable()
        {
            _sequenceManager.OnGestureRecognised += CheckSequence;
            _sequenceManager.OnReset += HandleReset;
            _sequenceManager.OnQuickCast += HandleOnQuickCast;
            ProgressionManager.OnProgressionEvent += HandleProgressionEvent;
        }

        private void OnDestroy()
        {
            _sequenceManager.OnGestureRecognised -= CheckSequence;
            _sequenceManager.OnSequenceCreated -= ValidateSequence;
            _sequenceManager.OnReset -= HandleReset;
            ProgressionManager.OnProgressionEvent += HandleProgressionEvent;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

       private void HandleProgressionEvent(ProgressionData data)
        {
            if (data.Type == ProgressionType.SpellUnlock && data.Data is SpellUnlockData spellData)
            {
                Debug.Log($"Spell '{spellData.Spell.name}' has been unlocked!");
                UnlockSpell(spellData.Spell);
            }
            else
            {
                Debug.LogError("Invalid data received for SpellUnlock event.");
            }
        } 
        
        public bool IsSpellUnlockedForGesture(Gesture gesture)
        {
            foreach (var spell in _availableSpells)
            {
                if (spell._gestureSequence.Contains(gesture))
                {
                    return spell._isUnlocked;
                }
            }
            return false;
        }
        
        public void ValidateSequence()
        {
            bool spellFound = false;

            foreach (var spell in _availableSpells)
            {
                if (!spell._isUnlocked)
                {
                    continue;
                }
                if (spell._gestureSequence.Count == _sequenceManager.ValidatedGestures.Count &&
                    spell._gestureSequence.SequenceEqual(_sequenceManager.ValidatedGestures))
                {
                    SetSpell(spell);
                    spellFound = true;
                    break;
                }
 
            }
        }
        
        public void CheckSequence(List<Gesture> performedGestures)
        {
            Spell exactMatch = GetExactMatchingSpell(performedGestures);
            bool hasPartialMatch = HasPartialMatchingSpell(performedGestures);

            if (exactMatch != null)
            {
                SetSpell(exactMatch);
                onSpellValidation?.Invoke(true);
            }
            else if (!hasPartialMatch)
            {
                _currentSpell = null;
                SpellWrongIndication(_cancellationTokenSource.Token);
                onNoSpellMatch?.Invoke();
            }
            // If there is a partial match, do nothing and wait for more gestures.
        }

        private Spell GetExactMatchingSpell(List<Gesture> performedGestures)
        {
            foreach (Spell spell in _availableSpells)
                if (IsExactMatch(spell, performedGestures))
                    return spell;

            return null;
        }

        private bool HasPartialMatchingSpell(List<Gesture> performedGestures)
        {
            foreach (Spell spell in _availableSpells)
                if (IsPartialMatch(spell, performedGestures))
                    return true;

            return false;
        }

        private bool IsExactMatch(Spell spell, List<Gesture> performedGestures)
        {
            if (performedGestures.Count != spell._gestureSequence.Count)
                return false;

            for (int i = 0; i < spell._gestureSequence.Count; i++)
                if (spell._gestureSequence[i] != performedGestures[i])
                    return false;

            return true;
        }

        private bool IsPartialMatch(Spell spell, List<Gesture> performedGestures)
        {
            if (performedGestures.Count >= spell._gestureSequence.Count)
                return false;

            for (int i = 0; i < performedGestures.Count; i++)
                if (spell._gestureSequence[i] != performedGestures[i])
                    return false;

            return true;
        }

        private void SetSpell(Spell spell)
        {
            _currentSpell = spell;
            _lastSpell = spell;
            SetHandEffects(true);
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

                    SetHandEffects(false);
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

            SetHandEffects(false);

            onSpellValidation?.Invoke(false);
        }

        private void HandleOnQuickCast()
        {
            _currentSpell = _lastSpell;

            SetHandEffects(true);

            onSpellValidation?.Invoke(true);
        }

        private void UnlockSpell(Spell spell)
        {
            if (!spell._isUnlocked)
                spell._isUnlocked = true;
        }
      
        private void SetHandEffects(bool isDefaultColor)
        {
            if (isDefaultColor)
            {
                if (_currentSpell._duoSpell)
                {
                    _rightHandMaterial.material = _currentSpell._primaryConfig._handMaterial;
                    _rightHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._primaryConfig._handColor);
                    _leftHandMaterial.material = _currentSpell._secondaryConfig._handMaterial;
                    _leftHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._secondaryConfig._handColor);
                }
                else
                {
                    _rightHandMaterial.material = _currentSpell._primaryConfig._handMaterial;
                    _rightHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._primaryConfig._handColor);
                    _leftHandMaterial.material = _currentSpell._primaryConfig._handMaterial;
                    _leftHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._primaryConfig._handColor);
                }
            }
            else
            {
                _rightHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                _leftHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
            }
        }
    }
}

