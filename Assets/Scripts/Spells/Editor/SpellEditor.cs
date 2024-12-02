using UnityEditor;

namespace RogueApeStudios.SecretsOfIgnacios.Spells.Editor
{
    [CustomEditor(typeof(Spell))]
    internal class SpellEditor : UnityEditor.Editor
    {
        SerializedProperty _nameProp;
        SerializedProperty _gestureSequenceProp;
        SerializedProperty _primaryConfigProp;
        SerializedProperty _secondaryConfigProp;
        SerializedProperty _duoSpellProp;
        SerializedProperty _isUnlockedProp;

        private void OnEnable()
        {
            _nameProp = serializedObject.FindProperty("_name");
            _isUnlockedProp = serializedObject.FindProperty("_isUnlocked");
            _gestureSequenceProp = serializedObject.FindProperty("_gestureSequence");
            _duoSpellProp = serializedObject.FindProperty("_duoSpell");
            _primaryConfigProp = serializedObject.FindProperty("_primaryConfig");
            _secondaryConfigProp = serializedObject.FindProperty("_secondaryConfig");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nameProp);
            EditorGUILayout.PropertyField(_isUnlockedProp);
            EditorGUILayout.PropertyField(_gestureSequenceProp);
            EditorGUILayout.PropertyField(_duoSpellProp);
            EditorGUILayout.PropertyField(_primaryConfigProp);

            if (_duoSpellProp.boolValue)
                EditorGUILayout.PropertyField(_secondaryConfigProp);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
