using UnityEngine;
using UnityEngine.SceneManagement;

namespace worldspace
{
    public class GridVisualiser : MonoBehaviour
    {
        // CHANGE THIS PLEASE//
        private int _instanceWidth = 3; // 3 for scene 1, 6 for scene 2. This WILL NOT WORK IF NOT SET PROPERLY
        //                  //


        private GridObject _gridObject;
        public static GridVisualiser Instance { get; private set; }

        /// <summary>
        /// Runs on object creation
        /// </summary>
        private void Awake()
        {
            //Changes the size of the grid dependant on scene :) 
            if (SceneManager.GetActiveScene().name.Equals("Scenario2")) // Hopefully will work.
            {
                _instanceWidth = 6;
            }
            _gridObject = new GridObject(_instanceWidth, 3, 25f);
            Instance = this;
        }

        public GridObject GetGridObject()
        {
            return _gridObject;
        }
    }
}