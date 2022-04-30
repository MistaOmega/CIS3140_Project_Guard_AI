using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(FOV))]
    public class FOVEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            FOV fov = (FOV)target;
            Handles.color = Color.white;
            Vector3 position = fov.transform.position;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
            Vector3 viewAngleA = fov.DirectionFromAngle(-fov.viewAngle / 2, false);
            Vector3 viewAngleB = fov.DirectionFromAngle(fov.viewAngle / 2, false);

            Handles.DrawLine(position, position + viewAngleA * fov.viewRadius);
            Handles.DrawLine(position, position + viewAngleB * fov.viewRadius);

            Handles.color = Color.red;
            foreach (Transform visibleTarget in fov.visibleTargets)
                Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }
    }
}