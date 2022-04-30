using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using UnityEngine;

namespace AI.GuardStates
{
    public class GuardAttackState : GuardBaseState
    {
        private static List<Collider> _slaves;
        private float _range;
        private List<Transform> targetTransforms = new List<Transform>();

        public override void EnterState(GuardStateMan guard)
        {
            Debug.Log("Entered Attack State");
            guard.agent.isStopped = false;
            guard.SetAgentSpeed((float)(guard.initialAgentSpeed * 1.2));
            _range = guard._FOV.viewRadius;
            _slaves ??= new List<Collider>();
            BroadcastToAgentsInRange(guard);
            TesterScript.Instance.DetectThief();
        }


        public override void UpdateState(GuardStateMan guard)
        {
            Vector3 lastKnownPosition = Controller.Instance.transform.position;
            // Debug.Log("Attacking Target");
            if (!IsTargetStillWithinRange(guard))
            {
                guard.SetOnDamaging(
                    false); // called here as it may never reach FOV check if player gets out of range fast enough 
                if (!(Vector3.Distance(guard.agent.transform.position, lastKnownPosition) < 5)) return;
                guard.agent.ResetPath();
                guard.agent.isStopped = true;
                return;
            }

            if (guard.agent.isStopped) guard.agent.isStopped = false;
            Transform targetTransform = Controller.Instance.transform;

            Vector3 localPosition = targetTransform.localPosition;
            Vector3 dir = localPosition - guard.agent.transform.position;
            guard.agent.SetDestination(localPosition);

            Quaternion rotation = Quaternion.LookRotation(dir);
            guard.agent.transform.rotation =
                Quaternion.Lerp(guard.agent.transform.rotation, rotation, 5 * Time.deltaTime);
            if (!IsTargetInFOVField(guard))
            {
                guard.SetOnDamaging(false);
                return;
            }

            if (guard.onDamaging) return;
            guard.SetOnDamaging(true);
            guard.StartOnDamage(); // the coroutine ends when OnDamaging is false, so starting it once from an update should be okay
        }

        public override void OnCollisionEnter(GuardStateMan guard)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Instead of being within the FOV cone constantly, this checks if the player is still within the full viewing circle.
        ///     This can be changed. Maybe. Depends how well it works.
        /// </summary>
        /// <param name="guard"></param>
        /// <returns></returns>
        private bool IsTargetStillWithinRange(GuardStateMan guard)
        {
            return Vector3.Distance(guard.agent.transform.position, Controller.Instance.GetPosition()) < _range;
        }

        private bool IsTargetInFOVField(GuardStateMan guard)
        {
            FOV fov = guard._FOV;
            targetTransforms = fov.FindVisibleTargets();

            return targetTransforms.Any();
        }

        /// <summary>
        ///     Head agent broadcasts to all others and assigns them as slaves. Permanently keeps track of all slaves until state
        ///     is ended.
        /// </summary>
        /// <param name="guard"></param>
        private void BroadcastToAgentsInRange(GuardStateMan guard)
        {
            if (guard.IsSlave()) return;

            Collider[] colliders = Physics.OverlapSphere(guard.agent.transform.position, guard._FOV.viewRadius);
            Debug.Log($"Hello {colliders.Length}");
            if (colliders.Length == 0) return;

            foreach (Collider c in colliders)
            {
                _slaves ??= new List<Collider>();

                if (_slaves.Contains(c)) continue;
                _slaves.Add(c);
                GameObject gameObject = c.gameObject;

                if (gameObject.GetComponent<GuardStateMan>() == null) continue;
                GuardStateMan otherGuard = gameObject.GetComponent<GuardStateMan>();
                otherGuard.ChangeState(otherGuard._guardAttackState);
            }
        }
    }
}