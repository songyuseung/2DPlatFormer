using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sr;
    CircleCollider2D weaponPos;
    TrailRenderer trail;
    [SerializeField]
    MonsterSet monster;

    private float hAxis;

    [SerializeField, Header("HP")]
    private float PlayerMaxHP;
    private float PlayerHP;
    [SerializeField, Header("Speed")]
    private float WalkSpeed;
    [SerializeField]
    private float RunSpeed;
    [SerializeField]
    private float DashSpeed;
    private float Speed;

    [SerializeField, Header("Jump")]
    private float JumpForce;

    [field: SerializeField, Header("Weapon")]
    public float WeaponDamage { get; private set; }

    private const float tapSpeed = .3f;
    private float lastTapTime = 0;

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
        weaponPos = GetComponentInChildren<CircleCollider2D>();
        trail = GetComponentInChildren<TrailRenderer>();
        monster = GetComponent<MonsterSet>();

        trail.enabled = false;

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
            //DashInput();
        }
    }

    /// <summary>
    /// Player한테 입힐 데미지
    /// </summary>
    /// <param name="Damage"></param>
    public void TakeDamage(float Damage)
    {
        PlayerHP -= Damage;

        if (PlayerHP <= 0)
        {
            Destroy(gameObject);
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
        weaponPos.enabled = true;
        yield return new WaitForSeconds(0.5f);
        isAttack = false;
        yield return new WaitForSeconds(0.7f);
        weaponPos.enabled = false;
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

    /*
    void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            float L_timeSinceLastClick = Time.time - lastTapTime;

            if ((Time.time - lastTapTime) < tapSpeed)
            {
                // Double Click
                if (ReadyDash)
                {
                    Debug.Log("A Double");
                    ReadyDash = false;
                    isDash = true;
                }
            }

            lastTapTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            float R_timeSinceLastClick = Time.time - lastTapTime;

            if ((Time.time - lastTapTime) < tapSpeed)
            {
                // Double Click
                if (ReadyDash)
                {
                    Debug.Log("D Double");
                    ReadyDash = false;
                    isDash = true;
                }
            }

            lastTapTime = Time.time;
        }

        if (!ReadyDash)
        {
            DashCool += Time.deltaTime;
            ReadyDash = DashCool >= DefaultDashCoolTime;
        }

        if (DashTime <= 0)
        {
            Speed = WalkSpeed;
            trail.enabled = false;

            if (isDash)
            {
                DashTime = 0.1f;
                DashCool = 0;
            }   
        }
        else
        {
            trail.enabled = true;
            DashTime -= Time.deltaTime;
            Speed = DashSpeed;
            ReadyDash = false;
        }

        isDash = false;
    } */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            monster = collision.gameObject.GetComponent<MonsterSet>();
            monster.TakeDamage(WeaponDamage);
            monster.isHit = true;
        }
    }
}
