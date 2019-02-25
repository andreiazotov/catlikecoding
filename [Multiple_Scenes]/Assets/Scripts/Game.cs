using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Shape
{
    private const int SAVE_VERSION = 2;

    public ShapeFactory shapeFactory;
    public PersistentStorage storage;
    public KeyCode createKey = KeyCode.C;
    public KeyCode destroyKey = KeyCode.X;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public int levelCount;

    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }

    private List<Shape> _shapes;
    private float _creationProgress;
    private float _destructionProgress;
    private int _loadedLevelIndex;

    private void Start() {
        _shapes = new List<Shape>();

        if (Application.isEditor) {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                var loadedLevel = SceneManager.GetSceneAt(i);
                if (loadedLevel.name.Contains("level")) {
                    SceneManager.SetActiveScene(loadedLevel);
                    _loadedLevelIndex = loadedLevel.buildIndex;
                    return;
                }
            }
        }

        StartCoroutine(LoadLevel(1));
    }

    private void Update() {
        if (Input.GetKeyDown(createKey)) {
            CreateShape();
        } else if (Input.GetKeyDown(newGameKey)) {
            BeginNewGame();
        } else if (Input.GetKeyDown(saveKey)) {
            storage.Save(this, SAVE_VERSION);
        } else if (Input.GetKeyDown(loadKey)) {
            BeginNewGame();
            storage.Load(this);
        } else if (Input.GetKeyDown(destroyKey)) {
            DestroyShape();
        } else {
            for (int i = 1; i <= levelCount; i++) {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i)) {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

        _creationProgress += Time.deltaTime * CreationSpeed;
        while (_creationProgress >= 1.0f) {
            _creationProgress -= 1.0f;
            CreateShape();
        }

        _destructionProgress += Time.deltaTime * DestructionSpeed;
        while (_destructionProgress >= 1.0f) {
            _destructionProgress -= 1.0f;
            DestroyShape();
        }
    }

    private void DestroyShape() {
        if (_shapes.Count > 0) {
            int index = Random.Range(0, _shapes.Count);
            shapeFactory.Reclaim(_shapes[index]);
            int lastIndex = _shapes.Count - 1;
            _shapes[index] = _shapes[lastIndex];
            _shapes.RemoveAt(lastIndex);
        }
    }

    private void CreateShape() {
        var instance = shapeFactory.GetRandom();
        var t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5.0f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
        instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        _shapes.Add(instance);
    }

    private void BeginNewGame() {
        for (int i = 0; i < _shapes.Count; i++) {
            shapeFactory.Reclaim(_shapes[i]);
        }
        _shapes.Clear();
    }

    private IEnumerator LoadLevel(int loadLevelIndex) {
        enabled = false;
        if (_loadedLevelIndex > 0) {
            yield return SceneManager.UnloadSceneAsync(_loadedLevelIndex);
        }
        yield return SceneManager.LoadSceneAsync(loadLevelIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(loadLevelIndex));
        _loadedLevelIndex = loadLevelIndex;
        enabled = true;
    }

    public override void Save(GameDataWriter writer) {
        writer.Write(_shapes.Count);
        writer.Write(_loadedLevelIndex);
        for (int i = 0; i < _shapes.Count; i++) {
            writer.Write(_shapes[i].ShapeId);
            writer.Write(_shapes[i].MaterialId);
            _shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader) {
        int version = reader.Version;

        if (version > SAVE_VERSION) {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }

        int count = version <= 0 ? -version : reader.ReadInt();
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
        for (int i = 0; i < count; i++) {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            var instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            _shapes.Add(instance);
        }
    }
}
