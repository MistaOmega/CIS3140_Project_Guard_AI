using UnityEngine;

namespace GameLogic
{
    public class ExitScript : MonoBehaviour
    {
        private TesterScript _testerScript;
        public static ExitScript Instance { get; private set; }

        private void Awake()
        {
            Instance = (ExitScript)FindObjectOfType(typeof(ExitScript));
            _testerScript = TesterScript.Instance;
        }

        private void Update()
        {
            if (_testerScript == null) _testerScript = TesterScript.Instance;
            if (Instance != null) return;
            Instance = (ExitScript)FindObjectOfType(typeof(ExitScript));
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
                      $"  AVERAGE NODE STALENESS: {_testerScript.ComputeCellAverages()}");
            
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}