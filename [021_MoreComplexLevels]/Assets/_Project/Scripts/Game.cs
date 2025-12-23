using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{
    private const int SAVE_VERSION = 7;

    [SerializeField]
    private ShapeFactory[] _factories;

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

    [SerializeField]
    private float _destroyDuration;

    private List<Shape> _objects;

    private List<ShapeInstance> _killList;

    private List<ShapeInstance> _markAsDyingList;

    private int _loadedLevel;

    private Random.State _randomState;

    private bool _inGameUpdateLoop;

    private int _dyingShapeCount;

    public static Game Instance { get; private set; }

    void OnEnable()
    {
        Instance = this;
        if (_factories[0].FactoryId != 0)
        {
            for (int i = 0; i < _factories.Length; i++)
            {
                _factories[i].FactoryId = i;
            }
        }
    }

    private void Start()
    {
        _randomState = Random.state;
        _objects = new();
        _killList = new();
        _markAsDyingList = new();

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

    private void FixedUpdate()
    {
        _inGameUpdateLoop = true;
        for (int i = 0; i < _objects.Count; i++)
        {
            _objects[i].GameUpdate();
        }
        GameLevel.Current.GameUpdate();
        _inGameUpdateLoop = false;

        if (Input.GetKeyUp(_keyCreateObj))
        {
            GameLevel.Current.SpawnShape();
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

        int limit = GameLevel.Current.PopulationLimit;
        if (limit > 0)
        {
            while (_objects.Count - _dyingShapeCount > limit)
            {
                DestroyObject();
            }
        }

        if (_killList.Count > 0)
        {
            for (int i = 0; i < _killList.Count; i++)
            {
                if (_killList[i].IsValid)
                {
                    KillImmediately(_killList[i].Shape);
                }
            }
            _killList.Clear();
        }

        if (_markAsDyingList.Count > 0)
        {
            for (int i = 0; i < _markAsDyingList.Count; i++)
            {
                if (_markAsDyingList[i].IsValid)
                {
                    MarkAsDyingImmediately(_markAsDyingList[i].Shape);
                }
            }
            _markAsDyingList.Clear();
        }
    }

    public void MarkAsDying(Shape shape)
    {
        if (_inGameUpdateLoop)
        {
            _markAsDyingList.Add(shape);
        }
        else
        {
            MarkAsDyingImmediately(shape);
        }
    }

    public bool IsMarkedAsDying(Shape shape)
    {
        return shape.SaveIndex < _dyingShapeCount;
    }

    private void DestroyObject()
    {
        if (_objects.Count > 0)
        {
            Shape shape = _objects[Random.Range(_dyingShapeCount, _objects.Count)];
            if (_destroyDuration <= 0.0f)
            {
                KillImmediately(shape);
            }
            else
            {
                shape.AddBehavior<ShapeBehaviorDie>().Initialize(shape, _destroyDuration);
            }
        }
    }

    private void MarkAsDyingImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        if (index < _dyingShapeCount)
        {
            return;
        }
        _objects[_dyingShapeCount].SaveIndex = index;
        _objects[index] = _objects[_dyingShapeCount];
        shape.SaveIndex = _dyingShapeCount;
        _objects[_dyingShapeCount++] = shape;
    }

    public void Kill(Shape shape)
    {
        if (_inGameUpdateLoop)
        {
            _killList.Add(shape);
        }
        else
        {
            KillImmediately(shape);
        }
    }

    void KillImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        shape.Recycle();

        if (index < _dyingShapeCount && index < --_dyingShapeCount)
        {
            _objects[_dyingShapeCount].SaveIndex = index;
            _objects[index] = _objects[_dyingShapeCount];
            index = _dyingShapeCount;
        }

        int lastIndex = _objects.Count - 1;
        if (index < lastIndex)
        {
            _objects[lastIndex].SaveIndex = index;
            _objects[index] = _objects[lastIndex];
        }
        _objects.RemoveAt(lastIndex);
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
                _objects[i].Recycle();
            }
            _objects.Clear();
        }
        _dyingShapeCount = 0;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(_objects.Count);
        writer.Write(Random.state);
        writer.Write(_loadedLevel);
        GameLevel.Current.Save(writer);
        for (int i = 0; i < _objects.Count; i++)
        {
            writer.Write(_objects[i].OriginFactory.FactoryId);
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

    private IEnumerator LoadGame(GameDataReader reader)
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
            int factoryId = version >= 5 ? reader.ReadInt() : 0;
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            var o = _factories[factoryId].Get(shapeId, materialId);
            o.Load(reader);
        }
        for (int i = 0; i < _objects.Count; i++)
        {
            _objects[i].ResolveShapeInstances();
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

    public void AddShape(Shape shape)
    {
        shape.SaveIndex = _objects.Count;
        _objects.Add(shape);
    }

    public Shape GetShape(int index)
    {
        return _objects[index];
    }
}
