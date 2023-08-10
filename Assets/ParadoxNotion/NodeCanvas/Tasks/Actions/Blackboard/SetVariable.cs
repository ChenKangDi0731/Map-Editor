using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Blackboard")]
    public class SetVariable<T> : ActionTask
    {

        [BlackboardOnly]
        public BBParameter<T> valueA;
        public BBParameter<T> valueB;

        protected override string info {
            get { return "<color=#0000dd>"+valueA + " = " + valueB+"</color>"; }
        }

        protected override void OnExecute() {
            valueA.value = valueB.value;
            EndAction();
        }
    }
}