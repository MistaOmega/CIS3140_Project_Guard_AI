using UnityEngine.AI;

namespace AI.GuardStates
{
    public abstract class GuardBaseState
    {
        private NavMeshAgent _agent;
        public abstract void EnterState(GuardStateMan guard);


        public abstract void UpdateState(GuardStateMan guard);


    }
}