using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : Shape
{
    private const int SAVE_VERSION = 4;

    [SerializeField]
    private Slider creationSpeedSlider;
    [SerializeField]
    private Slider destructionSpeedSlider;
    [SerializeField]
    private ShapeFactory shapeFactory;
    [SerializeField]
    private PersistentStorage storage;
    [SerializeField]
    private KeyCode createKey = KeyCode.C;
    [SerializeField]
    private KeyCode destroyKey = KeyCode.X;
    [SerializeField]
    private KeyCode newGameKey = KeyCode.N;
    [SerializeField]
    private KeyCode saveKey = KeyCode.S;
    [SerializeField]
    private KeyCode loadKey = KeyCode.L;
    [SerializeField]
    private int levelCount;
    [SerializeField]
    private bool reseedOnLoad;

    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }

    private List<Shape> _shapes;
    private float _creationProgress;
    private float _destructionProgress;
    private int _loadedLevelIndex;
    private Random.State _mainRandomState;

    private void Start() {
        _mainRandomState = Random.state;
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
        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    private void Update() {
        if (Input.GetKeyDown(createKey)) {
            CreateShape();
        } else if (Input.GetKeyDown(newGameKey)) {
            BeginNewGame();
            StartCoroutine(LoadLevel(_loadedLevelIndex));
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
    }

    private void FixedUpdate() {
        for (int i = 0; i < _shapes.Count; i++) {
            _shapes[i].GameUpdate();
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
        GameLevel.Current.ConfigureSpawn(instance);
        _shapes.Add(instance);
    }

    private void BeginNewGame() {
        Random.state = _mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledDeltaTime;
        _mainRandomState = Random.state;
        Random.InitState(seed);
        creationSpeedSlider.value = CreationSpeed = 0.0f;
        destructionSpeedSlider.value = DestructionSpeed = 0.0f;
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
        writer.Write(Random.state);
        writer.Write(CreationSpeed);
        writer.Write(_creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(_destructionProgress);
        writer.Write(_loadedLevelIndex);
        GameLevel.Current.Save(writer);
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
        StartCoroutine(LoadGame(reader));
    }

    private IEnumerator LoadGame(GameDataReader reader) {
        int version = reader.Version;
        int count = version <= 0 ? -version : reader.ReadInt();

        if (version >= 3) {
            var state = reader.ReadRandomState();
            if (reseedOnLoad) {
                Random.state = state;
            }
            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            _creationProgress = reader.ReadFloat();
            DestructionSpeed = reader.ReadFloat();
            _destructionProgress = reader.ReadFloat();
        }

        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
        if (version >= 3) {
            GameLevel.Current.Load(reader);
        }
        for (int i = 0; i < count; i++) {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            var instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            _shapes.Add(instance);
        }
    }
}
