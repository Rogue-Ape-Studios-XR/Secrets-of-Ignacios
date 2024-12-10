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
        [SerializeField] private HandVfxManager _handVfxManager;

        [Header("Spells")]
        [SerializeField] private Spell[] _availableSpells;

        private Spell _currentSpell;
        private Spell _lastSpell;
        private CancellationTokenSource _cancellationTokenSource;

        public static event Action onSpellValidation;
        public static event Action onQuickCastValidation;
        public static event Action onNoSpellMatch;

        internal Spell CurrentSpell => _currentSpell;

        private void Awake()
        {
            _serviceLocator.RegisterService(this);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            _sequenceManager.onGestureRecognised += CheckSequence;
            _sequenceManager.onReset += HandleReset;
            _sequenceManager.onQuickCast += HandleOnQuickCast;
            ProgressionManager.OnProgressionEvent += HandleProgressionEvent;
        }

        private void OnDestroy()
        {
            _sequenceManager.onGestureRecognised -= CheckSequence;
            _sequenceManager.onReset -= HandleReset;
            _sequenceManager.onQuickCast -= HandleOnQuickCast;
            ProgressionManager.OnProgressionEvent += HandleProgressionEvent;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void HandleProgressionEvent(ProgressionData data)
        {
            if (data.Type == ProgressionType.SpellUnlock && data.Data is SpellUnlockData spellData)
                UnlockSpell(spellData.Spell);
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

        public void CheckSequence(List<Gesture> performedGestures)
        {
            Spell exactMatch = GetExactMatchingSpell(performedGestures);
            bool hasPartialMatch = HasPartialMatchingSpell(performedGestures);

            if (exactMatch != null && exactMatch._isUnlocked)
            {
                SetSpell(exactMatch);
                onSpellValidation?.Invoke();
            }
            else if (!hasPartialMatch)
            {
                _currentSpell = null;
                _handVfxManager.HandleOnSpellFailed();
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
            _handVfxManager.SetHandEffects(_currentSpell, false);
        }


        internal void HandleReset()
        {
            _currentSpell = null;
            _handVfxManager.ResetHandColors();
        }

        private void HandleOnQuickCast()
        {
            _currentSpell = _lastSpell;
            _handVfxManager.SetHandEffects(_currentSpell, true);
            onQuickCastValidation?.Invoke();
            onSpellValidation?.Invoke();
        }

        private void UnlockSpell(Spell spell)
        {
            if (!spell._isUnlocked)
                spell._isUnlocked = true;
        }
    }
}

