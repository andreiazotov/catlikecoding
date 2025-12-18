using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{
    private const int SAVE_VERSION = 3;

    [SerializeField]
    private ShapeFactory _factory;

    [SerializeField]
    private PersistentStorage _storage;

    [SerializeField]
    private KeyCode _keyCreateObj = KeyCode.C;

    [SerializeField]
    private KeyCode _keyDestroObj = KeyCode.X;

    [SerializeField]
    private KeyCode _keyNewGame = KeyCode.N;

    [SerializeField]
    private KeyCode _keySave = KeyCode.S;

    [SerializeField]
    private KeyCode _keyLoad = KeyCode.L;

    [SerializeField]
    private int _levelCount;

    [SerializeField]
    private bool _reseedOnLoad;

    private List<Shape> _objects;

    private int _loadedLevel;

    private Random.State _randomState;

    private void Start()
    {
        _randomState = Random.state;
        _objects = new();

        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level"))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    _loadedLevel = loadedScene.buildIndex;
                    return;
                }
            }
        }
        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    private void Update()
    {
        if (Input.GetKeyUp(_keyCreateObj))
        {
            CreateObject();
        }
        else if (Input.GetKeyUp(_keyDestroObj))
        {
            DestroyObject();
        }
        else if (Input.GetKeyUp(_keyNewGame))
        {
            BeginNewGame();
            StartCoroutine(LoadLevel(_loadedLevel));
        }
        else if (Input.GetKeyUp(_keySave))
        {
            _storage.Save(this, SAVE_VERSION);
        }
        else if (Input.GetKeyUp(_keyLoad))
        {
            BeginNewGame();
            _storage.Load(this);
        }
        else
        {
            for (int i = 1; i <= _levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }
    }

    private void CreateObject()
    {
        var o = _factory.GetRandom();
        Transform t = o.transform;
        t.SetLocalPositionAndRotation(GameLevel.Current.SpawnPoint, Random.rotation);
        t.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
        o.SetColor(Random.ColorHSV(0.0f, 1.0f, 0.5f, 1.0f, 0.25f, 1.0f, 1.0f, 1.0f));
        _objects.Add(o);
    }

    private void DestroyObject()
    {
        if (_objects.Count > 0)
        {
            int index = Random.Range(0, _objects.Count);
            _factory.Reclaim(_objects[index]);
            int lastIndex = _objects.Count - 1;
            _objects[index] = _objects[lastIndex];
            _objects.RemoveAt(lastIndex);
        }
    }

    private void BeginNewGame()
    {
        Random.state = _randomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        _randomState = Random.state;
        Random.InitState(seed);

        if (_objects != null && _objects.Count > 0)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _factory.Reclaim(_objects[i]);
            }
            _objects.Clear();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_objects.Count);
        writer.Write(Random.state);
        writer.Write(_loadedLevel);
        GameLevel.Current.Save(writer);
        for (int i = 0; i < _objects.Count; i++)
        {
            writer.Write(_objects[i].ShapeId);
            writer.Write(_objects[i].MaterialId);
            _objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > SAVE_VERSION)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        StartCoroutine(LoadGame(reader));
    }

    IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;
        int count = version <= 0 ? -version : reader.ReadInt();

        if (version >= 3)
        {
            var state = reader.ReadRandomState();
            if (!_reseedOnLoad)
            {
                Random.state = state;
            }
        }

        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
        if (version >= 3)
        {
            GameLevel.Current.Load(reader);
        }
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            var o = _factory.Get(shapeId, materialId);
            o.Load(reader);
            _objects.Add(o);
        }
    }

    private IEnumerator LoadLevel(int level)
    {
        enabled = false;
        if (_loadedLevel > 0)
        {
            yield return SceneManager.UnloadSceneAsync(_loadedLevel);
        }
        yield return SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(level));
        _loadedLevel = level;
        enabled = true;
    }
}
