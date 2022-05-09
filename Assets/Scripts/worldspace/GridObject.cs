using System.Collections.Generic;
using AI;
using GameLogic;
using UnityEngine;
using Utilities.UI;

namespace worldspace
{
    public class GridObject
    {
        private readonly int[,] _gridArray;
        private readonly int _height;
        public readonly float Sz;
        private readonly int _width;
        private readonly TextMesh[,] _textArray;

        public GridObject(int width, int height, float sz)
        {
            _width = width;
            _height = height;
            Sz = sz;

            _gridArray = new int[_width, _height];
            _textArray = new TextMesh[_width, _height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                _textArray[x, z] = UIUtils.CreateWorldText(_gridArray[x, z].ToString(), null,
                    GetWorldPosition(x, z) + new Vector3(Sz, 0, Sz) * .5f, 20,
                    Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.cyan, 100);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.cyan, 100);
            }

        }

        /// <summary>
        ///     This is used to add a value to all cells one after the other as opposed to individually
        ///     Includes a system for adding double value to the grid region where the valuable is, to ensure guards check the area
        ///     more often
        ///     This has been extended to include multiple XY co-ordinates to allow for multiple exits to be flagged, etc.
        /// </summary>
        /// <param name="value">Value to add</param>
        public void AddValueToAllCells(int value)
        {
            List<Vector2> XYPairs = new List<Vector2>();
            Valuable v = (Valuable)Object.FindObjectOfType(typeof(Valuable));
            if (v != null)
            {
                Vector2 valuableVec = GetXZFromVec(v.transform.position);
                if (!XYPairs.Contains(valuableVec)) XYPairs.Add(valuableVec);
            }

            ExitScript[] exits = (ExitScript[])Object.FindObjectsOfType(typeof(ExitScript));
            foreach (ExitScript exit in exits)
            {
                Vector2 exitVec = GetXZFromVec(exit.transform.position);
                if (!XYPairs.Contains(exitVec)) XYPairs.Add(exitVec);
            }

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                bool isDoubled = false;
                foreach (Vector2 vec in XYPairs)
                {
                    if ((int)vec.x != x || (int)vec.y != z) continue;
                    isDoubled = true;
                    _gridArray[x, z] += 3 * value;
                }

                if (isDoubled)
                {
                    _textArray[x, z].text = _gridArray[x, z].ToString();
                    continue;
                }

                _gridArray[x, z] += value;
                _textArray[x, z].text = _gridArray[x, z].ToString();
            }
        }

        /// <summary>
        ///     Gets the largest value weight for a guard to travel to next and delivers a position vector at the center of the
        ///     grid cell
        ///     The highest value is what determines the next place for the guard to go
        ///     Guards will not enter the same cell region as one another
        /// </summary>
        /// <param name="origin">Guard's current position vector</param>
        /// <param name="distanceMag">How much to scale the distance as a parameter</param>
        /// <returns>Next position vector for the guard to travel to.</returns>
        public Vector3 GetNextGridLocation(Vector3 origin, float distanceMag)
        {
            GuardManagerHolder
                gmh = GuardManagerHolder.Instance; // This is so we can check current guard cell allocations
            int m = 0, n = 0;
            
            // we need this to define the next movable area. If this wasn't a very low negative we'd have issues later with negative value weights.
            float highestVal = Mathf.NegativeInfinity; 

            for (int x = 0; x < _gridArray.GetLength(0); x++) // iterate through every grid cell
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                bool isAlreadyGuarded = false; // set to not guarded
                // Check if the region is guarded
                foreach (GuardStateMan guard in gmh.guards)
                {
                    int guardx, guardz;
                    guard.GetGuardAssignedGridPosition(out guardx, out guardz);
                    if (z == guardz && x == guardx) isAlreadyGuarded = true;
                }

                if (isAlreadyGuarded) continue; // ignore guarded regions
                float value = _gridArray[x, z]; // Get the value of the cell


                Vector3 pos = GetWorldPosition(x, z);
                value -= Vector3.Distance(origin, pos) * distanceMag; // Scale back the value by a factor of the distance

                if (!(value > highestVal)) continue; // Ignore cells lower in value than the highest
                highestVal = value; // assign higher values
                m = x;
                n = z;
                //textArray[x, z].text = _gridArray[x, z].ToString();
            }

            return GetWorldPosition(m, n, origin.y); // Convert cell position to center region of cell and return vector
        }

        public Vector3 GetWorldPosition(int x, int z, float y = 1)
        {
            Vector3 finalVec = new Vector3(x, y, z) * Sz;

            return new Vector3(finalVec.x, y, finalVec.z);
        }

        public void GetXZFromVec(Vector3 worldPos, out int x, out int z)
        {
            x = Mathf.FloorToInt(worldPos.x / Sz);
            z = Mathf.FloorToInt(worldPos.z / Sz);
        }

        public Vector2 GetXZFromVec(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(worldPos.x / Sz);
            int z = Mathf.FloorToInt(worldPos.z / Sz);
            return new Vector2(x, z);
        }

        public void SetValueOfCell(int value, int x, int z)
        {
            if (x < 0 || z < 0 || x >= _width || z >= _height) return;
            _gridArray[x, z] = value;
            _textArray[x, z].text = _gridArray[x, z].ToString();
        }

        public void SetValueOfCell(Vector3 worldPos, int value)
        {
            int x, z;
            GetXZFromVec(worldPos, out x, out z);
            SetValueOfCell(x, z, value);
        }

        public void ModifyValueAtCell(Vector3 worldPos, int value)
        {
            int x, z;
            GetXZFromVec(worldPos, out x, out z);
            ModifyValueAtCell(x, z, value);
        }

        private void ModifyValueAtCell(int x, int z, int value)
        {
            List<Vector2> XYPairs = new List<Vector2>();
            Valuable v = (Valuable)Object.FindObjectOfType(typeof(Valuable));
            if (v != null)
            {
                Vector2 valuableVec = GetXZFromVec(v.transform.position);
                if (!XYPairs.Contains(valuableVec)) XYPairs.Add(valuableVec);
            }

            ExitScript[] exits = (ExitScript[])Object.FindObjectsOfType(typeof(ExitScript));
            foreach (ExitScript exit in exits)
            {
                Vector2 exitVec = GetXZFromVec(exit.transform.position);
                if (!XYPairs.Contains(exitVec)) XYPairs.Add(exitVec);
            }

            if (x < 0 || z < 0 || x >= _width || z >= _height) return;
            foreach (Vector2 vec in XYPairs)
            {
                if (x != (int)vec.x || z != (int)vec.y || v == null) continue;
                _gridArray[x, z] += 3 * value;
                if (_gridArray[x, z] <= 0) _gridArray[x, z] = 0;
                _textArray[x, z].text = _gridArray[x, z].ToString();
                return;
            }

            _gridArray[x, z] += value;
            if (_gridArray[x, z] <= 0) _gridArray[x, z] = 0;
            _textArray[x, z].text = _gridArray[x, z].ToString();
        }
        

        public int[,] GetGridArray()
        {
            return _gridArray;
        }
    }
}