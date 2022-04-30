using UnityEngine;
using UnityEngine.AI;

namespace AI.GuardStates
{
    /// <summary>
    ///     This is more of a wandering style patrol, but a patrol system nonetheless, may split between a standard set path
    ///     patrol and this one. Will eventually use region staleness to determine navigation
    /// </summary>
    public class GuardPatrolState : GuardBaseState
    {
        private readonly float _baseWaitTime = 3.0f;
        private float _currentWaitTime;

        private float
            _minX,
            _maxX,
            _minZ,
            _maxZ; // boundary variables to be used to ensure the guard doesn't travel outside the play area

        private Vector3 _moveToPosition;


        public override void EnterState(GuardStateMan guard)
        {
            if (guard.agent.autoBraking) guard.agent.autoBraking = false;
            if (guard.agent.isStopped) guard.agent.isStopped = false;
            //Debug.Log($"MAX X = {_maxX}");
            if (_maxX == 0 || _maxZ == 0) // get patrol area to allow for navigation to fit inside area
                GetBoundariesOfPatrolArea();

            _moveToPosition = GetRandomRoamingPosition(guard.transform.position, 15, -1);
            guard.agent.SetDestination(_moveToPosition);
            guard.SetAgentSpeed(guard.initialAgentSpeed);

            _currentWaitTime = _baseWaitTime;
        }

        /// <summary>
        ///     This UpdateState handles the standard guard patrol
        ///     Check to see if agent has arrived at destination.
        ///     If so, check wait time, if still waiting return, if not set a new destination
        /// </summary>
        /// <param name="guard">Guard State Manager instance</param>
        public override void UpdateState(GuardStateMan guard)
        {
            if (!(Vector3.Distance(guard.transform.position, _moveToPosition) < 5f)) return;
            guard.agent.isStopped = true;
            if (_currentWaitTime <= 0.5)
            {
                _moveToPosition = GetRandomRoamingPosition(guard.transform.position, 10, -1);
                guard.agent.isStopped = false;
                guard.agent.SetDestination(_moveToPosition);
                _currentWaitTime = 3;
            }
            else
            {
                _currentWaitTime -= Time.deltaTime;
            }
        }

        public override void OnCollisionEnter(GuardStateMan guard)
        {
            guard.ChangeState(guard._guardAttackState);
        }

        /// <summary>
        ///     https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950/
        ///     Gets random patrol region for agent that satisfies the navmesh region
        /// </summary>
        /// <param name="origin">Starting vector</param>
        /// <param name="dist">Distance from start vector</param>
        /// <param name="layermask">Layer for physics raycast, ignored here</param>
        /// <returns></returns>
        private Vector3 GetRandomRoamingPosition(Vector3 origin, float dist, int layermask)
        {
            Vector3 newVec =
                new Vector3(Random.Range(_minX, _maxX), origin.y,
                    Random.Range(_minZ, _maxZ)); // gather a random new positon that fits in the region space

            NavMeshHit navHit;

            NavMesh.SamplePosition(newVec, out navHit, dist, layermask);

            Debug.Log(navHit.position.ToString());
            return navHit.position;
        }

        /// <summary>
        ///     Gets the maximum boundaries for the walkable region using the ground renderer's Bounds property
        /// </summary>
        private void GetBoundariesOfPatrolArea() // could probably use the navmesh for this, but this works
        {
            GameObject ground = GameObject.FindWithTag("Ground");
            Renderer groundSz = ground.GetComponent<Renderer>();
            Bounds bounds = groundSz.bounds;
            _minX = bounds.center.x - bounds.extents.x + 2;
            _maxX = bounds.center.x + bounds.extents.x - 2;
            _minZ = bounds.center.z - bounds.extents.z + 2;
            _maxZ = bounds.center.z + bounds.extents.z - 2;
            //Debug.Log($"MIN X = {_minX}, MAX X = {_maxX}, MIN Z = {_minZ}, MAX Z = {_maxZ}");
        }
    }
}