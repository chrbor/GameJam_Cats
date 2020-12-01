using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using static MousScript;
using static GameManager;

public class CatScript : MonoBehaviour
{

    public float moveSpeed;
    public float jumpForce;
    public float JumpSpread;

    public int type;

    public SpriteResolver leftFront;
    public SpriteResolver rightFront;
    public SpriteResolver leftBack;
    public SpriteResolver rightBack;
    public SpriteResolver body;
    public SpriteResolver head;


    private bool doingAction;
    private bool turn;
    private bool onPlattform;
    private bool onGround;

    private Animator anim;
    private Rigidbody2D rb;

    private GameObject[] plattforms;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        plattforms = GameObject.FindGameObjectsWithTag("Platt");

        leftFront.SetCategoryAndLabel("Front", type.ToString());
        rightFront.SetCategoryAndLabel("Front", type.ToString());
        leftBack.SetCategoryAndLabel("Back", type.ToString());
        rightBack.SetCategoryAndLabel("Back", type.ToString());
        head.SetCategoryAndLabel("Head", type.ToString());
        body.SetCategoryAndLabel("Body", type.ToString());

        doingAction = true;
        StartCoroutine(MoveToRandom());
        StartCoroutine(AutoDestroy());
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitUntil(() => game_running);
        yield return new WaitUntil(() => !game_running);
        Destroy(this);
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        if (doingAction) return;
        doingAction = true;

        int ran = Random.Range(0, 5);

        switch (ran)
        {
            case 0: StartCoroutine(Ponder()); break;
            case 1: StartCoroutine(MoveToRandom()); break;
            case 2: StartCoroutine(MoveToRandom()); break;
            case 3: StartCoroutine(JumpForYarn()); break;
            default: StartCoroutine(JumpOnPlattform()); break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground") onGround = true;
        else if (other.gameObject.tag == "Yarn") game_running = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Platt" && rb.velocity.y < 0) onPlattform = true;
    }

    IEnumerator Ponder()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        anim.Play("Stretch");
        yield return new WaitForSeconds(2);
        doingAction = false;
        yield break;
    }

    IEnumerator MoveToRandom()
    {
        Debug.Log("Move");

        float x_pos = Random.Range(-8f, 8f);

        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        turn = true;
        StartCoroutine(TurnToPos(x_pos));
        yield return new WaitWhile(() => turn);

        anim.Play("Walk");
        while(Mathf.Abs(x_pos - rb.position.x) > 0.2f)
        {
            rb.position += Vector2.right * moveSpeed * transform.localScale.x;
            yield return new WaitForFixedUpdate();
        }
        anim.Play("Empty");

        doingAction = false;
        yield break;
    }

    IEnumerator JumpForYarn()
    {
        Debug.Log("Attack");

        turn = true;
        StartCoroutine(TurnToPos(yarnPos.x));
        yield return new WaitWhile(() => turn);

        Vector2 diff = (yarnPos - rb.position) * new Vector2(1,2);
        anim.Play(diff.x > diff.y? "H_Jump":"Jump");
        yield return new WaitForSeconds(1f);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = diff;
        if (rb.velocity.y < 5) rb.velocity += Vector2.up * 5;
        rb.position += rb.velocity.normalized * 0.05f;
        onGround = false;

        if(diff.x > diff.y)
        {
            anim.Play("H_Fly");
            yield return new WaitUntil(() => onGround);
            anim.Play("H_Land");
        }
        else
        {
            anim.Play("Fly");
            yield return new WaitUntil(() => onGround);
            anim.Play("Land");
        }

        yield return new WaitForSeconds(1f);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        doingAction = false;
        yield break;
    }

    IEnumerator JumpOnPlattform()
    {
        Debug.Log("Jump");

        GameObject plattform = plattforms[Random.Range(0, plattforms.Length)];

        turn = true;
        StartCoroutine(TurnToPos(plattform.transform.position.x));
        yield return new WaitWhile(() => turn);

        anim.Play("Jump");
        yield return new WaitForSeconds(1f);
        Vector2 diff = ((Vector2)plattform.transform.position - rb.position) * new Vector2(0.7f, 2f);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = diff;
        if (rb.velocity.y < 9) rb.velocity = new Vector2(rb.velocity.x, 9);
        rb.position += rb.velocity.normalized * 0.05f;
        onPlattform = false;
        onGround = false;

        anim.Play("Fly");
        yield return new WaitUntil(() => onPlattform || onGround);
        anim.Play("Land");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(1f);

        while(Random.Range(0, 2) == 0)
        {
            turn = true;
            StartCoroutine(TurnToPos(yarnPos.x));
            yield return new WaitWhile(() => turn);

            anim.Play("Scratch");
            yield return new WaitForSeconds(1.5f);
        }

        StartCoroutine(JumpForYarn());
        yield break;
    }

    IEnumerator TurnToPos(float x_pos)
    {
        if (Mathf.Sign(x_pos - rb.position.x) != Mathf.Sign(transform.localScale.x))
        {
            //Turn Cat:
            Vector3 scaleStep = new Vector3(-transform.localScale.x * 2, 0);
            for (float count = 0; count < 1; count += Time.deltaTime)
            {
                yield return new WaitForEndOfFrame();
                transform.localScale += scaleStep * Time.deltaTime;
            }
        }
        turn = false;
        yield break;
    }
}
