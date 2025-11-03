using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorTree.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        //This tree is selected by menu GUI
        private BehaviourTree _selectedTree;
        
        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        private VisualElement _blackboardView;

        private ToolbarButton _selectTreeButton;
        private ToolbarMenu _blackboardVariableMenu;
        private ToolbarButton _centerButton;
        private ToolbarButton _autoAlignButton;
        private ToolbarToggle _gridSnappingToggle;
        
        private SerializedObject _treeObject;
        private SerializedProperty _blackboardProperty;

        private int _objectPickerID;
        private bool _enableSnapping = true;
        [MenuItem("Window/Behaviour Tree Editor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("Behaviour Tree Editor");
        }
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }
            return false;
        }
        public void CreateGUI()
        {

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/BehaviorTree/Editor/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(rootVisualElement);
            VisualElement root = rootVisualElement;
        

            var style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/BehaviorTree/Editor/BehaviourTreeEditor.uss");
            root.styleSheets.Add(style);
        
            _treeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _gridSnappingToggle = root.Q<ToolbarToggle>("grid-toggle");
            _blackboardView = root.Q<VisualElement>("blackboard-container");
            _blackboardVariableMenu = root.Q<ToolbarMenu>("blackboard-variable-menu");
            _centerButton = root.Q<ToolbarButton>("center-button");  
            _selectTreeButton = root.Q<ToolbarButton>("select-tree");
            
            _treeView.OnNodeSelected = OnNodeSelectionChanged;



            _selectTreeButton.clicked += () =>
            {
                _objectPickerID = GUIUtility.GetControlID("EditorGUIUtility_ShowObjectPicker".GetHashCode(), FocusType.Keyboard, position);
                EditorGUIUtility.ShowObjectPicker<BehaviourTree>(_selectedTree, false, "", _objectPickerID);
               
            };
            

            _gridSnappingToggle.RegisterValueChangedCallback(a =>
            {
                _enableSnapping = a.newValue;
                
                foreach (var node in _treeView.nodes)
                {
                    if (node is NodeView nodeView)
                    {
                        nodeView.enableSnapping = _enableSnapping;

                        if (_enableSnapping)
                            nodeView.capabilities |= Capabilities.Snappable;
                        else
                            nodeView.capabilities &= ~Capabilities.Snappable;
                    }
                }
            });

            
            
            _blackboardVariableMenu.menu.AppendAction("String", _ => _treeView.AddVariableToBlackboard(typeof(BlackboardString)));
            _blackboardVariableMenu.menu.AppendAction("Bool", _ => _treeView.AddVariableToBlackboard(typeof(BlackboardBool)));
            _blackboardVariableMenu.menu.AppendAction("Int", _ => _treeView.AddVariableToBlackboard(typeof(BlackboardInt)));
            
            

            _centerButton.clicked += () =>
            {
                _treeView.FrameAll();
            };
            _autoAlignButton = root.Q<ToolbarButton>("auto-align-button");
            _autoAlignButton.clicked += () =>
            {
                _treeView.OrganizeTree();
            };
            
            var imgui = new IMGUIContainer();
            _blackboardView.Add(imgui);
            imgui.onGUIHandler = () =>
            {
                if (_treeObject == null) return;

                _treeObject.Update();
                foreach (var variable in _treeView._tree.blackboard.variables)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(variable.key);

                    if (variable is BlackboardString strVar)
                    {
                        strVar.Value = EditorGUILayout.TextField(strVar.Value);
                    }
                    else if (variable is BlackboardInt intVar)
                    {
                        intVar.Value = EditorGUILayout.IntField(intVar.Value);
                    }
                    else if (variable is BlackboardBool boolVar)
                    {
                        boolVar.Value = EditorGUILayout.Toggle(boolVar.Value);
                    }

                    if (EditorGUILayout.LinkButton("Delete"))
                    {
                        _treeView.RemoveVariableFromBlackboard(variable.key);
                        _treeObject.Update();
                        _treeObject.ApplyModifiedProperties();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                _treeObject.ApplyModifiedProperties();
            };
            
            
            
            if (Selection.activeObject is BehaviourTree tree)
            {
                InitializeTree(tree);
            }
            else if (_selectedTree != null)
            {
                InitializeTree(_selectedTree);
            }
        }

        private void InitializeTree(BehaviourTree tree)
        {
            _treeView.PopulateView(tree);
            _treeObject = new SerializedObject(tree);
            _blackboardProperty = _treeObject.FindProperty("blackboard");
        }
        private void OnSelectionChange()
        {
            
            BehaviourTree tree = Selection.activeObject as BehaviourTree;

                if (!tree && Selection.activeGameObject)
                {
                    BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                    if (runner) tree = runner.tree;
                } 
                
            
                
            
            if (Application.isPlaying)
            {
                if (tree != null)
                {
                    _treeView.PopulateView(tree);
                }
                
            } else
            if (tree != null && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                _treeView.PopulateView(tree);
            }


            if (tree != null)
            {
                _treeObject = new SerializedObject(tree);
                _blackboardProperty = _treeObject.FindProperty("blackboard");
            }
           
        }

        void OnNodeSelectionChanged(NodeView view)
        {
            _inspectorView.UpdateSelection(view);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (_treeView == null) return;
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                
            }
        }

        private void OnInspectorUpdate()
        {

            _treeView?.UpdateNodeStates();
        }

        private void OnGUI()
        {
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == _objectPickerID)
                {
                    _selectedTree = EditorGUIUtility.GetObjectPickerObject() as BehaviourTree;
                    if (_selectedTree != null)
                    {
                        EditorUtility.FocusProjectWindow();
                        EditorGUIUtility.PingObject(_selectedTree);
                        InitializeTree(_selectedTree);
                        Repaint();
                    }

                    _objectPickerID = -1;
                }
            }
        }
    }
    
    
}
