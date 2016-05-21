using UnityEngine;
using System.Collections;

public class BirdSpawner : MonoBehaviour {

    new public Camera camera;
    public float speed;
    public Object birdPrefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        int number = Random.Range(1,100);
        if ( number == 5 )
        {
            number = Random.Range(0,2);
            bool flag = number == 1;
            Vector3 pos = Vector3.one * Random.Range(0, camera.pixelHeight);
            pos.x = (flag ? 0 : camera.pixelWidth);
            pos = camera.ScreenToWorldPoint(pos);
            pos.y = Mathf.Abs(pos.y);
            GameObject bird = Instantiate( birdPrefab, pos, Quaternion.identity ) as GameObject;
            BirdController bc = bird.GetComponent<BirdController>();
            bc.speed = (flag ? speed : -speed);
            bc.camera = camera;
            if ( !flag )
            {
                bc.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
	}
}
