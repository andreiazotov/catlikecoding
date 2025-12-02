using UnityEngine;

public class Graph : MonoBehaviour // MonoBehaviour extends Behaviour, which extends Component, which extends Object
{
    [SerializeField]
    private Transform _point;

    [SerializeField, Range(10, 100)]
    private int _pointsCounter = 10;

    private Transform[] _points;

    private void Awake()
    {
        // Instantiating a game object is done via the Object.Instantiate method.
        // This is a publicly available method of Unity's Object type, which Graph
        // indirectly inherited by extending MonoBehaviour.
        // The Instantiate method clones whatever Unity object is passed to it as an argument.
        // In the case of a prefab, it will result in an instance being added to the current scene.

        _points = new Transform[_pointsCounter];
        float step = 2.0f / _pointsCounter;
        Vector3 pos = Vector3.zero;
        var scale = Vector3.one * step;
        for (int i = 0; i < _pointsCounter; i++)
        {
            // Because we gave it a reference to a Transform component,
            // that's what we get in return
            Transform p = Instantiate(_point);
            pos.x = (i + 0.5f) * step - 1.0f;
            p.localPosition = pos;
            p.localScale = scale;

            // When a new parent is set Unity will attempt to keep the object at its original
            // world position, rotation, and scale. We don't need this in our case.
            // We can signal this by passing false as a second argument to SetParent.
            p.SetParent(transform, false);

            _points[i] = p;
        }
    }

    private void Update()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            var p = _points[i];
            var pos = p.localPosition;
            pos.y = Mathf.Sin(Mathf.PI * (pos.x + Time.time));
            p.localPosition = pos;
        }
    }
}
