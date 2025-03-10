using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer SpriteRenderer;
    CapsuleCollider2D capsulecollider;


    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            nextMove *= -1;
            SpriteRenderer.flipX = nextMove == 1;
            CancelInvoke();
            Invoke("Think", 2);
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);


        anim.SetInteger("WalkSpeed", nextMove);

        if (nextMove != 0)
            SpriteRenderer.flipX = nextMove == 1;

        float nextThinkTime = Random.Range(1f, 3f);
        Invoke("Think", nextThinkTime);

    }


    //void Turn()
    //{
    //    nextMove *= -1;
    //    SpriteRenderer.flipX = nextMove == 1;
    //    CancelInvoke();
    //    Invoke("Think", 2);
    //}

    public void OnDamaged()
    {
        SpriteRenderer.color = new Color(1, 1, 1, 0.4f);

        SpriteRenderer.flipY = true;

        capsulecollider.enabled = false;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }



}
