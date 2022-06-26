using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FibonacciSphere : MonoBehaviour
{
    [Range(0, 11111)] public int count = 1111;
    [Range(0, 100f)] public float radius = 1f;
    public Color color = Color.green;
    [HideInInspector] public float min = 0f;
    [HideInInspector] public float max = 1f;

    [Range(-180, 180)]
    public float angleStart = 0f;

    [Range(0, 360)]
    public float angleRange = 360f;

    public enum Direction { Up, Forward, Left, Right, Back, Down }

    public Direction direction = Direction.Up;

    /// <summary> -- golden angle in radians -- </summary>
    static float Phi = Mathf.PI * (3f - Mathf.Sqrt(5f));

    static float Pi2 = Mathf.PI * 2;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        for (var i = 0; i < count; ++i)
        {
            var _p = Point(radius, i, count, min, max, angleStart, angleRange);

            if (direction != Direction.Up) _p = Shift(_p, direction);

            Gizmos.DrawSphere(transform.position + _p, 0.04f);
        }

    }
    public static Vector3 Point(float radius, int index, int total, float min = 0f, float max = 1f, float angleStartDeg = 0f, float angleRangeDeg = 360)
    {
      
        var y = ((index / (total - 1f)) * (max - min) + min) * 2f - 1f;


        // y goes from 1 to -1
        //var y = 1f - ( i / ( count - 1f ) ) * 2f; 

        // radius at y
        var rY = Mathf.Sqrt(1 - y * y);

        // golden angle increment
        var theta = Phi * index;

        if (angleStartDeg != 0 || angleRangeDeg != 360)
        {
            theta = (theta % (Pi2));
            theta = theta < 0 ? theta + Pi2 : theta;

            var a1 = angleStartDeg * Mathf.Deg2Rad;
            var a2 = angleRangeDeg * Mathf.Deg2Rad;

            theta = theta * a2 / Pi2 + a1;
        }


        var x = Mathf.Cos(theta) * rY;
        var z = Mathf.Sin(theta) * rY;

        return new Vector3(x, y, z) * radius;
    }

    public static Vector3 Shift(Vector3 p, Direction direction = Direction.Up)
    {
        switch (direction)
        {
            default:
            case Direction.Up: return new Vector3(p.x, p.y, p.z);
            case Direction.Right: return new Vector3(p.y, p.x, p.z);
            case Direction.Left: return new Vector3(-p.y, p.x, p.z);
            case Direction.Down: return new Vector3(p.x, -p.y, p.z);
            case Direction.Forward: return new Vector3(p.x, p.z, p.y);
            case Direction.Back: return new Vector3(p.x, p.z, -p.y);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FibonacciSphere))]
    public class FibonacciSphereEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This MonoBehavious is used for gizmo visualization only , " +
                "use the static methods of the FibonacciSphere class instead", MessageType.None);

            GUILayout.Space(30);

            base.OnInspectorGUI();

            // Draw min max slider 

            var min = serializedObject.FindProperty("min");
            var max = serializedObject.FindProperty("max");

            var minVal = min.floatValue;
            var maxVal = max.floatValue;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.MinMaxSlider("Min Max", ref minVal, ref maxVal, 0f, 1f);

            if (EditorGUI.EndChangeCheck())
            {
                min.floatValue = minVal;
                max.floatValue = maxVal;

            }

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}