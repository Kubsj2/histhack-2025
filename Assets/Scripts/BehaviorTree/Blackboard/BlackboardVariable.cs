using UnityEngine;

[System.Serializable]
public abstract class BlackboardVariable : ScriptableObject
{
    public string key;
    public abstract object GetValue();
    public abstract void SetValue(object value);

    public BlackboardVariable Clone()
    {
        return Instantiate(this);
    }
}
