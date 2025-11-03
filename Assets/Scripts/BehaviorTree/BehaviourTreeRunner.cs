using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    protected void Start()
    {
        if (tree == null) Debug.LogError($"Tree is null for {gameObject.name}");
        tree = tree.Clone();
        tree.Bind();
        
    }
    protected void Update()
    {
        if (tree == null) return;
        tree.Update();
    }
}
