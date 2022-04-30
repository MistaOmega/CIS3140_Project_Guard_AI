using System;
using AI;
using UnityEngine;
using Utilities.UI;
using Object = UnityEngine.Object;

namespace worldspace
{
    public class GridObject
    {
        private readonly int[,] _gridArray;
        private readonly int _height;
        public readonly float _sz;
        private readonly int _width;
        private readonly TextMesh[,] textArray;

        public GridObject(int width, int height, float sz)
        {
            _width = width;
            _height = height;
            _sz = sz;

            _gridArray = new int[_width, _height];
            textArray = new TextMesh[_width, _height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                textArray[x, z] = UIUtils.CreateWorldText(_gridArray[x, z].ToString(), null,
                    GetWorldPosition(x, z) + new Vector3(_sz, 0, _sz) * .5f, 20,
                    Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.cyan, 100);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.cyan, 100);
            }

            //SetValueOfCell(26, 2, 1);
        }
        
        /// <summary>
        /// This is used to add a value to all cells one after the other as opposed to individually
        /// Includes a system for adding double value to the grid region where the valuable is, to ensure guards check the area more often
        /// This can be extended to include multiple valuables using a list of Object.FindObjectsOfType, but is not required for the scope of this project
        /// </summary>
        /// <param name="value">Value to add</param>
        public void AddValueToAllCells(int value)
        {
            Valuable v = (Valuable)Object.FindObjectOfType(typeof(Valuable));
            int valuable_x;
            int valuable_z;
            GetXZFromVec(v.transform.position, out valuable_x, out valuable_z);
            
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                if (valuable_x == x && valuable_z == z)
                {
                    _gridArray[x, z] += 2*value;
                }
                _gridArray[x, z] += value;
                textArray[x, z].text = _gridArray[x, z].ToString();
            }
        }

        /// <summary>
        /// Gets the largest value weight for a guard to travel to next and delivers a position vector at the center of the grid cell
        ///
        /// The highest value is what determines the next place for the guard to go
        ///
        /// Guards will not enter the same cell region as one another
        /// </summary>
        /// <param name="origin">Guard's current position vector</param>
        /// <param name="distanceMag">How much to scale the distance as a parameter</param>
        /// <returns>Next position vector for the guard to travel to.</returns>
        public Vector3 GetNextGridLocation(Vector3 origin, float distanceMag)
        {
            GuardManagerHolder gmh = GuardManagerHolder.Instance; // This is so we can check current guard cell allocations
            int m = 0, n = 0;
            float highestVal = Mathf.NegativeInfinity; // we need this to define the next movable area. If this wasn't a very low negative we'd have issues later with negative value weights.
            
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < _gridArray.GetLength(1); z++)
                {
                    bool isAlreadyGuarded = false;
                    foreach (GuardStateMan guard in gmh.guards)
                    {
                        int guardx, guardz;
                        guard.GetGuardAssignedGridPosition(out guardx, out guardz);
                        if (z == guardz && x == guardx)
                        {
                            isAlreadyGuarded = true;
                        }
                    }

                    if (isAlreadyGuarded)
                    {
                        continue;
                    }
                    float value = _gridArray[x, z];
                    

                    Vector3 pos = GetWorldPosition(x, z);
                    value -= Vector3.Distance(origin, pos) * distanceMag;
                    
                    Debug.Log($"X: {x}, Z{z} VALUE: {value}");
                    if (!(value > highestVal)) continue;
                    Debug.Log($"Highest Value {highestVal} Current Value {value} REPLACING NOW");
                    highestVal = value;
                    m = x;
                    n = z; 
                    //textArray[x, z].text = _gridArray[x, z].ToString();
                }
            }
            Debug.Log($"RETURN GRID = {m}, {n}");

            return GetWorldPosition(m, n, origin.y);
        }

        public Vector3 GetWorldPosition(int x, int z, float y = 1)
        {
            Vector3 finalVec = new Vector3(x, y, z) * _sz;
            
            return new Vector3(finalVec.x, y, finalVec.z);
        }

        public void GetXZFromVec(Vector3 worldPos, out int x, out int z)
        {
            x = Mathf.FloorToInt(worldPos.x / _sz);
            z = Mathf.FloorToInt(worldPos.z / _sz);
        }

        public void SetValueOfCell(int value, int x, int z)
        {
            if (x < 0 || z < 0 || x >= _width || z >= _height) return;
            _gridArray[x, z] = value;
            textArray[x, z].text = _gridArray[x, z].ToString();
        }

        public void SetValueOfCell(Vector3 worldPos, int value)
        {
            int x, z;
            GetXZFromVec(worldPos, out x, out z);
            SetValueOfCell(x, z, value);
        }

        public void ModifyValueAtCell(Vector3 worldPos, int value)
        {
            //Debug.Log($"REQUEST MADE FROM {worldPos}");
            int x, z;
            GetXZFromVec(worldPos, out x, out z);
            ModifyValueAtCell(x, z, value);
        }
        public void ModifyValueAtCell(int x, int z, int value)
        {
            Valuable v = (Valuable)Object.FindObjectOfType(typeof(Valuable));
            int valuable_x;
            int valuable_z;
            GetXZFromVec(v.transform.position, out valuable_x, out valuable_z);
            if (x < 0 || z < 0 || x >= _width || z >= _height) return;
            if(x == valuable_x && z == valuable_z)
            {
                _gridArray[x, z] += 2*value;
            }
            _gridArray[x, z] += value;
            if (_gridArray[x, z] <= 0)
            {
                _gridArray[x, z] = 0;
            }
            textArray[x, z].text = _gridArray[x, z].ToString();
        }
        

        public int[,] GetGridArray()
        {
            return _gridArray;
        }
        
        
    }
}