using UnityEditor;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells.Editor
{
    [CustomPropertyDrawer(typeof(HandConfig))]
    public class HandConfigEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Determine header based on property name
            string header = property.name == "_primaryConfig" ? "Primary Config" : "Secondary Config";

            // Define padding, line height, and layout properties
            float padding = 4f;
            float lineHeight = EditorGUIUtility.singleLineHeight + padding;
            float labelWidth = 100f;  // Label width

            // Draw header
            position.y += padding;
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, lineHeight), header, EditorStyles.boldLabel);

            // Adjust position for fields below header
            position.y += lineHeight;

            // Set label width for each field
            EditorGUIUtility.labelWidth = labelWidth;

            CastTypes castType = (CastTypes)property.FindPropertyRelative("_castType").enumValueIndex;
            Rect positionRect = new();
            Rect elementTypeRect;
            Rect castTypeRect;
            Rect handEffectRect;
            Rect spellPrefabRect;
            Rect handMaterialRect;
            Rect handColorRect;
            Rect poolSizeRect;
            Rect chargeEffectRect;
            Rect audioClipRect;

            if (castType != CastTypes.Touch)
            {
                // Draw fields with more space for value inputs
                elementTypeRect = new Rect(position.x, position.y, position.width, lineHeight);
                castTypeRect = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);
                handEffectRect = new Rect(position.x, position.y + 2 * lineHeight, position.width, lineHeight);
                spellPrefabRect = new Rect(position.x, position.y + 3 * lineHeight, position.width, lineHeight);
                handMaterialRect = new Rect(position.x, position.y + 4 * lineHeight, position.width, lineHeight);
                handColorRect = new Rect(position.x, position.y + 5 * lineHeight, position.width, lineHeight);
                poolSizeRect = new Rect(position.x, position.y + 6 * lineHeight, position.width, lineHeight);
                chargeEffectRect = new Rect(position.x, position.y + 7 * lineHeight, position.width, lineHeight);
                audioClipRect = new Rect(position.x, position.y + 8 * lineHeight, position.width, lineHeight);
            }
            else
            {
                // Draw fields with more space for value inputs
                elementTypeRect = new Rect(position.x, position.y, position.width, lineHeight);
                castTypeRect = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);
                positionRect = new Rect(position.x, position.y + 2 * lineHeight, position.width, lineHeight);
                handEffectRect = new Rect(position.x, position.y + 3 * lineHeight, position.width, lineHeight);
                spellPrefabRect = new Rect(position.x, position.y + 4 * lineHeight, position.width, lineHeight);
                handMaterialRect = new Rect(position.x, position.y + 5 * lineHeight, position.width, lineHeight);
                handColorRect = new Rect(position.x, position.y + 6 * lineHeight, position.width, lineHeight);
                poolSizeRect = new Rect(position.x, position.y + 7 * lineHeight, position.width, lineHeight);
                chargeEffectRect = new Rect(position.x, position.y + 8 * lineHeight, position.width, lineHeight);
                audioClipRect = new Rect(position.x, position.y + 9 * lineHeight, position.width, lineHeight);
            }

            // Draw each field
            EditorGUI.PropertyField(elementTypeRect, property.FindPropertyRelative("_elementType"));
            EditorGUI.PropertyField(castTypeRect, property.FindPropertyRelative("_castType"));

            // Show _position only if _castType meets the condition

            if (castType == CastTypes.Touch) // Replace with actual type condition
            {
                EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("_position"));
            }

            EditorGUI.PropertyField(handEffectRect, property.FindPropertyRelative("_handEffect"));
            EditorGUI.PropertyField(spellPrefabRect, property.FindPropertyRelative("_spellPrefab"));
            EditorGUI.PropertyField(handMaterialRect, property.FindPropertyRelative("_handMaterial"));
            EditorGUI.PropertyField(handColorRect, property.FindPropertyRelative("_handColor"));
            EditorGUI.PropertyField(poolSizeRect, property.FindPropertyRelative("_poolSize"));
            EditorGUI.PropertyField(chargeEffectRect, property.FindPropertyRelative("_chargeEffect"));
            EditorGUI.PropertyField(audioClipRect, property.FindPropertyRelative("_audioClip"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Adjust height based on whether _position is shown
            CastTypes castType = (CastTypes)property.FindPropertyRelative("_castType").enumValueIndex;
            int extraHeight = (castType == CastTypes.Touch) ? 11 : 10; // One extra line for the header
            return (EditorGUIUtility.singleLineHeight + 4f) * extraHeight;
        }
    }
}
