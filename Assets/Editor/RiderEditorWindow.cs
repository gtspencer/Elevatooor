using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RiderV2))]
    public class RiderEditorWindow : UnityEditor.Editor
    {
        private SerializedProperty riderState;
        private SerializedProperty riderSpeed;
        
        private void OnEnable()
        {
            riderState = serializedObject.FindProperty("riderState");
            riderSpeed = serializedObject.FindProperty("riderSpeed");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(riderState);
            EditorGUILayout.PropertyField(riderSpeed);
            
            serializedObject.ApplyModifiedProperties();
            
            var rider = target as RiderV2;
            if (GUILayout.Button("Init Ride"))
            {
                rider.GetRandomElevator();
            }
        }
    }
}
