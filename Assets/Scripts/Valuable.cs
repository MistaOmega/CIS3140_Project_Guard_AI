using GameLogic;
using UnityEngine;

public class Valuable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Valuable collected");
        TesterScript.Instance.RegisterValuableCollected();
        Destroy(gameObject);
    }
}