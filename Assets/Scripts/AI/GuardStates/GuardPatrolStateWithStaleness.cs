using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using worldspace;

namespace AI.GuardStates
{
    /// <summary>
    ///     This class is designed to include the staleness system designed for the grid
    ///     This class will look to move the guards in much the same way as the original patrol function
    ///     However this will use a different method of acquiring the next position vector
    /// </summary>
    public class GuardPatrolStateWithStaleness : GuardBaseState
    {
        private const float BaseWaitTime = 3.0f; // how long to wait before moving to new point

        private readonly float
            magDistance =
                0.1f; // distance magnifier. This is used to determine how much distance affects next location 

        private float _currentWaitTime; // current wait time

        private Vector3 _moveToPosition; // next position guard is moving to 
        private GridObject go; // GridObject instance


        /// <summary>
        ///     This is ran once when the guard state changes
        /// </summary>
        /// <param name="guard">Instance of the Guard State Machine Manager</param>
        public override void EnterState(GuardStateMan guard)
        {
            if (guard.agent.autoBraking)
                guard.agent.autoBraking = false; // ensure guards don't slow down as they approach area
            if (guard.agent.isStopped) guard.agent.isStopped = false; // ensure guard is able to move

            guard.SetGridVisualiser(); // We need a grid visualiser to be set to use it's GridObject
            go = guard.GetGridVisualiser().GetGridObject(); // get the instance of the GridObject

            _moveToPosition =
                GetNextRoamingPosition(guard, guard.transform.position, 5,
                    -1); // Get the next position for the guard to navigate to
            guard.agent.SetDestination(_moveToPosition); // set destination as the new position vector

            guard.SetAgentSpeed(guard
                .initialAgentSpeed); // set the guard's speed (this changes when attacking so need to reset)

            _currentWaitTime = BaseWaitTime; // ensure the guard has a wait timer 
            guard.SetDroppingStaleness(true);
            guard.runCoroutine(LowerStaleness(1f,
                guard)); // create a coroutine handling the lowering of a region's staleness value.
        }

        /// <summary>
        ///     Coroutine enumerator for handling the lowering of a staleness value
        /// </summary>
        /// <param name="waitTime"></param>
        /// <param name="guard"></param>
        /// <returns></returns>
        public IEnumerator LowerStaleness(float waitTime, GuardStateMan guard)
        {
            while (guard.GetDroppingStaleness()) 
            {
                yield return new WaitForSeconds(waitTime); // wait for the allocated time
                go.ModifyValueAtCell(guard.transform.position,
                    -6); // we add one every tick, so let's take that + 5 away.
            }
        }

        /// <summary>
        ///     This UpdateState handles the standard guard patrol
        ///     Check to see if agent has arrived at destination.
        ///     If so, check wait time, if still waiting return, if not set a new destination
        /// </summary>
        /// <param name="guard">Guard State Manager instance</param>
        public override void UpdateState(GuardStateMan guard)
        {
            if (go == null)
            {
                guard.SetGridVisualiser();
                go = guard.GetGridVisualiser().GetGridObject();
            }

            go = guard.GetGridVisualiser().GetGridObject();
            if (!(Vector3.Distance(guard.transform.position, _moveToPosition) < 5f)) return;
            guard.agent.isStopped = true;


            if (_currentWaitTime <= 0.5)
            {
                _moveToPosition = GetNextRoamingPosition(guard, guard.transform.position, 5, -1);
                guard.agent.isStopped = false;
                guard.agent.SetDestination(_moveToPosition);
                _currentWaitTime = 3;
            }
            else
            {
                _currentWaitTime -= Time.deltaTime;
            }
        }



        /// <summary>
        ///     Gets the next roaming position for the guard based on staleness
        /// </summary>
        /// <param name="guard">Guard State Manager instance</param>
        /// <param name="origin">Current position vector</param>
        /// <param name="dist"> maximum distance for navhit</param>
        /// <param name="layermask">
        ///     Navigable layermask
        ///     https://docs.unity3d.com/540/Documentation/ScriptReference/NavMesh.SamplePosition.html
        /// </param>
        /// <returns>New position on the navmesh to navigate to</returns>
        private Vector3 GetNextRoamingPosition(GuardStateMan guard, Vector3 origin, float dist, int layermask)
        {
            Vector3 newVec = go.GetNextGridLocation(origin, magDistance);
            newVec += new Vector3(Random.Range(2, go.Sz), 0, Random.Range(2, go.Sz)) * .5f;

            int x, z;
            go.GetXZFromVec(newVec, out x, out z);
            guard.SetGuardAssignedGridPosition(x, z);

            NavMeshHit navHit;

            NavMesh.SamplePosition(newVec, out navHit, dist, layermask);

            if (navHit.distance > 1000000) // should prevent infinity coords
            {
                GetNextRoamingPosition(guard, origin, dist, layermask);
            }

            Debug.Log(navHit.position.ToString());

            return navHit.position;
        }
    }
}