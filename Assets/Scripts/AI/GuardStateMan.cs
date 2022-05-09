/* Jack Nash, Edge Hill University. 2022 */

using System.Collections;
using AI.GuardStates;
using UnityEngine;
using UnityEngine.AI;
using worldspace;

namespace AI
{
    /// <summary>
    ///     This class is responsible for managing the state of a guard
    ///     This State Manager will also keep instances of multiple guard components such as the FOV and NavMeshAgent, which
    ///     will be used by the individual states.
    /// </summary>
    public class GuardStateMan : MonoBehaviour
    {
        public NavMeshAgent
            agent; // this will get managed by the states, but as one is only in use at a time, there should be no problems with this approach. I hope.

        public FOV _FOV;
        public float initialAgentSpeed = 3.5f;
        public bool isSlave;
        public bool onDamaging;
        public GridVisualiser _gridVisualiser;
        private GuardBaseState _currentState; // Holds reference to whatever state the guard is currently in
        private bool _droppingStaleness;
        public GuardAttackState _guardAttackState = new GuardAttackState();
        private int _guardNavX;
        private int _guardNavZ;
        public GuardPatrolState _guardPatrolState = new GuardPatrolState();
        public GuardPatrolStateWithStaleness _GuardPatrolStateWithStaleness = new GuardPatrolStateWithStaleness();
        public GuardSearchState _guardSearchState = new GuardSearchState();

        /// <summary>
        ///     Start function runs at the start of the game state, inherited from MonoBehaviour
        ///     Sets initial state to patrol state.
        /// </summary>
        private void Start()
        {
            _currentState = _GuardPatrolStateWithStaleness;
            agent = GetComponent<NavMeshAgent>();
            _FOV = GetComponent<FOV>();
            _currentState.EnterState(this);
            _gridVisualiser = GameObject.FindWithTag("Grid").GetComponent<GridVisualiser>();
        }

        /// <summary>
        ///     Runs the UpdateState method of the current state each frame
        /// </summary>
        private void Update()
        {
            _currentState.UpdateState(this);
        }

        public void GetGuardAssignedGridPosition(out int x, out int z)
        {
            x = _guardNavX;
            z = _guardNavZ;
        }

        public void SetGuardAssignedGridPosition(int x, int z)
        {
            _guardNavX = x;
            _guardNavZ = z;
        }

        public void ResetGuardGridPosition()
        {
            _guardNavX = 99; // this isn't in grid space so we can ignore
            _guardNavZ = 99;
        }
        

        /// <summary>
        /// Allows the creation of any coroutine called from other classes
        /// Potentially dangerous, but provides no errors or issues
        /// </summary>
        /// <param name="routine">Coroutine reference</param>
        public void runCoroutine(IEnumerator routine)
        {
            StartCoroutine(routine);
        }

        /// <summary>
        /// Start damage coroutine
        /// </summary>
        public void StartOnDamage()
        {
            StartCoroutine(DealDamage());
        }

        public void SetOnDamaging(bool dmg)
        {
            onDamaging = dmg; // change on damage state
        }

        public void SetDroppingStaleness(bool state)
        {
            _droppingStaleness = state; // change _droppingStaleness state
        }

        public bool GetDroppingStaleness()
        {
            return _droppingStaleness; // get ref to dropping staleness bool
        }

        /// <summary>
        /// Deal damage to the player, one health per second
        /// This is a coroutine, therefore runs on a separate loop to the rest of the game code
        /// </summary>
        /// <returns></returns>
        private IEnumerator DealDamage()
        {
            while (onDamaging)
            {
                Controller.Instance.DamagePlayer();
                yield return new WaitForSeconds(1); // wait for a second
            }
        }

        /// <summary>
        ///     Changes the state to a new state as defined by the received parameter
        /// </summary>
        /// <param name="state">New state to change to</param>
        public void ChangeState(GuardBaseState state)
        {
            _currentState = state;
            _currentState.EnterState(this);
        }

        public bool IsSlave()
        {
            return isSlave;
        }

        public void SetSlave(bool slave)
        {
            isSlave = slave;
        }


        /// <summary>
        ///     Returns the current active state of the agent
        /// </summary>
        /// <returns>Agent's active state</returns>
        public GuardBaseState GetCurrentState()
        {
            return _currentState;
        }

        public void SetGridVisualiser()
        {
            _gridVisualiser = GameObject.FindWithTag("Grid").GetComponent<GridVisualiser>();
        }

        public GridVisualiser GetGridVisualiser()
        {
            return _gridVisualiser;
        }

        public void SetAgentSpeed(float speed)
        {
            agent.speed = speed;
        }
    }
}