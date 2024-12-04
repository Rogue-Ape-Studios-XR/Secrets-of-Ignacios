using RogueApeStudios.SecretsOfIgnacios.Gestures;
using RogueApeStudios.SecretsOfIgnacios.Player.SpellMagicCircle;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using RogueApeStudios.SecretsOfIgnacios.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SequenceManager _sequenceManager;
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private HandVfxManager _vfxManager;
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
            _sequenceManager.onReset += HandleReset;
            _sequenceManager.onQuickCast += HandleOnQuickCast;
            ProgressionManager.OnProgressionEvent += HandleProgressionEvent;
        }

        private void OnDestroy()
        {
            _sequenceManager.OnGestureRecognised -= CheckSequence;
            _sequenceManager.onReset -= HandleReset;
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

        public void CheckSequence(List<Gesture> performedGestures)
        {
            Spell exactMatch = GetExactMatchingSpell(performedGestures);
            bool hasPartialMatch = HasPartialMatchingSpell(performedGestures);

            if (exactMatch != null && exactMatch._isUnlocked)
            {
                SetSpell(exactMatch);
                _vfxManager.HandleCastRecognized(true);
            }
            else if (!hasPartialMatch)
            {
                _currentSpell = null;
                _vfxManager.SpellWrongIndication(_cancellationTokenSource.Token);
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
            _vfxManager.SetHandEffects(true, _currentSpell);
        }


        internal void HandleReset()
        {
            _currentSpell = null;

            _vfxManager.SetHandEffects(false, _currentSpell);

            onSpellValidation?.Invoke(false);
        }

        private void HandleOnQuickCast()
        {
            _currentSpell = _lastSpell;

            _vfxManager.SetHandEffects(true, _currentSpell);

            onSpellValidation?.Invoke(true);
        }

        private void UnlockSpell(Spell spell)
        {
            if (!spell._isUnlocked)
                spell._isUnlocked = true;
        }
    }
}

