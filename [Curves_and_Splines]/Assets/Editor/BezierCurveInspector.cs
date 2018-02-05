using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
    private BezierCurve _curve;
    private Transform _handleTransform;
    private Quaternion _handleRotation;

    private const int LINE_STEPS = 30;
    private const float DIRECTION_SCALE = 1.5f;

    private void OnSceneGUI()
    {
        this._curve = target as BezierCurve;
        this._handleTransform = this._curve.transform;
        this._handleRotation = Tools.pivotRotation == PivotRotation.Local ? this._handleTransform.rotation : Quaternion.identity;

        var p0 = this.ShowPoint(0);
        var p1 = this.ShowPoint(1);
        var p2 = this.ShowPoint(2);
        var p3 = this.ShowPoint(3);

        Handles.color = Color.gray;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);

        this.ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2.0f);
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        var point = this._curve.GetPoint(0.0f);
        Handles.DrawLine(point, point + this._curve.GetDirection(0.0f) * DIRECTION_SCALE);
        for (int i = 1; i <= LINE_STEPS; i++)
        {
            point = this._curve.GetPoint(i / (float)LINE_STEPS);
            Handles.DrawLine(point, point + _curve.GetDirection(i / (float)LINE_STEPS) * DIRECTION_SCALE);
        }
    }


    private Vector3 ShowPoint(int index)
    {
        var point = this._handleTransform.TransformPoint(this._curve.points[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, this._handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this._curve, "Move Point");
            EditorUtility.SetDirty(this._curve);
            this._curve.points[index] = this._handleTransform.InverseTransformPoint(point);
        }
        return point;
    }
}
