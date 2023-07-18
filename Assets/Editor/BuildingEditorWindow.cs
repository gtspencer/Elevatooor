using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Building))]
    public class BuildingEditorWindow : UnityEditor.Editor
    {
        private SerializedProperty rightWallPrefab;

        private void OnEnable()
        {
            rightWallPrefab = serializedObject.FindProperty("RightWallPrefab");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(rightWallPrefab, true);

            serializedObject.ApplyModifiedProperties();
            
            var building = target as Building;
            if (GUILayout.Button("Add unit"))
            {
                building.AddUnit();
            }
            
            if (GUILayout.Button("Add floor"))
            {
                building.AddFloor();
            }
        }
    }
}
