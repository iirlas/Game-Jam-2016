using UnityEngine;
using System.Collections;

public class Timer
{

    float myStartTime = 0;
    float myPauseDelta = 0;
    bool myTimerIsRunning = false;
    bool myTimerIsPaused = false;

    public static bool operator true ( Timer timer )
    {
        return (timer.isRunning() && !timer.isPaused());
    }

    public static bool operator false ( Timer timer )
    {
        return !(timer.isRunning() && !timer.isPaused());
    }

    public static bool operator! ( Timer timer )
    {
        return !(timer.isRunning() && !timer.isPaused());
    }

    public bool start ()
    {
        if (!myTimerIsRunning)
        {
            myStartTime = Time.time;
            myTimerIsRunning = true;
            return true;
        }
        else if (myTimerIsPaused)
        {
            myStartTime = Time.time - myPauseDelta;
            myPauseDelta = 0;
            myTimerIsPaused = false;
            return true;
        }
        return false;
    }

    public float pause ()
    {
        if (myTimerIsRunning && !myTimerIsPaused)
        {
            myPauseDelta = Time.time - myStartTime;
            myTimerIsPaused = true;
            return myPauseDelta;
        }
        return -1; 
    }

    public float stop ()
    {
        if (myTimerIsRunning)
        {
            float stopTime = Time.time;
            myTimerIsRunning = false;
            myTimerIsPaused = false;
            myPauseDelta = 0;
            return (stopTime - myStartTime);
        }
        return -1;
    }

    public float reset ()
    {
        float stopTime = stop();
        start();
        return stopTime;
    }

    public float elapsedTime ()
    {
        if ( myTimerIsRunning )
        {
            if (myTimerIsPaused)
            {
                return myPauseDelta;
            }
            else
            {
                return Time.time - myStartTime;
            }
        }
        return -1;
    }

    public bool isRunning ()
    {
        return myTimerIsRunning;
    }

    public bool isPaused ()
    {
        return myTimerIsPaused;
    }
}
