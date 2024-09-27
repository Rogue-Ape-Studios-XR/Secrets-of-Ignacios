using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    [CreateAssetMenu(fileName = "Gesture", menuName = "Scriptable Objects/Gesture")]
    internal class Gesture : ScriptableObject
    {
        [SerializeField] internal string _name;

        // Left/Right hand shape to create the gesture
        [SerializeField] internal HandShape _leftHandShape; 
        [SerializeField] internal HandShape _rightHandShape;

        // Max distance between the hands in x, y, z direction for the gesture to be recognised
        [SerializeField] internal float _xHandDistance; 
        [SerializeField] internal float _yHandDistance;
        [SerializeField] internal float _zHandDistance;
    }
}
