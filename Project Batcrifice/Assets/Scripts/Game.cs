using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Game : MonoBehaviour {
    public enum State
    {
        PLAY,
        STOP
    };

    public State state;
    private static Game instance = null;

    public float health = 100.0f;
    public float hunger = 100.0f;
    public float dayTimeScale = 5.0f;
    public float nightTimeScale = 100.0f;
    public bool isDayTime = false;
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
        Random.seed = System.DateTime.Now.Millisecond;
	}
	
	// Update is called once per frame
	void Update () {
        if ( (isDayTime && dayTimer.elapsedTime() > dayTimeScale) ||
            (!isDayTime && dayTimer.elapsedTime() > nightTimeScale))
        {
            dayTimer.reset();
            isDayTime = !isDayTime;
        }

        //hunger -= 1.00f * Time.deltaTime;

        if ( isDayTime )
        {
            health -= 1.00f * Time.deltaTime;
        }

        if ( ( hunger <= 0 || health <= 0 ) && SceneManager.GetActiveScene().buildIndex < 3 )
        {
            SceneManager.LoadScene( 3 );
            hunger = 100;
            health = 100;
        }
	}
}
