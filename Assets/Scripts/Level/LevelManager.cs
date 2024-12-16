using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    static LevelManager _instance;
    public static LevelManager instance { get { return _instance; } }

    public List<GameObject> roomPrefabs = new List<GameObject>();

#if UNITY_EDITOR
    [Header("Testing")]
    public bool forceGenerate = false;
    public int forceGenerateRoomId = 0;
#endif

    void Awake()
    {
        _instance = this;
    }

    public void GenerateRoom(Vector3 position, Quaternion rotation)
    {
#if UNITY_EDITOR
        int randomRoomId = Mathf.Clamp(forceGenerateRoomId, 0, roomPrefabs.Count);
        if (!forceGenerate)
        {
            randomRoomId = Random.Range(0, roomPrefabs.Count);
        }
#else
        int randomRoomId = Random.Range(0, roomPrefabs.Count);
#endif
        Instantiate(roomPrefabs[randomRoomId], position, rotation);
    }
}
