using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Building))]
    public class BuildingEditorWindow : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
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
