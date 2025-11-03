using UnityEngine;

public class RootNode : Node
{
    public Node child;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        return child.Update();
    }

    public override string tooltip { get => "This is the Root of Your tree"; }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}
