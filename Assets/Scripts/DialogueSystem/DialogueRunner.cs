using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueRunner : BehaviourTreeRunner
{ 
    private DialogueManager _manager;
    
    private TextMeshProUGUI _button1Text;
    private TextMeshProUGUI _button2Text;
    private TextMeshProUGUI _button3Text;
    void Start()
    {
        _manager = DialogueManager.Instance;
        
        _button1Text = _manager.dialogueButton1.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _button2Text = _manager.dialogueButton2.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _button3Text = _manager.dialogueButton3.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        base.Start();
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        UpdateDialogueWindow();
    }

    private void UpdateDialogueWindow()
    {
    }
}
