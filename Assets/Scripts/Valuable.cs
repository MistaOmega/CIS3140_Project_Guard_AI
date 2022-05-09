using GameLogic;
using UnityEngine;

public class Valuable : MonoBehaviour
{
    /// <summary>
    /// OnTriggerEnter function to allow thief to pick up valuable
    /// Signals tester script to flag a valuable as collected
    /// </summary>
    /// <param name="other">Other collider that interacted with trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Valuable collected");
        TesterScript.Instance.RegisterValuableCollected();
        Destroy(gameObject);
    }
}