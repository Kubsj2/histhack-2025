using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class Blackboard : ScriptableObject
{
    [SerializeReference] public List<BlackboardVariable> variables = new List<BlackboardVariable>();

    public T GetValue<T>(string key)
    {
        var variable = variables.Find(v => v.key == key);
        if (variable != null)
        {
            return (T)variable.GetValue();
        }
        else
        {
            Debug.LogError($"Key {key} not found in blackboard when trying to get value");
            return default;
        }
    }

    public void SetValue<T>(string key, T value)
    {
        var variable = variables.Find(v => v.key == key);
        if (variable != null)
        {
            variable.SetValue(value);
        }
        else
        {
            Debug.LogError($"Key {key} not found in blackboard when trying to set value {value}");
        }
    }

    public Blackboard Clone()
    {
        Blackboard b = Instantiate(this);
        for (int i = 0; i < variables.Count; i++)
        {
            b.variables[i] = b.variables[i].Clone();
        }
        return b;
    }
}
