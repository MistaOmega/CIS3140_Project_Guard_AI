/*
 * UI Based common utilities for Unity 
 */

using UnityEngine;

namespace Utilities.UI
{
    public class UIUtils
    {
        public void SetCursorConfined()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void SetCursorLocked()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void SetCursorUnlocked()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default,
            int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft,
            TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
        {
            color ??= Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment,
                sortingOrder);
        }


        // Create Text in the World
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize,
            Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            gameObject.transform.Rotate(90, 0, 0);
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }
    }
}