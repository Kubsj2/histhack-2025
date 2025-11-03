using UnityEngine;

[System.Serializable]
public class BlackboardInt : BlackboardVariable
{
    [SerializeField] private int value;

    public override object GetValue() => value;
    public override void SetValue(object val) => value = (int)val;

    public int Value
    {
        get => value;
        set => this.value = value;
    }
}
