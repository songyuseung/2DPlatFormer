using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sr;

    [SerializeField, Header("Speed")]
    private float WalkSpeed;
    [SerializeField]
    private float RunSpeed;
    [SerializeField, Header("Jump")]
    private float JumpForce;

    private bool FirstGame = true;

    private float h;
    private float Speed;
    private bool isGround;
    private bool isAttack;

    void Start()
    {
        Speed = WalkSpeed;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        Invoke("First", 1);
        Anim();
    }

    void Update()
    {
        if (!FirstGame)
        {
            if (!isAttack)
                Move();
            Jump();
            Anim();
        }
    }

    void Anim()
    {
        anim.SetBool("Fisrt", FirstGame);
        anim.SetBool("IsMoving", h != 0);
        anim.SetBool("Grounded", isGround);
        
        if (h < 0)
            sr.flipX = false;
        else if (h > 0)
            sr.flipX = true;
    }

    void First()
    {
        FirstGame = false;
    }

    void Move()
    {
        h = Input.GetAxisRaw("Horizontal");

        rigid.velocity = new Vector2(h * Speed, rigid.velocity.y);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            Speed = RunSpeed;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            Speed = WalkSpeed;

        if (Input.GetMouseButtonDown(0) && isGround)
        {
            rigid.velocity = Vector2.zero;
            StartCoroutine("Attack");
            isAttack = true;
            anim.SetTrigger("DoAttack");
        }
    }

    IEnumerator Attack()
    {
        // 공격 코드 작성
        yield return new WaitForSeconds(0.5f);
        isAttack = false;
        yield return new WaitForSeconds(0.7f);
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            rigid.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            isGround = false;
            anim.SetTrigger("DoJump");
        }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
        
    }
}
