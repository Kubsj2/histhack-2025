using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PickDialogueOptionNode : CompositeNode
{
    private bool _hasButtonBeenPressed = false;
    private int _buttonIndex = -1;
    private bool _subNodeStarted = false;

    
    public string button1Text;
    public string button2Text;
    public string button3Text;

    public override string tooltip { get => "This node replaces the text field with n buttons, where n is the number of this node's children, and executes the specified child based on the chosen option. The chosen button corresponds to the child, where 1 is the leftmost child and 3 is the rightmost."; }
    public override Category category => Category.DialogueNodes;
    protected override void OnStart()
    {
        _hasButtonBeenPressed = false;
        _buttonIndex = -1;
        _subNodeStarted = false;

        DialogueManager.Instance.dialogueText.SetActive(false);
        DialogueManager.Instance.dialogueTitle.SetActive(false);
        if (children.Count > 0)
        {
            
            DialogueManager.Instance.dialogueButton1.SetActive(true);
            DialogueManager.Instance.dialogueButton1.GetComponent<Button>().onClick.AddListener(Button1Clicked);
            DialogueManager.Instance.dialogueButton1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = button1Text;
        }
        if (children.Count > 1)
        {
            DialogueManager.Instance.dialogueButton2.SetActive(true);
            DialogueManager.Instance.dialogueButton2.GetComponent<Button>().onClick.AddListener(Button2Clicked);
            DialogueManager.Instance.dialogueButton2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = button2Text;
        }
        if (children.Count > 2)
        {
            DialogueManager.Instance.dialogueButton3.SetActive(true);
            DialogueManager.Instance.dialogueButton3.GetComponent<Button>().onClick.AddListener(Button3Clicked);
            DialogueManager.Instance.dialogueButton3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = button3Text;
        }
    }

    protected override void OnStop()
    {
        DialogueManager.Instance.dialogueButton1.SetActive(false);
        DialogueManager.Instance.dialogueButton2.SetActive(false);
        DialogueManager.Instance.dialogueButton3.SetActive(false);
        DialogueManager.Instance.dialogueText.SetActive(true);
        DialogueManager.Instance.dialogueTitle.SetActive(true);
        
        DialogueManager.Instance.dialogueButton1.GetComponent<Button>().onClick.RemoveListener(Button1Clicked);
        DialogueManager.Instance.dialogueButton2.GetComponent<Button>().onClick.RemoveListener(Button2Clicked);
        DialogueManager.Instance.dialogueButton3.GetComponent<Button>().onClick.RemoveListener(Button3Clicked);
    }

    protected override State OnUpdate()
    {
        if (children.Count < 1 || children.Count > 3)
        {
            Debug.LogError("DialogueOptionPicker requires 1 to 3 children.");
            return State.Failure;
        }

        if (!_hasButtonBeenPressed)
        {
            return State.Running;
        }

        var child = children[_buttonIndex];

        if (!_subNodeStarted)
        {
            child.started = false; // force it to run from beginning
            _subNodeStarted = true;
        }

        var result = child.Update();

        if (result == State.Running)
        {
            return State.Running;
        }

        return result; // Success or Failure of selected child
    }

    private void Button1Clicked()
    {
        _buttonIndex = 0;
        _hasButtonBeenPressed = true;
    }

    private void Button2Clicked()
    {
        _buttonIndex = 1;
        _hasButtonBeenPressed = true;
    }

    private void Button3Clicked()
    {
        _buttonIndex = 2;
        _hasButtonBeenPressed = true;
    }
}