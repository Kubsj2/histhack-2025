using UnityEngine;

namespace BehaviorTree.Nodes
{
    public class CompareValueNode : DecoratorNode
    {
        public override Category category => Category.GeneralNodes;
        public enum VariableType
        {
            Bool, 
            Int, 
            String
        }
        public VariableType type;
        public override string tooltip =>
            "This node updates its child only if the comparison returns 'true', otherwise the CompareValueNode will return Failure.";

        [Header("Blackboard variable key")]
        public string key;

        public string stringValue;
        public int intValue;
        public bool boolValue;

        private bool _comparedBool;
        private int _comparedInt;
        private string _comparedString;
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {

            switch (type)
            {
                case VariableType.Bool:
                    _comparedBool = blackboard.GetValue<bool>(key);
                    if (_comparedBool == boolValue)
                    {
                        child.Update();
                        return State.Success;
                    }

                    break;
                case VariableType.Int:
                    _comparedInt = blackboard.GetValue<int>(key);
                    if (_comparedInt == intValue)
                    {
                        child.Update();
                        return State.Success;
                    }
                    break;
                case VariableType.String:
                    _comparedString = blackboard.GetValue<string>(key);
                    if (_comparedString == stringValue)
                    {
                        child.Update();
                        return State.Success;
                    }
                    break;
                default:
                    Debug.LogError($"{key} - Unknown variable type: {type}!");
                    return State.Failure;
            }
            return State.Failure;

            
            
        }
    }
}