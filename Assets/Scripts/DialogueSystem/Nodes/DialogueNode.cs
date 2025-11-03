using TMPro;
using UnityEngine;

public class DialogueNode : ActionNode
{
    [TextArea] public string dialogueSubject;
    [TextArea] public string dialogueText;
    public override string tooltip { get => "The Dialogue Node sets the values of the Dialogue window to display text and the subject"; }
    public override Category category => Category.DialogueNodes;

    protected override void OnStart()
    {
        DialogueManager.Instance.dialogueTitle.SetActive(true);
        DialogueManager.Instance.dialogueText.SetActive(true);
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        DialogueManager.Instance.dialogueTitle.GetComponent<TextMeshProUGUI>().text  = dialogueText;
        DialogueManager.Instance.dialogueTitle.GetComponent<TextMeshProUGUI>().text = dialogueSubject;
        return State.Success;
    }
}
