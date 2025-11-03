using BehaviorTree.Nodes;
using UnityEditor;
using UnityEngine;

    [CustomEditor(typeof(CompareValueNode))]
    public class CompareValueNodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var descProp = serializedObject.FindProperty("description");
            var keyProp = serializedObject.FindProperty("key");
            var typeProp = serializedObject.FindProperty("type");
            var stringProp = serializedObject.FindProperty("stringValue");
            var intProp = serializedObject.FindProperty("intValue");
            var boolProp = serializedObject.FindProperty("boolValue");
            
            EditorGUILayout.PropertyField(descProp);
            EditorGUILayout.PropertyField(keyProp);
            EditorGUILayout.PropertyField(typeProp);

            var type = (CompareValueNode.VariableType)typeProp.enumValueIndex;
            switch (type)
            {
                case CompareValueNode.VariableType.String:
                    EditorGUILayout.PropertyField(stringProp);
                    break;
                case CompareValueNode.VariableType.Int:
                    EditorGUILayout.PropertyField(intProp);
                    break;
                case CompareValueNode.VariableType.Bool:
                    EditorGUILayout.PropertyField(boolProp);
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
    }
