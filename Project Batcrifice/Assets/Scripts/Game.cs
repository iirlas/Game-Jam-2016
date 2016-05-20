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

    public float timeScale = 5.0f;
    public bool isDayTime = true;
    private Timer dayTimer = new Timer();

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

    public float currentTime ()
    {
        return dayTimer.elapsedTime();
    }

	// Use this for initialization
	void Start () {
        dayTimer.start();
	}
	
	// Update is called once per frame
	void Update () {
        if ( dayTimer.elapsedTime() > timeScale )
        {
            dayTimer.reset();
            isDayTime = !isDayTime;
        }
	}
}
