using UnityEngine;

public class SetVariableNode : ActionNode
{
    public string key;

    public enum VariableType
    {
        Bool, 
        Int, 
        String
    }
    public VariableType type;

    public bool boolValue;
    public int intValue;
    public string stringValue;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        switch (type)
        {
            case VariableType.Bool:
                blackboard.SetValue(key, boolValue);
                break;
            case VariableType.Int:
                blackboard.SetValue(key, intValue);
                break;
            case VariableType.String:
                blackboard.SetValue(key, stringValue);
                break;
            default:
                Debug.Log("Unknown variable type");
                return State.Failure;
        }
        return State.Success;
    }
}
