using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RiderManager))]
    public class RiderManagerEditorWindow : UnityEditor.Editor
    {
        private SerializedProperty riderPrefabProp;

        private void OnEnable()
        {
            riderPrefabProp = serializedObject.FindProperty("riderSpawnZone");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(riderPrefabProp, true);

            serializedObject.ApplyModifiedProperties();
            
            var rider = target as RiderManager;
            if (GUILayout.Button("Send Rider"))
            {
                rider.SendNewRider();
            }
        }
    }
}
