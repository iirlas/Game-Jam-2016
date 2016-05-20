using UnityEngine;
using System.Collections;

public class BatController : MonoBehaviour {

    enum State
    {
        FLY,
        ATTACH
    }

    public float speed = 5.0f;

    new public Transform transform;
    private State state;
	// Use this for initialization
	void Start () {
        transform = GetComponent<Transform>();
        state = State.FLY;
	}
	
	// Update is called once per frame
	void Update () {
        if (Game.getInstance().state != Game.State.PLAY )
            return;

        Vector3 translation = new Vector3( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0 );
        translation *= (speed * Time.deltaTime);


        if ( state == State.FLY )
        {

            transform.Translate(translation);

            if ( Input.GetButtonDown( "SwoopAttack" ) )
            {
                if ( translation.x != 0 )//swoop attack
                {
                    int side = (int)(translation.x / Mathf.Abs( translation.x ));
                    Game.getInstance().state = Game.State.STOP;
                    StartCoroutine(swoopAttack(180.0f, speed * side, 2.0f));
                }
            }
            else if (Input.GetButtonDown("Attach"))
            {
                Game.getInstance().state = Game.State.STOP;
                StartCoroutine(attach(2.0f, speed / 2.0f));
            }
            else if (Input.GetButtonDown("StraightAttack"))
            {
                int side = (int)(translation.x / Mathf.Abs(translation.x));

                Game.getInstance().state = Game.State.STOP;
                StartCoroutine(straightAttack(speed * 5.0f, 5.0f* side));
            }
        }
        else if ( state == State.ATTACH )
        {
            if (Input.GetButtonDown("Attach"))
            {
                state = State.FLY;
            }
        }
	}

    IEnumerator straightAttack ( float speed, float distance )
    {
        Vector3 targetPoint = transform.position;
        targetPoint += Vector3.right * distance * ( speed > 0 ? 1 : -1 );
        float targetDistance = 0;
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint, speed * Time.deltaTime);
            targetDistance = Vector3.Distance(transform.position, targetPoint);
            if (!Input.GetButton("StraightAttack"))
            {
                break;
            }
            if (targetDistance < .1f)
            {
                break;
            }

            yield return null;
        }

        Game.getInstance().state = Game.State.PLAY;
        yield return null;
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

            if (!Input.GetButton("SwoopAttack"))
            {
                break;
            }

            yield return null;
        }
        transform.eulerAngles = Vector3.zero;
        transform.Translate( speed * Time.deltaTime, 0, 0 );
        Game.getInstance().state = Game.State.PLAY;
        yield return null;
    }

    IEnumerator attach ( float distance, float speed )
    {
        bool onRebound = false;
        Vector3 targetPoint = transform.position;
        targetPoint += Vector3.up * distance;
        float targetDistance = 0;
        while (true)
        {
            if ( !onRebound )
            {
                transform.position = Vector3.Lerp(transform.position, targetPoint, speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
            }
            targetDistance = Vector3.Distance(transform.position, targetPoint);
            if (!Input.GetButton("Attach"))
            {
                break;
            }
            if (targetDistance < .1f)
            {
                if ( !onRebound )
                {
                    onRebound = true;
                    targetPoint -= Vector3.up * distance;
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    break;
                }
            }

            yield return null;
        }

        Game.getInstance().state = Game.State.PLAY;
        yield return null;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tree" && Input.GetButton("Attach"))
        {
            StopAllCoroutines();
            Game.getInstance().state = Game.State.PLAY;
            state = State.ATTACH;
        }
    }

}
