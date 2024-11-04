namespace RogueApeStudios.SecretsOfIgnacios.Spells.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(Spell))]
    internal class SpellEditor : Editor
    {
        SerializedProperty _nameProp;
        SerializedProperty _gestureSequenceProp;
        SerializedProperty _primaryConfigProp;
        SerializedProperty _secondaryConfigProp;
        SerializedProperty _duoSpellProp;

        private void OnEnable()
        {
            _nameProp = serializedObject.FindProperty("_name");
            _gestureSequenceProp = serializedObject.FindProperty("_gestureSequence");
            _duoSpellProp = serializedObject.FindProperty("_duoSpell");
            _primaryConfigProp = serializedObject.FindProperty("_primaryConfig");
            _secondaryConfigProp = serializedObject.FindProperty("_secondaryConfig");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nameProp);
            EditorGUILayout.PropertyField(_gestureSequenceProp);
            EditorGUILayout.PropertyField(_duoSpellProp);
            EditorGUILayout.PropertyField(_primaryConfigProp);

            if (_duoSpellProp.boolValue)
                EditorGUILayout.PropertyField(_secondaryConfigProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
