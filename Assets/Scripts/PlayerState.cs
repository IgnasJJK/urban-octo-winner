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

    public int collectibles = 0; 
}
