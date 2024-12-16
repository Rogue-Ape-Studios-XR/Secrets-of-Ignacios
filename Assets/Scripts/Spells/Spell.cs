using RogueApeStudios.SecretsOfIgnacios.Gestures;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    [Serializable]
    public struct HandConfig
    {
        [SerializeField] internal ElementType _elementType;
        [SerializeField] internal CastTypes _castType;
        [SerializeField] internal Vector3 _position;
        [SerializeField] internal VisualEffect _handEffect;
        [SerializeField] internal GameObject _spellPrefab;
        [SerializeField] internal Material _handMaterial;
        [SerializeField] internal Color _handColor;
        [SerializeField] internal int _poolSize;
        [SerializeField] internal VisualEffect _chargeEffect;
        [SerializeField] internal AudioClip _audioClip;
    }

    [CreateAssetMenu(fileName = "Spell", menuName = "Scriptable Objects/Spell")]
    public class Spell : ScriptableObject
    {
        [SerializeField] internal string _name;
        [SerializeField] internal List<Gesture> _gestureSequence;
        [SerializeField] internal bool _duoSpell;
        [SerializeField] internal HandConfig _primaryConfig;
        [SerializeField] internal HandConfig _secondaryConfig;
        [SerializeField] internal bool _isUnlocked;
    }
}
