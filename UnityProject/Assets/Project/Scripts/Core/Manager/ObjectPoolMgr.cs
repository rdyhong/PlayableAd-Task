using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolingObject
{
    void OnSpawn();
    void OnRecycle();
}
public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
{
    private Transform _parent = null;
    public Dictionary<string, List<GameObject>> _objPool = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, List<GameObject>> _objPoolUsing = new Dictionary<string, List<GameObject>>();

    // Photon 전용 풀 (네트워크 오브젝트)
    private readonly Dictionary<string, Queue<GameObject>> _photonPool = new Dictionary<string, Queue<GameObject>>();

    // Photon 풀에서 사용 중인 오브젝트 추적
    private readonly Dictionary<string, List<GameObject>> _photonPoolUsing = new Dictionary<string, List<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        _parent = new GameObject("WholePoolingObject").transform;
        _parent.SetParent(transform);
    }

    #region ===== Photon Instantiate 풀링 =====
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;

        if (_photonPool.TryGetValue(prefabId, out var queue) && queue.Count > 0)
        {
            obj = queue.Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            Debug.Log("Spawnd in Pool");
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>(prefabId);
            obj = Object.Instantiate(prefab, position, rotation);
            
            Debug.Log("Spawn new");
        }

        obj.name = prefabId;//.Replace("(Clone)", "").Trim();

        // Photon 사용 중인 오브젝트 추적에 추가
        if (!_photonPoolUsing.ContainsKey(prefabId))
        {
            _photonPoolUsing[prefabId] = new List<GameObject>();
        }
        _photonPoolUsing[prefabId].Add(obj);

        var poolingObj = obj.GetComponent<IPoolingObject>();
        poolingObj?.OnSpawn();

        return obj;
    }

    public void Destroy(GameObject gameObject)
    {
        if (gameObject == null) return;

        string prefabId = gameObject.name.Replace("(Clone)", "").Trim();

        // 사용 중인 목록에서 제거
        if (_photonPoolUsing.ContainsKey(prefabId) && _photonPoolUsing[prefabId].Contains(gameObject))
        {
            _photonPoolUsing[prefabId].Remove(gameObject);
        }

        if (!_photonPool.ContainsKey(prefabId))
            _photonPool[prefabId] = new Queue<GameObject>();

        gameObject.SetActive(false);
        gameObject.transform.SetParent(_parent);
        gameObject.transform.position = new Vector3(0, -1000, 0); // 위치 초기화

        var poolingObj = gameObject.GetComponent<IPoolingObject>();
        poolingObj?.OnRecycle();

        _photonPool[prefabId].Enqueue(gameObject);
    }

    public void PreloadPhoton(string prefabId, int count)
    {
        if (!_photonPool.ContainsKey(prefabId))
            _photonPool[prefabId] = new Queue<GameObject>();

        GameObject prefab = Resources.Load<GameObject>(prefabId);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.name = prefabId;
            obj.SetActive(false);
            obj.transform.SetParent(_parent);
            _photonPool[prefabId].Enqueue(obj);
        }
    }
    #endregion

    #region ===== 전체 회수 기능 =====

    public void RecycleAll(bool onGameOver = false)
    {
        RecycleAllLocal();
    }

    // 로컬 풀만 회수
    public void RecycleAllLocal()
    {
        var keysToProcess = new List<string>(_objPoolUsing.Keys);
        foreach (string key in keysToProcess)
        {
            var objectsToRecycle = new List<GameObject>(_objPoolUsing[key]);
            foreach (GameObject obj in objectsToRecycle)
            {
                Recycle(obj);
            }
        }
    }

    public bool IsAllRecycled()
    {
        bool result = true;
        var photonKeysToProcess = new List<string>(_photonPoolUsing.Keys);
        foreach (string key in photonKeysToProcess)
        {
            if(_photonPoolUsing[key].Count > 0)
            {
                Debug.LogWarning($"{key},{_photonPoolUsing[key].Count}");
                result = false;
            }
        }

        return result;
    }

    // 풀 상태 확인용 디버그 메서드
    public void LogPoolStatus()
    {
        Debug.Log("=== Local Pool Status ===");
        foreach (var pool in _objPoolUsing)
        {
            Debug.Log($"Local Using - {pool.Key}: {pool.Value.Count} objects");
        }
        foreach (var pool in _objPool)
        {
            Debug.Log($"Local Available - {pool.Key}: {pool.Value.Count} objects");
        }

        Debug.Log("=== Photon Pool Status ===");
        foreach (var pool in _photonPoolUsing)
        {
            Debug.Log($"Photon Using - {pool.Key}: {pool.Value.Count} objects");
        }
        foreach (var pool in _photonPool)
        {
            Debug.Log($"Photon Available - {pool.Key}: {pool.Value.Count} objects");
        }
    }
    #endregion

    // 기존 코드들...
    #region ===== 기존 로컬 풀 기능 =====
    public T Spawn<T>(string path) where T : MonoBehaviour
    {
        GameObject go = null;
        if (!_objPool.ContainsKey(path))
        {
            _objPool[path] = new List<GameObject>();
            go = Instantiate(Resources.Load(path) as GameObject);
            go.transform.SetParent(_parent);
        }
        else
        {
            if (_objPool[path].Count == 0)
            {
                go = Instantiate(Resources.Load(path) as GameObject);
                go.transform.SetParent(_parent);
            }
            else
            {
                go = _objPool[path][_objPool[path].Count - 1];
                _objPool[path].RemoveAt(_objPool[path].Count - 1);
            }
        }

        if (!_objPoolUsing.ContainsKey(path))
        {
            _objPoolUsing[path] = new List<GameObject>();
        }
        _objPoolUsing[path].Add(go);
        go.name = path;
        if (go.TryGetComponent(out IPoolingObject poolObj))
        {
            poolObj.OnSpawn();
        }
        go.SetActive(true);
        return go.GetComponent<T>();
    }

    public void Recycle(GameObject obj)
    {
        if (obj == null) return;
        obj.SetActive(false);
        obj.transform.position = new Vector3(0, 2000, 0);
        string path = obj.name;
        if (_objPoolUsing.ContainsKey(path) && _objPoolUsing[path].Contains(obj))
        {
            _objPoolUsing[path].Remove(obj);
        }
        if (!_objPool.ContainsKey(path))
        {
            _objPool[path] = new List<GameObject>();
        }
        _objPool[path].Add(obj);

        if (obj.TryGetComponent(out IPoolingObject poolObj))
        {
            poolObj.OnRecycle();
        }

        obj.transform.SetParent(_parent);
    }

    public void Preload(string path, int count)
    {
        GameObject go = null;
        if (!_objPool.ContainsKey(path))
        {
            _objPool[path] = new List<GameObject>();
        }

        for (int i = 0; i < count; i++)
        {
            go = Instantiate(Resources.Load(path) as GameObject);
            go.transform.SetParent(_parent);
            go.name = path;
            go.SetActive(false);
            _objPool[path].Add(go);
        }
    }

    public void LoadAsset<T>(ref T v, string path) where T : Object
    {
        T result = Resources.Load<T>(path);
        if (result != null)
        {
            v = result;
        }
    }

    public T LoadAsset<T>(string path) where T : Object
    {
        T result = Resources.Load<T>(path);
        if (result != null)
        {
            return result;
        }
        return null;
    }
    #endregion
}