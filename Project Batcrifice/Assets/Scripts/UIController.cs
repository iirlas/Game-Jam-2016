using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    public Slider health;
    public Slider hunger;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        health.value = Game.getInstance().health;
        hunger.value = Game.getInstance().hunger;
	}
}
