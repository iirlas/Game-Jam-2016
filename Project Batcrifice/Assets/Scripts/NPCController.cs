using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {

    public bool incapacitated = false;
    [HideInInspector]
    new public Transform transform;
    public Transform grabTransform;
    private SpriteRenderer sprite;
    private Animator animator;
    public BoxCollider2D[] boxColliders2d;
    private Timer timer = new Timer();
    private Vector3 direction;
	// Use this for initialization
	void Start () {
        transform = GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxColliders2d = GetComponents<BoxCollider2D>();
        timer.start();
        direction = Vector3.left;
        transform.localScale = direction + Vector3.up + Vector3.forward;
	}
	
	// Update is called once per frame
    void Update()
    {

        if (!incapacitated)
        {
            if (timer.elapsedTime() > Random.Range(1.0f, 6.0f) * 10.0f)
            {
                direction = (direction == Vector3.right ? Vector3.left : Vector3.right);
                transform.localScale = direction + Vector3.up + Vector3.forward;
                //sprite.flipX = !sprite.flipX;
                timer.reset();
                //grabTransform.localPosition = new Vector3(direction.x * Mathf.Abs(grabTransform.localPosition.x), 
                //                                          grabTransform.localPosition.y, 0) ;
            }

            transform.Translate(direction * Time.deltaTime);
        }
        else
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            //float yCenter = grabTransform.position.y + boxColliders2d[1].offset.y;
            //if (yCenter > boxColliders2d[0].bounds.size.y && !animState.IsTag("UP"))
            //{
            //    animator.SetBool("Up", true);
            //    animator.applyRootMotion = true;
            //}
            //else if (yCenter < boxColliders2d[0].bounds.size.y && animState.IsTag("UP"))
            //{
            //    animator.SetBool("Up", false);
            //    animator.applyRootMotion = true;
            //}

            if (animator.GetBool("Grabbed") && !animState.IsTag("UP"))
            {
                animator.SetBool("Up", true);
                animator.applyRootMotion = true;
            }
            else if (!animator.GetBool("Grabbed") && animState.IsTag("UP"))
            {
                animator.SetBool("Up", false);
                animator.applyRootMotion = true;
            }
            else
            {
                animator.applyRootMotion = false;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.tag == "Portal" )
        {
            Destroy(gameObject);
            Game.getInstance().hunger += 20.0f;
            //add to hunger;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
    }

    public void LateUpdate()
    {
        if ( transform.position.y + boxColliders2d[1].offset.y < 0 )
        {
            transform.position = new Vector3(transform.position.x, Mathf.Abs(boxColliders2d[1].offset.y), 0);
        }
    }
    

}
