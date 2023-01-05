using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;

    Animator anime;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public GameManager gameManager;
    CapsuleCollider2D capsuleCollider2D;
    
    

    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinished;
    AudioSource audioSource;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATK":
                audioSource.clip = audioAttack;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "DMG":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "FIN":
                audioSource.clip = audioFinished;
                break;
        }
        audioSource.Play();
    }

    private void Update()//�ܹ����� Ű �Է��� Update
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anime.GetBool("isJumping"))
        {
            //if (jumpTrigger == false)
            //{
                //jumpTrigger = true;
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                anime.SetBool("isJumping", true);
            //}
            PlaySound("JUMP");
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //Direction Sprite ������ȯ
        if (Input.GetButton("Horizontal")) {
            if(Input.GetAxisRaw("Horizontal") == -1)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3) 
        {
            anime.SetBool("isWalking", false);
        }
        else
        {
            anime.SetBool("isWalking", true);
        }
    }

    private void FixedUpdate()//�������� Ű �Է�
    {
        //MoveSpeed
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h * 5, ForceMode2D.Impulse);

        if(Mathf.Abs(rigid.velocity.x) > maxSpeed)//right limit speed
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * maxSpeed, rigid.velocity.y);
        }

        //Landing Platform
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));//�����ͻ� Ray�� �׷��ִ� �Լ�
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if(rigid.velocity.normalized.y < 0)
        {
            if (rayHit.collider != null)// ���� ���� ���� ���
            {
                if (rayHit.distance < 0.5f)
                {
                    Debug.Log(rayHit.collider.name);
                    //Ray�� ������ ���� �ʾƼ� �ѹ� ������ �� ==> �÷��̾ ���� �±� ������ ���� �ʿ�
                    //Hit Ȯ�� �ڵ�
                    anime.SetBool("isJumping", false);
                    //jumpTrigger = false;//�̷��� ���� ����
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                Debug.Log("�÷��̾ ���Ƚ��ϴ�.");
                gameManager.stagePoint += 100;
                rigid.AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
                OnAttack(collision.transform);
                PlaySound("ATK");
            }
            else
            {
                Debug.Log("�÷��̾ �¾ҽ��ϴ�.");
                OnDamaged(collision.transform.position);
                PlaySound("DMG");
            }
        }
    }

    void OnAttack(Transform enemy)
    {
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        //Health Down
        gameManager.HealthDown();

        //���̾� ����
        gameObject.layer = 11;
        
        //�ǰ� ȿ��
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //ƨ�� ������ ȿ��
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;//���� �����ڸ� �̿�
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //�ǰ� �ִϸ��̼�
        anime.SetTrigger("doDamaged");

        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            //POINT
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            //Deactive Item
            if (isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if (isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if (isGold)
            {
                gameManager.stagePoint += 150;
            }
            PlaySound("ITEM");
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //Next Stage
            PlaySound("FIN");
            gameManager.NextStage();
        }
    }
    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsuleCollider2D.enabled = false;
        //Die Effect jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
