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

    int lastGeneratedId = -1;

    void Awake()
    {
        _instance = this;
    }

    public void GenerateRoom(Vector3 position, Quaternion rotation)
    {
        int randomRoomId;
        do
        {
#if UNITY_EDITOR
            if (forceGenerate)
            {
                randomRoomId = Mathf.Clamp(forceGenerateRoomId, 0, roomPrefabs.Count);
                break;
            }
            else randomRoomId = Random.Range(0, roomPrefabs.Count);
#else
            randomRoomId = Random.Range(0, roomPrefabs.Count);
#endif
        }
        while (randomRoomId == lastGeneratedId && roomPrefabs.Count > 1);

        Instantiate(roomPrefabs[randomRoomId], position, rotation);
        lastGeneratedId = randomRoomId;
    }
}
