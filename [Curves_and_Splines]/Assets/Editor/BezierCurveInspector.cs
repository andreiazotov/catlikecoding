using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
    private BezierCurve _curve;
    private Transform _handleTransform;
    private Quaternion _handleRotation;

    private void OnSceneGUI()
    {
        this._curve = target as BezierCurve;
        this._handleTransform = this._curve.transform;
        this._handleRotation = Tools.pivotRotation == PivotRotation.Local ? this._handleTransform.rotation : Quaternion.identity;

        var p0 = this.ShowPoint(0);
        var p1 = this.ShowPoint(1);
        var p2 = this.ShowPoint(2);

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p1, p2);
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
