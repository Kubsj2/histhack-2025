namespace BehaviorTree.Nodes
{
    public class SplitNode : CompositeNode
    {
        public override Category category => Category.GeneralNodes;

        public override string tooltip =>
            "The Split Node Executes its children left to right regardless of their success";

        private int _current;

        protected override void OnStart()
        {
            _current = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            var child = children[_current];
            switch (child.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                case State.Success:
                {
                    _current++;
                    break;
                }
                
            }
            return _current >= children.Count ? State.Success : State.Running;
        }
    }
}