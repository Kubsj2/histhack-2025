using BehaviorTree.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

    private Editor editor;
    public InspectorView()
    {
        
    }

    internal void UpdateSelection(NodeView view)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(view.node);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (editor.target) editor.OnInspectorGUI();
        });
        Add(container);
        
    }
}
