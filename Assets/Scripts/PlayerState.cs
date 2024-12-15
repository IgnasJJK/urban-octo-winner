using UnityEngine;

public class PlayerState
{
    static PlayerState _instance;
    public static PlayerState instance {
        get
        {
            if (_instance == null) _instance = new PlayerState();
            return _instance;
        }
    }

    int collectibles = 0; 

    public void AddCollectibles(int count = 0)
    {
        collectibles += count;
    }

    public int GetCollectibles()
    {
        return collectibles;
    }
}
