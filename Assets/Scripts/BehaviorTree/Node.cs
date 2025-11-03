using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum Category
    {
        DialogueNodes,
        QuestNodes,
        DebugNodes,
        GeneralNodes,
        MiscNodes,
    }
    public enum State
    {
        Running,
        Failure,
        Success
    }
    
    [HideInInspector] public string guid;
    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public Blackboard blackboard;
    [HideInInspector] public virtual Category category => Category.MiscNodes;
    public virtual string tooltip => "A node";
    [TextArea] public string description;
    [HideInInspector]public Vector2 position;
    public State Update()
    {
        if (!started)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate();
        
        if (state == State.Failure || state == State.Success)
        {
            OnStop();
            started = false;
        }
        
        return state;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }
    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
    
}
