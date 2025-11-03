using UnityEngine;

public class RepeatNode : DecoratorNode
{
    public int repeats;
    private int _counter;
    public override string tooltip { get => "Makes the child node work again (repeats) times"; }
    public override Category category => Category.GeneralNodes;
    protected override void OnStart()
    {
        _counter = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        
        child.Update();
        if (_counter > repeats ) return State.Success;
        return State.Running;
    }
}
