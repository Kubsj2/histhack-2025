using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTree.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public Node node;
        public Port input;
        public Port output;
        public bool enableSnapping = true;

        private string baseTexPath = "Assets/Textures/Icons/";
        public NodeView(Node node) : base("Assets/Scripts/BehaviorTree/Editor/NodeView.uxml")
        {
            this.tooltip = node.tooltip;
            this.viewDataKey = node.guid;
            this.node = node;    
            title = GenerateTitle();
            style.left = node.position.x;
            style.top = node.position.y;
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            
            if (enableSnapping)
                capabilities |= Capabilities.Snappable;
            else
                capabilities &= ~Capabilities.Snappable;
            
            
            VisualElement iconContainer = this.Q<VisualElement>("node-icon");
            if (iconContainer != null )
            {
                Image icon = new Image();
                icon.style.width = 20;
                icon.style.height = 20;
                string path = "";
                if (node is SequenceNode) path = baseTexPath + "tabler--arrows-split.png"; 
                else if (node is DebugLogNode) path = baseTexPath + "tabler--bug-filled.png";
                else if (node is RepeatNode) path = baseTexPath + "tabler--repeat.png";
                else if (node is DialogueNode) path = baseTexPath + "tabler--bubble-text.png";
                else if (node is DialogueStartNode) path = baseTexPath + "tabler--bubble-plus.png";
                else if (node is PickDialogueOptionNode) path = baseTexPath + "tabler--chart-bubble.png";
                else if (node is WaitNode) path = baseTexPath + "tabler--player-pause.png";
                else if (node is RootNode) path = baseTexPath + "tabler--binary-tree.png";
                else path = baseTexPath + "tabler--question-mark.png";
                icon.image = AssetDatabase.LoadAssetAtPath<Texture>(path);
                iconContainer.Add(icon);
            }
            
            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));
        }

        private string GenerateTitle()
        {
            string basetitle = node.name;
            string noNodeString = node is RootNode ? basetitle : basetitle.Replace("Node", "");
            string[] split =  Regex.Split(noNodeString, @"(?<!^)(?=[A-Z])");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < split.Length; i++)
            {
                sb.Append(split[i]);
                if (i < split.Length - 1) sb.Append(' ');
            }
            return sb.ToString();
        }
        private void SetupClasses()
        {
            if (node is ActionNode) AddToClassList("action");
            else if (node is CompositeNode) AddToClassList("composite");
            else if (node is DecoratorNode) AddToClassList("decorator");
            else if (node is RootNode) AddToClassList("root");
        }

        private void CreateInputPorts()
        {
            if (node is ActionNode)
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            } else if (node is CompositeNode)
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (node is RootNode)
            {
                
            }
            else 
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (input != null)
            {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts()
        {
            if (node is ActionNode)
            {

            } else if (node is CompositeNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else if (node is RootNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            else
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            
            
            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour tree (Set Position)");
            node.position.x = newPos.x;
            node.position.y = newPos.y;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            
            if (enableSnapping)
                capabilities |= Capabilities.Snappable;
            else
                capabilities &= ~Capabilities.Snappable;
            
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        public void SortChildren()
        {
            CompositeNode composite = node as CompositeNode;
            if (composite)
            {
                composite.children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(Node left, Node right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("success");
            RemoveFromClassList("failure");
            
            if (!Application.isPlaying) return;
            switch (node.state)
            {
                case Node.State.Running:
                    if (node.started) AddToClassList("running");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
            }
        }
    }
    
    
}
