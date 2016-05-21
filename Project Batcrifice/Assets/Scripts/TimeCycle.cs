using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeCycle : MonoBehaviour {

    public Image dayImage;
    public Image nightImage;
    public Image fadeimage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void OnGUI()
    {
        float fillAmount = 0, scale = 0;
        Color targetColor;
        if ( Game.getInstance().isDayTime )
        {
            fillAmount = Game.getInstance().currentTime() / Game.getInstance().dayTimeScale;
            targetColor = Color.black / 2;
            scale = Game.getInstance().dayTimeScale;
        }
        else
        {
            fillAmount = Game.getInstance().currentTime() / Game.getInstance().nightTimeScale;
            targetColor = Color.clear;
            scale = Game.getInstance().nightTimeScale;
        }

        fadeimage.color = Color.Lerp( fadeimage.color, targetColor, (1 / scale) * Time.deltaTime );

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
