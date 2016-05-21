using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = (Vector3)((Vector2)target.position) + Vector3.forward * transform.position.z;
	}
}
