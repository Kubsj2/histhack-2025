
using UnityEngine;

public class DebugLogNode : ActionNode
{
    public string message;
    public override string tooltip { get => "The Debug Log Node is used to display the specified text in console."; }
    public override Category category => Category.DebugNodes;
    protected override void OnStart()
    {
        
        Debug.Log($"OnStart: {message}");
    }

    protected override void OnStop()
    {
            Debug.Log($"OnStop: {message}");
    }

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate: {message}");
        return State.Success;
    }
}
