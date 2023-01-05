using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anime;
    SpriteRenderer fx;
    public int nextMove;
    BoxCollider2D boxCollider;
    
    //AI
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        fx = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        //AI Move
        rigid.velocity = new Vector2(nextMove * 3, rigid.velocity.y) ;

        //Inhence Wisdom
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.6f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null)
        {
            Turn();
        }
    }

    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);//최저값을 포함, 최대값은 포함 X

        anime.SetInteger("isWalking", nextMove);
        if(nextMove != 0)
        {
            fx.flipX = nextMove == 1;
        }

        float nextThinkTime = Random.Range(2f, 5f);
        
        //Recursive
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;
        anime.SetInteger("isWalking", nextMove);
        fx.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        fx.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        fx.flipY = true;
        //Collider Disable
        boxCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up, ForceMode2D.Impulse);
        //Destroy
        Invoke("DeActive", 3);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
