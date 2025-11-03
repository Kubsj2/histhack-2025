using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "New Behavior Tree", menuName = "Behavior Tree")]
public class BehaviourTree : ScriptableObject
{
    public Blackboard blackboard;
    public Node root;
    public Node.State treeState = Node.State.Running;
    public List<Node> nodes = new List<Node>();
    public Node.State Update()
    {
        if (root.state == Node.State.Running) return root.Update();
        return treeState;
    }
#if UNITY_EDITOR
    

    public Node CreateNode(System.Type type)
    {
        Node n = ScriptableObject.CreateInstance(type) as Node;
        n.name = type.Name;
        n.guid = GUID.Generate().ToString();
        
        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        nodes.Add(n);
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(n, this);
        }
        Undo.RegisterCreatedObjectUndo(n, "Behaviour Tree (CreateNode)");
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(n);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
        AssetDatabase.SaveAssets();
        return n;
    }

    public void DeleteNode(Node node)
    {
        Undo.RegisterCompleteObjectUndo(this, "Remove Node");
        
        nodes.Remove(node);
        
        if (!Application.isPlaying)
        {
            Undo.DestroyObjectImmediate(node);
        }
        
        EditorUtility.SetDirty(this);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        DecoratorNode deco = parent as DecoratorNode;
        if (deco)
        {
            Undo.RecordObject(deco, "Behaviour Tree (AddChild)");
            deco.child = child;
            EditorUtility.SetDirty(deco);
        }
        
        CompositeNode comp = parent as CompositeNode;
        if (comp)
        {
            Undo.RecordObject(comp, "Behaviour Tree (AddChild)");
            comp.children.Add(child);
            EditorUtility.SetDirty(comp);
        }
        
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
            rootNode.child = child;
            EditorUtility.SetDirty(rootNode);
        }
        
    }

    public void RemoveChild(Node parent, Node child)
    {
        DecoratorNode deco = parent as DecoratorNode;
        if (deco)
        {
            Undo.RecordObject(deco, "Behaviour Tree (removeChild)");
            deco.child = null;
            EditorUtility.SetDirty(deco);
        }
        
        CompositeNode comp = parent as CompositeNode;
        if (comp)
        {
            Undo.RecordObject(comp, "Behaviour Tree (AddChild)");
            comp.children.Remove(child);
            EditorUtility.SetDirty(comp);
        }
        
        RootNode root = parent as RootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behaviour Tree (AddChild)");
            root.child = null;
            EditorUtility.SetDirty(root);
        }
    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        DecoratorNode deco = parent as DecoratorNode;
        if (deco && deco.child != null)
        {
            children.Add(deco.child);
        }
        
        RootNode root = parent as RootNode;
        if (root && root.child != null) children.Add(root.child);
        
        CompositeNode comp = parent as CompositeNode;
        if (comp) return comp.children;
        return children;
        
       
    }
#endif
    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }
    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.root = tree.root.Clone();
        tree.blackboard = blackboard.Clone();
        tree.nodes = new List<Node>();
        Traverse(tree.root, (n) =>
        {
            tree.nodes.Add(n);
        });
        return tree;
    }

    public void Bind()
    {
        Traverse(root, node =>
        {
            node.blackboard = blackboard;
        });
    }
    public Node CloneNode(Node original)
    {
        Node clone = original.Clone(); // uses Node.Clone()
        clone.guid = GUID.Generate().ToString();
        return clone;
    }
    
    public Node CopyNodeData(Node source)
    {
        Node newNode = CreateNode(source.GetType());

        var fields = source.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(HideInInspector), true)) continue;
            if (field.Name == "guid" || field.Name == "position") continue;

            field.SetValue(newNode, field.GetValue(source));
        }

        newNode.description = source.description;
        newNode.position = source.position;

        return newNode;
    }
    
}
