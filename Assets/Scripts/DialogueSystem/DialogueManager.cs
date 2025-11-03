
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    public GameObject dialogueContainer;
    public GameObject dialogueTitle;
    public GameObject dialogueText;
    [Space]
    public GameObject dialogueButton1;
    public GameObject dialogueButton2;
    public GameObject dialogueButton3;
    void Start()
    {
        
        if (Instance != null)
        {
            Debug.LogError($"There is more than one Dialogue Manager Instance in the scene, {this.gameObject.name} has been destroyed.");
            Destroy(this);
        }
        else
        {
            Instance = this;
            
        }

    }

    public static void ShowDialogueWindow(string dialogueTitle, string dialogueText)
    {
        Instance.dialogueContainer.SetActive(true);
        Instance.dialogueTitle.SetActive(true);
        Instance.dialogueText.SetActive(true);
        
        Instance.dialogueButton1.SetActive(false);
        Instance.dialogueButton2.SetActive(false);
        Instance.dialogueButton3.SetActive(false);
        
        Instance.dialogueTitle.GetComponent<TextMeshProUGUI>().text = dialogueTitle;
        Instance.dialogueText.GetComponent<TextMeshProUGUI>().text = dialogueText;
        
    }
    
}
