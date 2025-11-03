using UnityEngine;

public class DialogueStartNode : ActionNode
{
    public override string tooltip { get => "The Dialogue Start Node shows the Dialogue Window."; }
    public override Category category => Category.DialogueNodes;
    protected override void OnStart()
    {
        Debug.Log("Start dialogue");
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (DialogueManager.Instance.dialogueContainer == null)
        {
            Debug.LogError("Dialogue container not set for dialogue manager");
            return State.Failure;
        }
        DialogueManager.Instance.dialogueContainer.SetActive(true);
        return State.Success;
    }
}
