using System.Collections;
using System.Linq;
using UnityEngine;
using worldspace;

namespace GameLogic
{
    /// <summary>
    ///     Singleton class.
    ///     TesterScript is the script for managing the testing phase of the program
    ///     It keeps track of variables that are modified when certain conditions are met. It also keeps track of how long the
    ///     game has ran for.
    /// </summary>
    public class TesterScript : MonoBehaviour
    {
        private static TesterScript _instance;
        public bool hasDetectedThief;
        public bool hasDetectedThiefWithValuable;
        public bool hasCapturedThief;

        public bool thiefHasCollectedValuable;

        public float escapeTimer;
        public float detectionToCaptureTimer;

        public bool isCoroutineRunning;
        public bool isGamePlaying;
        private GridObject _gridObject;

        public static TesterScript Instance { get; private set; }


        private void Start()
        {
            if (Instance == null) Instance = GameObject.FindWithTag("Manager").GetComponent<TesterScript>();
            GridVisualiser gridVisualiser = GridVisualiser.Instance;
            _gridObject = gridVisualiser.GetGridObject();

            if (!isCoroutineRunning) StartCoroutine(EnumeratorNodeStaleness(1f));
        }


        // Update is called once per frame
        private void Update()
        {
            if (Instance == null) Instance = GameObject.FindWithTag("Manager").GetComponent<TesterScript>();
            escapeTimer += Time.deltaTime;
            if (hasDetectedThief) detectionToCaptureTimer += Time.deltaTime;
        }

        private IEnumerator EnumeratorNodeStaleness(float delay)
        {
            isCoroutineRunning = true;
            while (isCoroutineRunning)
            {
                yield return new WaitForSeconds(delay);
                UpdateNodeStaleness();
            }
        }

        public float ComputeCellAverages()
        {
            int[,] arr = _gridObject.GetGridArray();
            float total = arr.Cast<int>().Aggregate<int, float>(0, (current, val) => current + val);
            return total / (arr.GetLength(0) * arr.GetLength(1));
        }

        private void UpdateNodeStaleness()
        {
            _gridObject.AddValueToAllCells(1);
        }

        public void DetectThief()
        {
            if (hasDetectedThief) return; // force run once, when it's called again skip.
            if (thiefHasCollectedValuable) hasDetectedThiefWithValuable = true;
            hasDetectedThief = true;
        }

        public void RegisterValuableCollected()
        {
            thiefHasCollectedValuable = true;
        }
    }
}