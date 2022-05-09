using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.GuardStates;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public float viewRadius;

    [Range(0, 360)] public float viewAngle;

    public List<Transform> visibleTargets = new List<Transform>();
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public GuardStateMan _GuardStateMan;

    private float guardStateTimer;

    private void Start()
    {
        _GuardStateMan = GetComponent<GuardStateMan>();
        StartCoroutine(FindTargetsWithDelay(.2f));
    }


    /// <summary>
    ///     IEnumerator to find delay the FindVisibleTargets method
    /// </summary>
    /// <param name="delay"> Delay between execution of method</param>
    /// <returns></returns>
    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            visibleTargets = FindVisibleTargets();
            //Debug.Log($"Count = {visibleTargets.Count}");

            /* checking to see if the visible targets are present. In this case the target is the thief and there should only be one.
             * However this system will attempt to account for multiple by attacking the closest target. If this works or not, who knows.
             */
            if (visibleTargets == null) continue;
            if (visibleTargets.Count > 0 && _GuardStateMan.GetCurrentState().GetType() != typeof(GuardAttackState))
            {
                guardStateTimer = 0;
                _GuardStateMan.SetDroppingStaleness(false);
                _GuardStateMan.ResetGuardGridPosition();
                _GuardStateMan.ChangeState(_GuardStateMan._guardAttackState);
            }
            else if (!visibleTargets.Any() &&
                     _GuardStateMan.GetCurrentState().GetType() == typeof(GuardAttackState))
            {
                if (Math.Abs(guardStateTimer - 10f) < .5f)
                {
                    guardStateTimer = 0;
                    _GuardStateMan.SetSlave(false);
                    _GuardStateMan.ChangeState(_GuardStateMan._GuardPatrolStateWithStaleness);
                }

                guardStateTimer += Time.deltaTime + delay;
                //Debug.Log($"Timer = {guardStateTimer}");
            }
        }


        // ReSharper disable once IteratorNeverReturns
    }

    /// <summary>
    ///     Finds targets within FOV region
    /// </summary>
    public List<Transform> FindVisibleTargets()
    {
        List<Transform> targets = new List<Transform>();
        targets.Clear();
        Collider[] targetsInViewRadius =
            Physics.OverlapSphere(transform.position, viewRadius, targetMask); // find all targets in view radius circle
        targets.AddRange(targetsInViewRadius.Select(c => c.transform)
            .Select(target => new { target, dirToTarget = (target.position - transform.position).normalized })
            . // get targets in range
            Where(t => Vector3.Angle(transform.forward, t.dirToTarget) < viewAngle / 2)
            . // where they fall into the FOV cone
            Select(t => new { t, distToTarget = Vector3.Distance(transform.position, t.target.position) })
            . // select targets within that selection
            Where(t => !Physics.Raycast(transform.position, t.t.dirToTarget, t.distToTarget, obstacleMask))
            . // where a raycast can hit
            Select(t => t.t.target)); // select those into the list

        return targets;
    }

    /// <summary>
    ///     Bad idea? Maybe.
    ///     Returns the list of current visible targets, updated when "FindVisibleTargets" is called.
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetVisibleTargets()
    {
        return visibleTargets;
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlob)
    {
        if (!angleIsGlob) angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}