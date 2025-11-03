using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SetVariableNode))]
public class SetVariableNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var descProp = serializedObject.FindProperty("description");
        var keyProp = serializedObject.FindProperty("key");
        var typeProp = serializedObject.FindProperty("type");
        var boolProp = serializedObject.FindProperty("boolValue");
        var intProp = serializedObject.FindProperty("intValue");
        var stringProp = serializedObject.FindProperty("stringValue");
        EditorGUILayout.PropertyField(descProp);
        EditorGUILayout.PropertyField(keyProp);
        EditorGUILayout.PropertyField(typeProp);

        var type = (SetVariableNode.VariableType)typeProp.enumValueIndex;
        switch (type)
        {
            case SetVariableNode.VariableType.Bool:
                EditorGUILayout.PropertyField(boolProp);
                break;
            case SetVariableNode.VariableType.Int:
                EditorGUILayout.PropertyField(intProp);
                break;
            case SetVariableNode.VariableType.String:
                EditorGUILayout.PropertyField(stringProp);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
