using AI;
using UnityEngine;

namespace worldspace
{
    public class GridVisualiser : MonoBehaviour
    {
        private GridObject _gridObject;
        public static GridVisualiser Instance { get; private set; }

        private void Awake()
        {
            _gridObject = new GridObject(6, 3, 25f);
            Instance = this;
            
        }

        public GridObject GetGridObject()
        {
            return _gridObject;
        }
    }
}