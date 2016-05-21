using UnityEngine;
using System.Collections;

public class BatController : MonoBehaviour {

    public AudioClip powerUpSound;
    public AudioClip batBiteSound;
    public AudioClip flapSound;

    public enum State
    {
        FLY,
        ATTACH,
        ATTACK
    }

    public static float speed = 5.0f;
    [HideInInspector]
    new public Transform transform;
    public Transform sprite;
    public Animator animator;
    private State state;
    private int side;
    private bool isGrabbing = false;

    public State GetState()
    {
        return state;
    }

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
            transform.localScale = new Vector2(side, 1);
        }

        Vector3 translation = new Vector3( h, v, 0 );
        translation *= (speed * Time.deltaTime);


        if ( state == State.FLY )
        {

            transform.Translate(translation);

            if (Input.GetButtonDown("SwoopAttack") )
            {
                Game.getInstance().state = Game.State.STOP;
                Game.getInstance().hunger -= 10;
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
                Game.getInstance().hunger -= 20;
                Game.getInstance().state = Game.State.STOP;
                StartCoroutine(straightAttack(speed , 5.0f * side));
            }
            else if ( translation != Vector3.zero )
            {
                sprite.localEulerAngles = Vector3.forward * Vector3.Angle(Vector3.down, translation) - Vector3.forward * 90;
            }
        }
        else if ( state == State.ATTACH )
        {
            Game.getInstance().health += 1.00f * Time.deltaTime;
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
        Game.getInstance().audioSource.PlayOneShot(batBiteSound);
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
        Game.getInstance().audioSource.PlayOneShot(batBiteSound);
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

    //--------------------------------------------------------------------------
    IEnumerator grab(GameObject item, Transform collision, Vector3 offset)
    {
        item.GetComponentInParent<Animator>().SetBool("Grabbed", true);
        animator.speed = 4;
        while (Input.GetButton("SwoopAttack") && item && collision)
        {
            if (transform.position.y < item.transform.position.y + collision.localPosition.y) 
            {
                transform.Translate(Vector3.up);
            }
            item.transform.position = transform.position - (collision.localPosition + offset);
            yield return null;
        }
        isGrabbing = false;
        animator.speed = 1;
        if ( item )
        { 
            item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            item.GetComponentInParent<Animator>().SetBool("Grabbed", false);
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


        if (collision.tag == "PowerUp")
        {
            Game.getInstance().audioSource.PlayOneShot(powerUpSound);
            int powerUp = Random.Range(0, 5) + 1;
 
            switch (powerUp)
            {
                case 1: //health up
                    if (Game.getInstance().health == 100)
                    {
                    }
                    else
                    {
                        if (Game.getInstance().health < 81)
                        {
                            float newHealth = Game.getInstance().health + 20;
                            Game.getInstance().health = newHealth;
                        }
                        else
                        {
                            Game.getInstance().health = 100;
                        }

                        print("HEALTH");
                    }

                    break;
                case 2: //night up
                    Game.getInstance().dayTimer.reset();
                    print("TIMER");
                    break;
                case 3: //hunger up
                    if (Game.getInstance().hunger == 100)
                    {
                    }
                    else
                    {
                        if (Game.getInstance().hunger < 81)
                        {
                            float newHunger = Game.getInstance().hunger + 20;
                            Game.getInstance().hunger = newHunger;
                        }
                        else
                        {
                            Game.getInstance().hunger = 100;
                        }
                    }
                    print("HUNGER");
                    break;
                case 4:  //speed up
                    BatController.speed += (BatController.speed * 0.50f);
                    print("SPEED");
                    break;
                case 5:  //SWARM
                    Game.getInstance().hunger = 100;
                    Game.getInstance().health = 100;
                    BatController.speed += (BatController.speed * 0.50f);
                    Game.getInstance().isDayTime = false;
                    Game.getInstance().dayTimer.reset();
                    print("HI SCOTT");
                    break;
            }

            Destroy(collision.gameObject);

        }
    }

    //--------------------------------------------------------------------------
    public void OnTriggerStay2D(Collider2D collision)
    {
        if ( collision.tag == "NPC" && state == State.ATTACK &&
             collision.gameObject.GetComponent<Animator>().GetBool("Attacked") == false )
        {
            StopAllCoroutines();
            collision.transform.localScale = Vector3.one;
            collision.gameObject.GetComponent<Animator>().SetBool("Attacked", true);
            collision.gameObject.GetComponent<NPCController>().incapacitated = true;
            Rigidbody2D r = collision.gameObject.AddComponent<Rigidbody2D>();
            r.gravityScale = 1f;
            r.constraints = RigidbodyConstraints2D.FreezeRotation;
            Game.getInstance().state = Game.State.PLAY;
            state = State.FLY;
        }
        else if ( collision.tag == "Grabbable" && !isGrabbing &&
                  collision.gameObject.GetComponentInParent<Animator>().GetBool("Attacked") )
        {
            isGrabbing = true;
            StopAllCoroutines();
            transform.eulerAngles = Vector3.zero;
            StartCoroutine(grab(collision.transform.parent.gameObject, collision.transform, Vector3.up));
            Game.getInstance().state = Game.State.PLAY;
            state = State.FLY;
        }
    }
}
