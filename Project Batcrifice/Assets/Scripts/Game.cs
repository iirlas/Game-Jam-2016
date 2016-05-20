using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
    public enum State
    {
        PLAY,
        STOP
    };

    public State state;
    private static Game instance = null;    

    public static Game getInstance()
    {
        if ( instance == null )
        {
            instance = FindObjectOfType<Game>();
            if ( instance == null )
            {
                GameObject singleton = new GameObject("Singleton.Game");
                instance = singleton.AddComponent<Game>();
                DontDestroyOnLoad(singleton);
            }
        }
        return instance;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
