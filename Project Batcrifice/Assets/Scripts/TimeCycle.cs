using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeCycle : MonoBehaviour {

    public Image dayImage;
    public Image nightImage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void OnGUI()
    {
        float fillAmount = Game.getInstance().currentTime() / Game.getInstance().timeScale;
        if (Game.getInstance().isDayTime)
        {
            dayImage.fillClockwise = true;
            nightImage.fillClockwise = false;
            dayImage.fillAmount = 1 - fillAmount;
            nightImage.fillAmount = fillAmount;

        }
        else
        {
            dayImage.fillClockwise = false;
            nightImage.fillClockwise = true;
            dayImage.fillAmount = fillAmount;
            nightImage.fillAmount = 1 - fillAmount;
        }
    }


}
