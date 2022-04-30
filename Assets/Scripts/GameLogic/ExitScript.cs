using UnityEngine;

namespace GameLogic
{
    public class ExitScript : MonoBehaviour
    {
        private TesterScript _testerScript;
        public static ExitScript Instance { get; private set; }

        private void Start()
        {
            Instance = this;
            _testerScript = TesterScript.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) End();
        }

        public void End()
        {
            Debug.Log("========================================== \n" +
                      "                               RESULTS             \n" +
                      "========================================== \n" +
                      $"  TOTAL TIME TAKEN: {_testerScript.escapeTimer} \n" +
                      $"  DETECTED BY GUARD: {_testerScript.hasDetectedThief} \n" +
                      $"  HAD VALUABLE WHEN DETECTED: {_testerScript.hasDetectedThiefWithValuable} \n" +
                      $"  THIEF COLLECTED VALUABLE: {_testerScript.thiefHasCollectedValuable} \n" +
                      $"  REMAINING PLAYER HEALTH: {Controller.Instance.health} \n" +
                      $"  TIME TO CAPTURE AFTER DETECTION: {_testerScript.detectionToCaptureTimer} \n" +
                      "   AVERAGE NODE STALENESS: 0"); // todo add later
        }
    }
}