using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEditor;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios
{
    [CustomEditor(typeof(CastScriptTester))]
    public class CastScriptTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CastScriptTester myComponent = (CastScriptTester)target;

            if (GUILayout.Button("Call GO"))
                myComponent.Go();
        }
    }
}
