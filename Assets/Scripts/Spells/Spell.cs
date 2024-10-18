using RogueApeStudios.SecretsOfIgnacios.Gestures;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    [CreateAssetMenu(fileName = "Spell", menuName = "Scriptable Objects/Spell")]
    internal class Spell : ScriptableObject
    {
        [SerializeField] internal string _name;
        [SerializeField] internal ElementType _elementType;
        [SerializeField] internal List<Gesture> _gestureSequence;
        [SerializeField] internal Color _handColor;
        [SerializeField] internal VisualEffect _handEffect;
        [SerializeField] internal GameObject _spellPrefab;
        [SerializeField] internal CastTypes _castType;
    }
}
