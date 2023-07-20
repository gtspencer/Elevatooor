using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GoldManager))]
    public class GoldManagerEditorWindow : UnityEditor.Editor
    {
        // private SerializedProperty rightWallPrefab;

        private void OnEnable()
        {
            // rightWallPrefab = serializedObject.FindProperty("RightWallPrefab");
        }

        private int goldAmount = 1000;
        public override void OnInspectorGUI()
        {
            /*serializedObject.Update();
            
            EditorGUILayout.PropertyField(rightWallPrefab, true);

            serializedObject.ApplyModifiedProperties();*/

            goldAmount = EditorGUILayout.IntField("Gold", goldAmount);

            var goldManager = target as GoldManager;
            if (GUILayout.Button("Add Gold"))
            {
                goldManager.TestAddGold(goldAmount);
            }
        }
    }
}
