using UnityEngine;

public class SequenceNode : CompositeNode
{
    private int _current;
    public override string tooltip { get => "The sequence node executes its subsequent children one by one, left to right, only if the child previous to the current one returned success"; }
    public override Category category => Category.GeneralNodes;
    protected override void OnStart()
    {
        _current = 0;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        var child = children[_current];
        switch (child.Update())
        {
            case State.Running: { return State.Running; }
            case State.Failure: { return State.Failure; }
            case State.Success:
            {
                _current++;
                break;
            }
        }
        return _current >= children.Count ? State.Success : State.Running;
    }
}
