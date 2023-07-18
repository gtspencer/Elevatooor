using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RiderV2))]
    public class RiderEditorWindow : UnityEditor.Editor
    {
        private SerializedProperty riderState;
        private SerializedProperty riderSpeed;
        private SerializedProperty currentFloor;
        private SerializedProperty destinationFloor;
        
        private void OnEnable()
        {
            riderState = serializedObject.FindProperty("riderState");
            riderSpeed = serializedObject.FindProperty("riderSpeed");
            currentFloor = serializedObject.FindProperty("currentFloor");
            destinationFloor = serializedObject.FindProperty("destinationFloor");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(riderState);
            EditorGUILayout.PropertyField(riderSpeed);
            EditorGUILayout.PropertyField(currentFloor);
            EditorGUILayout.PropertyField(destinationFloor);
            
            serializedObject.ApplyModifiedProperties();
            
            var rider = target as RiderV2;
        }
    }
}
