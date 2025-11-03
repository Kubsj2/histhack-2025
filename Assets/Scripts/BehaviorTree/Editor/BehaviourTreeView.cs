using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace BehaviorTree.Editor
{
    public class BehaviourTreeView : GraphView
    {
        public BehaviourTree _tree;
        public Action<NodeView> OnNodeSelected;
        private readonly GridBackground _gridBackground = new GridBackground();
        
        
        private readonly List<Node> _copiedNodes = new();
        private readonly Dictionary<string, List<string>> _copiedConnections = new();
        private Vector2 _pastePosition;
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
        public BehaviourTreeView()
        {
            Insert(0, _gridBackground);
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            serializeGraphElements += HandleCopy;
            unserializeAndPaste += (_, __) => HandlePaste(new Vector2(300, 300));
            canPasteSerializedData += _ => true;
            
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/BehaviorTree/Editor/BehaviourTreeEditor.uss");
            styleSheets.Add(stylesheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }
        
        private void HandleCut(List<NodeView> selectedNodeViews)
        {
            HandleCopy(selectedNodeViews);
            Undo.RegisterCompleteObjectUndo(_tree, "Cut Nodes");

            foreach (var view in selectedNodeViews)
            {
                _tree.DeleteNode(view.node);
            }

            PopulateView(_tree);
            EditorUtility.SetDirty(_tree);
        }
        
        private string HandleCopy(IEnumerable<GraphElement> elements)
        {
            _copiedNodes.Clear();
            _copiedConnections.Clear();

            var nodeViews = elements.OfType<NodeView>().ToList();
            var selected = nodeViews.Select(nv => nv.node).ToList();
            
            foreach (var node in selected)
            {
                _copiedNodes.Add(node);
                var children = _tree.GetChildren(node).Where(c => selected.Contains(c)).ToList();
                _copiedConnections[node.guid] = children.Select(c => c.guid).ToList();
            }

            return "Copy";
        }

        private void HandlePaste(Vector2 position)
        {
            if (_copiedNodes.Count == 0 || _tree == null) return;

            Undo.RegisterCompleteObjectUndo(_tree, "Paste Nodes");

            Dictionary<string, Node> guidToNew = new();
            List<Node> newNodes = new();

            Vector2 offset = new Vector2(30, 30);
            Vector2 basePos = _copiedNodes[0].position;

            foreach (var original in _copiedNodes)
            {
                Node newNode = _tree.CopyNodeData(original);
                newNode.position = position + (original.position - basePos) + offset;
                newNode.description = original.description;

                guidToNew[original.guid] = newNode;
                newNodes.Add(newNode);
                
                
            }

            foreach (var kvp in _copiedConnections)
            {
                if (!guidToNew.TryGetValue(kvp.Key, out var newParent)) continue;

                foreach (var childGuid in kvp.Value)
                {
                    if (guidToNew.TryGetValue(childGuid, out var newChild))
                    {
                        _tree.AddChild(newParent, newChild);
                    }
                }
            }

            PopulateView(_tree);

            ClearSelection();
            foreach (var node in newNodes)
            {
                var view = FindNodeView(node);
                if (view != null) AddToSelection(view);
            }

            EditorUtility.SetDirty(_tree);
            AssetDatabase.SaveAssets();
        }

        private void ClearClipboard()
        {
            _copiedNodes.Clear();
            _copiedConnections.Clear();
            
        }
        
        private void OnUndoRedo()
        {
            if (_tree != null)
            {
                EditorUtility.SetDirty(_tree); 
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_tree));
                PopulateView(_tree);
                AssetDatabase.SaveAssets();
            }
        }

        NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        public void PopulateView(BehaviourTree tree)
        {
            _tree = tree;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (_tree.blackboard == null)
            {
                _tree.blackboard = ScriptableObject.CreateInstance(typeof(Blackboard)) as Blackboard;
                _tree.blackboard.name = "Blackboard";
                AssetDatabase.AddObjectToAsset(_tree.blackboard, tree);
                EditorUtility.SetDirty(_tree);
                EditorUtility.SetDirty(_tree.blackboard);
                AssetDatabase.SaveAssets();
            }
            
            if (_tree.root == null)
            {
                _tree.root = _tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(_tree);
                AssetDatabase.SaveAssets();
            }
            
            tree.nodes.ForEach(CreateNodeView);
            
            tree.nodes.ForEach(n =>
            {
                var children = tree.GetChildren(n);
                children.ForEach(c =>
                {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList(); 
            
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            if (graphviewchange.elementsToRemove != null)
            {
                graphviewchange.elementsToRemove.ForEach(elem =>
                {
                    if (elem is NodeView nodeView) _tree.DeleteNode(nodeView.node);

                    if (elem is Edge edge)
                    {
                        if (edge.input.node is NodeView childView && 
                            edge.output.node is NodeView parentView) 
                            _tree.RemoveChild(parentView.node, childView.node);
                    }
                });
            }

            if (graphviewchange.edgesToCreate != null)
            {
                graphviewchange.edgesToCreate.ForEach(edge =>
                {
                    if (edge.output.node is NodeView parentView && 
                        edge.input.node is NodeView childView) 
                        _tree.AddChild(parentView.node, childView.node);
                });
            }

            if (graphviewchange.movedElements != null)
            {
                nodes.ForEach((n) =>
                {
                    if (n is NodeView view)
                        view.SortChildren();
                });
            }
            ValidateTree();
            return graphviewchange;
        }

        void CreateNode(Type type, Vector2 position)
        {
            Node node = _tree.CreateNode(type);
            node.position = position;
            CreateNodeView(node);
        }
        void CreateNodeView(Node node)
        {
            NodeView nv = new NodeView(node);
            nv.OnNodeSelected = OnNodeSelected;
            AddElement(nv);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            
            Vector2 localPos = _gridBackground.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            var baseTypes = TypeCache.GetTypesDerivedFrom<Node>();
            foreach (var baseType in baseTypes)
            {
                var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType);
                foreach (var type in derivedTypes)
                {
                        string[] nodeBaseTypeName = Regex.Split(type.BaseType.Name, @"(?<!^)(?=[A-Z])");
                        string nodeBaseType = "";
                        foreach (string s in nodeBaseTypeName) nodeBaseType += (s + " ");
                        
                        string[] nodeTypeName = Regex.Split(type.Name, @"(?<!^)(?=[A-Z])");
                        string nodeType = "";
                        foreach (var s in nodeTypeName) nodeType += (s + " ");
                        
                        evt.menu.AppendAction($"Add Node/ {nodeBaseType} / {nodeType}", _ => CreateNode(type, localPos));
                }
            }
            bool hasSelection = selection.Any(s => s is NodeView);
            bool hasCopiedData = _copiedNodes.Count > 0;
            var selectedNodes = selection.OfType<NodeView>().ToList();

            evt.menu.AppendSeparator();

            evt.menu.AppendAction("Cut", _ => HandleCut(selectedNodes), hasSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            
            evt.menu.AppendAction("Copy", _ => HandleCopy(selectedNodes), hasSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

            evt.menu.AppendAction("Paste", _ => HandlePaste(localPos), hasCopiedData ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            
            evt.menu.AppendSeparator();
            
            evt.menu.AppendAction("Clear Clipboard", _ =>
            {
                _copiedNodes.Clear();
                _copiedConnections.Clear();
                hasCopiedData = false;
            });
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                NodeView view = n as NodeView;
                view.UpdateState();
            });
        }
        
        public void OrganizeTree()
        {
            if (_tree.root == null || _tree == null) return;

            float verticalSpacing = 50f;
            float horizontalSpacing = 20f;

            float StartLayout(Node node, float x, float y)
            {
                NodeView view = FindNodeView(node);
                
                float nodeWidth = view?.resolvedStyle.width ?? 160f;
                float nodeHeight = view?.resolvedStyle.height ?? 100f;

                List<Node> children = _tree.GetChildren(node);

                if (children.Count == 0)
                {
                    node.position = new Vector2(x, y);
                    view?.SetPosition(new Rect(node.position, new Vector2(nodeWidth, nodeHeight)));
                    return x + nodeWidth + horizontalSpacing;
                }

                float childX = x;
                float childY = y + nodeHeight + verticalSpacing;

                float totalWidth = 0f;
                foreach (Node child in children)
                {
                    childX = StartLayout(child, childX, childY);
                    totalWidth += (FindNodeView(child)?.resolvedStyle.width ?? 160f) + horizontalSpacing;
                }

                float subtreeWidth = totalWidth - horizontalSpacing;
                float thisX = x + subtreeWidth / 2f - nodeWidth / 2f;

                node.position = new Vector2(thisX, y);
                view?.SetPosition(new Rect(node.position, new Vector2(nodeWidth, nodeHeight)));

                view.SortChildren();
                return x + subtreeWidth + horizontalSpacing;
            }

            StartLayout(_tree.root, 0, 0);
        }

        private float LayoutNodeRecursive(Node node, ref float x, float y, float hSpacing, float vSpacing)
        {
            var children = _tree.GetChildren(node);
            float childX = x;

            if (children.Count == 0)
            {
                node.position = new Vector2(x, y);
                x += hSpacing;
                return node.position.x;
            }

            float midX = 0;
            foreach (var child in children)
            {
                midX = LayoutNodeRecursive(child, ref x, y + vSpacing, hSpacing, vSpacing);
            }
            
            node.position = new Vector2(AverageChildX(children), y);

            
            foreach (var n in _tree.nodes)
            {
                EditorUtility.SetDirty(n);
            }
            AssetDatabase.SaveAssets();
            
            return node.position.x;
            
            
        }

        private float AverageChildX(List<Node> children)
        {
            if (children.Count == 0) return 0;
            float sum = 0;
            foreach (var c in children)
            {
                sum += c.position.x;
            }
            return sum / children.Count;
        }
        private void ValidateTree()
        {
            if (_tree == null) return;
            HashSet<string> guids = new HashSet<string>();
            foreach (var node in _tree.nodes)
            {
                if (!guids.Add(node.guid))
                {
                    Debug.LogError($"Duplicate GUID found: {node.guid} in {_tree.name}");
                }

                if (node == null)
                {
                    Debug.LogError("Null node reference in tree.");
                }
            }
        }
        
        public void AddVariableToBlackboard(Type type)
        {
            if (_tree == null) return;

            Undo.RegisterCompleteObjectUndo(_tree.blackboard, "Add Blackboard Variable");

            var variable = ScriptableObject.CreateInstance(type) as BlackboardVariable;
            variable.name = $"New {type.Name.Replace("Blackboard", "")}";
            variable.key = "Variable" + _tree.blackboard.variables.Count;
            _tree.blackboard.variables.Add(variable);
            AssetDatabase.AddObjectToAsset(variable, _tree);
            EditorUtility.SetDirty(_tree);
            AssetDatabase.SaveAssets();
            
        }

        public void RemoveVariableFromBlackboard(string key)
        {
            if (_tree == null) return;
            Undo.RegisterCompleteObjectUndo(_tree.blackboard, "Remove Blackboard Variable");
            var variable = _tree.blackboard.variables.Find(v => v.key == key);
            _tree.blackboard.variables.Remove(variable);
            if (!Application.isPlaying) Undo.DestroyObjectImmediate(variable);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_tree));
            EditorUtility.SetDirty(_tree);
            EditorUtility.SetDirty(_tree.blackboard);
            AssetDatabase.SaveAssets();
        }
    }
    
    
}
