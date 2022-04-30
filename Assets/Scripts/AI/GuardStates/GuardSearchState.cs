using System;

namespace AI.GuardStates
{
    public class GuardSearchState : GuardBaseState
    {
        public override void EnterState(GuardStateMan guard)
        {
            guard.SetAgentSpeed((float)(guard.initialAgentSpeed * 1.2));
            guard.agent.isStopped = false; // this shouldn't be needed. I will have it here anyway
        }

        public override void UpdateState(GuardStateMan guard)
        {
            throw new NotImplementedException();
        }

        public override void OnCollisionEnter(GuardStateMan guard)
        {
            throw new NotImplementedException();
        }
    }
}