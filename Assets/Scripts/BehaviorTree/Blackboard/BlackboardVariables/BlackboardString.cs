using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Blackboard/String")]
public class BlackboardString : BlackboardVariable
{
    [SerializeField] private string value;

    public override object GetValue() => value;
    public override void SetValue(object val) => value = (string)val;

    public string Value
    {
        get => value;
        set => this.value = value;
    }
}
