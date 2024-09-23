using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    [CreateAssetMenu(fileName = "Spell", menuName = "Scriptable Objects/Spell")]
    internal class Spell : ScriptableObject
    {
        [SerializeField] internal string _name;
        [SerializeField] internal ElementType _elementType;
        [SerializeField] internal Gesture[] _gestureSequence;
        [SerializeField] internal Color _handColor;
        [SerializeField] internal GameObject _handEffect;
        [SerializeField] internal GameObject _castEffect;
    }
}
