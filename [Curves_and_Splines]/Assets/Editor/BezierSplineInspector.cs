using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    private BezierSpline _spline;
    private Transform _handleTransform;
    private Quaternion _handleRotation;
    private int selectedIndex = -1;

    private const int LINE_STEPS = 30;
    private const float DIRECTION_SCALE = 1.5f;
    private const int STEPS_PER_CURVE = 20;
    private const float HANDLE_SIZE = 0.04f;
    private const float PICK_SIZE = 0.06f;

    private static Color[] s_modeColors =
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    private void OnSceneGUI()
    {
        this._spline = target as BezierSpline;
        this._handleTransform = this._spline.transform;
        this._handleRotation = Tools.pivotRotation == PivotRotation.Local ? this._handleTransform.rotation : Quaternion.identity;

        var p0 = this.ShowPoint(0);
        for (int i = 1; i < this._spline.ControlPointCount; i += 3)
        {
            var p1 = this.ShowPoint(i);
            var p2 = this.ShowPoint(i + 1);
            var p3 = this.ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2.0f);
            p0 = p3;
        }

        this.ShowDirections();
    }

    public override void OnInspectorGUI()
    {
        this._spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", this._spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this._spline, "Toggle Loop");
            EditorUtility.SetDirty(this._spline);
            this._spline.Loop = loop;
        }
        if (this.selectedIndex >= 0 && selectedIndex < this._spline.ControlPointCount)
        {
            this.DrawSelectedPointInspector();
        }
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(this._spline, "Add Curve");
            this._spline.AddCurve();
            EditorUtility.SetDirty(this._spline);
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", this._spline.GetControlPoint(this.selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this._spline, "Move Point");
            EditorUtility.SetDirty(this._spline);
            this._spline.SetControlPoint(this.selectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        var mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", this._spline.GetControlPointMode(this.selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this._spline, "Change Point Mode");
            this._spline.SetControlPointMode(this.selectedIndex, mode);
            EditorUtility.SetDirty(this._spline);
        }
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        var point = this._spline.GetPoint(0.0f);
        Handles.DrawLine(point, point + this._spline.GetDirection(0.0f) * DIRECTION_SCALE);
        int steps = STEPS_PER_CURVE * this._spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            point = this._spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + this._spline.GetDirection(i / (float)steps) * DIRECTION_SCALE);
        }
    }

    private Vector3 ShowPoint(int index)
    {
        var point = this._handleTransform.TransformPoint(this._spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
        {
            size *= 2.0f;
        }
        Handles.color = s_modeColors[(int)this._spline.GetControlPointMode(index)];
        if (Handles.Button(point, this._handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotHandleCap))
        {
            this.selectedIndex = index;
            this.Repaint();
        }
        if (this.selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, this._handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this._spline, "Move Point");
                EditorUtility.SetDirty(this._spline);
                this._spline.SetControlPoint(index, this._handleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }
}
