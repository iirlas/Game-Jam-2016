using UnityEngine;
using System.Collections;

public class BatController : MonoBehaviour {

    public float speed = 5.0f;

    new public Transform transform;

	// Use this for initialization
	void Start () {
        transform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Game.getInstance().state != Game.State.PLAY)
            return;

        Vector3 translation = new Vector3( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0 );
        translation *= (speed * Time.deltaTime);


        transform.Translate(translation);

        if ( Input.GetButtonDown( "Jump" ) )
        {
            if ( translation.x != 0 )//swoop attack
            {
                int side = (int)(translation.x / Mathf.Abs( translation.x ));
                Game.getInstance().state = Game.State.STOP;
                StartCoroutine(swoopAttack(180.0f, speed * side, 2.0f));
            }
        }

        if (Input.GetButtonDown("Attach"))
        {
            Game.getInstance().state = Game.State.STOP;
            StartCoroutine(attach(2.0f));
        }
	}

    IEnumerator swoopAttack ( float angle, float speed, float pivotOffset )
    {
        Vector3 pivotPoint = transform.position;
        pivotPoint = (speed > 0 ? pivotPoint + (Vector3.right * pivotOffset) :
                                  pivotPoint - (Vector3.right * pivotOffset));

        while ( angle != 0 )
        {
            transform.RotateAround(pivotPoint, Vector3.forward, speed);
            angle = (speed > 0 ? angle - speed : angle + speed);

            if ( !Input.GetButton( "Jump" ) )
            {
                break;
            }

            //pivotPoint.x += (speed > 0 ? 1 : -1) * .1f;

            yield return null;
        }
        transform.eulerAngles = Vector3.zero;
        transform.Translate( speed * Time.deltaTime, 0, 0 );
        Game.getInstance().state = Game.State.PLAY;
        yield return null;
    }

    IEnumerator attach ( float distance )
    {
        Vector3 targetPoint = transform.position;
        targetPoint += Vector3.up * distance;
        while ( Vector3.Distance( transform.position, targetPoint ) > .1f )
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
            if (!Input.GetButton("Attach"))
            {
                break;
            }
            yield return null;
        }
        Game.getInstance().state = Game.State.PLAY;
        yield return null;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //co-routine
        //if wall bounce off and damage
    }

}
