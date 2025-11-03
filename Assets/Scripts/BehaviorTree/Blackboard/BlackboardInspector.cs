using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackboardInspector : IMGUIContainer
{
    public new class UxmlFactory : UxmlFactory<BlackboardInspector, UxmlTraits> { }
    
    private Editor editor;

    public BlackboardInspector()
    {
        
    }
}
