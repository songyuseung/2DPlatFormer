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

    private float hAxis;

    [SerializeField, Header("Speed")]
    private float WalkSpeed;
    [SerializeField]
    private float RunSpeed;
    [SerializeField]
    private float DashSpeed;
    private float Speed;

    [SerializeField, Header("Jump")]
    private float JumpForce;

    private const float Double_Click_Time = .2f;
    private float lastClickTime;

    private bool FirstGame = true;

    private bool isDash;
    private bool ReadyDash = true;
    private float DashTime;
    private float DashCool;
    private float DefaultDashCoolTime = 3;

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
            DashClick();
        }
    }

    void Anim()
    {
        anim.SetBool("Fisrt", FirstGame);
        anim.SetBool("IsMoving", hAxis != 0);
        anim.SetBool("Grounded", isGround);
        
        if (hAxis < 0)
            sr.flipX = false;
        else if (hAxis > 0)
            sr.flipX = true;
    }

    void First()
    {
        FirstGame = false;
    }

    void Move()
    {
        hAxis = Input.GetAxisRaw("Horizontal");

        rigid.velocity = new Vector2(hAxis * Speed, rigid.velocity.y);

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

    void DashClick()
    {
        if (hAxis != 0)
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= Double_Click_Time)
            {
                // Double Click
                if (ReadyDash)
                {
                    isDash = true;
                    ReadyDash = false;
                }
            }
            else
            {
                // Normal Click
                Speed = WalkSpeed;
            }

            lastClickTime = Time.time;
        }

        if (!ReadyDash)
        {
            DashCool += Time.deltaTime;
            ReadyDash = DashCool >= DefaultDashCoolTime;
        }
        
        if (DashTime <= 0)
        {
            Speed = WalkSpeed;

            if (isDash)
            {
                DashTime = 0.1f;
            }
        }
        else
        {
            DashTime -= Time.deltaTime;
            Speed = DashSpeed;
            ReadyDash = false;
        }

        isDash = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
        
    }
}
