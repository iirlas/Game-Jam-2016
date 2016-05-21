using UnityEngine;
using System.Collections;

public class BatController : MonoBehaviour {

    enum State
    {
        FLY,
        ATTACH,
        ATTACK
    }

    public float speed = 5.0f;
    [HideInInspector]
    new public Transform transform;
    public Transform sprite;
    public Animator animator;
    private State state;
    private int side;
    //--------------------------------------------------------------------------
	// Use this for initialization
	void Start () {
        transform = GetComponent<Transform>();        
        state = State.FLY;
        animator.applyRootMotion = true;
        side = 1;
	}
	
    //--------------------------------------------------------------------------
	// Update is called once per frame
	void Update () {
        if (Game.getInstance().state != Game.State.PLAY )
            return;

        //sprite.localEulerAngles = Vector3.forward * 90 * ;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if ( h != 0 )
        {
            side = (int)(h / Mathf.Abs(h));
        }

        Vector3 translation = new Vector3( h, v, 0 );
        translation *= (speed * Time.deltaTime);


        if ( state == State.FLY )
        {

            transform.Translate(translation);

            if (Input.GetButtonDown("SwoopAttack") )
            {
                Game.getInstance().state = Game.State.STOP;
                StartCoroutine(swoopAttack(180.0f, speed * side, 2.0f));
            }
            else if (Input.GetButtonDown("Attach"))
            {
                Game.getInstance().state = Game.State.STOP;
                StartCoroutine(attach(speed / 2.0f, 1.0f));
            }
            else if (Input.GetButtonDown("StraightAttack") )
            {
                state = State.ATTACK;

                Game.getInstance().state = Game.State.STOP;
                StartCoroutine(straightAttack(speed , 5.0f * side));
            }
            else if ( translation != Vector3.zero )
            {
                sprite.localEulerAngles = Vector3.forward * Vector3.Angle(Vector3.down, translation) *
                                          ( translation.x > 0 ? 1 : -1 ) - Vector3.forward * 90;

            }
        }
        else if ( state == State.ATTACH )
        {
            if (Input.GetButtonDown("Attach"))
            {
                state = State.FLY;
                animator.Play("Fly");
            }
        }
	}

    //--------------------------------------------------------------------------
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
        state = State.FLY;
        Game.getInstance().state = Game.State.PLAY;
        yield return null;
    }

    //--------------------------------------------------------------------------
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

    //--------------------------------------------------------------------------
    IEnumerator attach ( float speed, float distance )
    {
        bool onRebound = false;
        Vector3 targetPoint = transform.position;
        Vector3 startPoint = transform.position;
        targetPoint += Vector3.up * distance;
        float targetDistance = 0;

        animator.applyRootMotion = false;
        sprite.localEulerAngles = Vector3.forward;
        animator.Play("Attach", -1, 0);
        

        while (true)
        {
            if ( !onRebound )
            {
                transform.position = Vector3.Lerp(transform.position, targetPoint, speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, startPoint, speed * Time.deltaTime);
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
                    yield return new WaitForSeconds(0.5f);
                    onRebound = true;
                    animator.Play("Unattach", -1, 0);
                }
                else
                {
                    break;
                }
            }

            yield return null;
        }
        animator.applyRootMotion = true;
        animator.Play("Fly");
        Game.getInstance().state = Game.State.PLAY;
        yield return null;
    }

    IEnumerator grab ( GameObject item, Transform collision, Vector3 offset )
    {

        while (Input.GetButton("SwoopAttack"))
        {
            item.transform.position = transform.position - (collision.transform.localPosition + offset);
            yield return null;
        }
        yield return null;
    }

    //--------------------------------------------------------------------------
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tree" && Input.GetButton("Attach") && 
            sprite.eulerAngles.z > 160)
        {
            StopAllCoroutines();
            animator.applyRootMotion = true;
            Game.getInstance().state = Game.State.PLAY;
            state = State.ATTACH;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if ( collision.tag == "NPC" && state == State.ATTACK)
        {
            StopAllCoroutines();
            collision.transform.localScale = Vector3.one;
            collision.gameObject.GetComponent<Animator>().SetBool("Attacked", true);
            collision.gameObject.GetComponent<NPCController>().incapacitated = true;
            collision.gameObject.AddComponent<Rigidbody2D>().gravityScale = 1f;
            Game.getInstance().state = Game.State.PLAY;
            state = State.FLY;
        }
        else if ( collision.tag == "Grabbable" )
        {
            StopAllCoroutines();
            transform.eulerAngles = Vector3.zero;
            StartCoroutine(grab(collision.transform.parent.gameObject, collision.transform, Vector3.up));
            Game.getInstance().state = Game.State.PLAY;
            state = State.FLY;
        }
    }
}
