using UnityEngine;

[System.Serializable]
public class BlackboardBool : BlackboardVariable
{
    [SerializeField] private bool value;

    public override object GetValue() => value;
    public override void SetValue(object val) => value = (bool)val;

    public bool Value
    {
        get => value;
        set => this.value = value;
    }
}
