using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour {

    public float speed;
    new public Camera camera;
    private float distance;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(speed * Time.deltaTime, 0, 0);
        distance += Mathf.Abs(speed);
        if ( camera && distance > camera.pixelWidth )
        {
            Destroy(gameObject);
        }
	}

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if ( collision.transform.tag == "Player" )
        {
            Game.getInstance().health -= 50;
            collision.transform.Translate(Vector3.down);
        }
    }
}
