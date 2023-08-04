using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    TrailRenderer trail;
 
    private float hAxis;

    [SerializeField, Header("HP")]
    private float PlayerMaxHP;
    [SerializeField]
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
    [SerializeField]
    private Transform Pos;
    [SerializeField]
    private Vector2 BoxSize;

    private const float tapSpeed = .3f;
    private float L_lastTapTime = 0;
    private float R_lastTapTime = 0;

    private bool isDash;
    private bool ReadyDash = true;
    private float DashTime;
    private float DashCool;
    private float DefaultDashCoolTime = 3;

    private bool isGround;
    private bool isAttack;

    void Start()
    {
        PlayerHP = PlayerMaxHP;

        Speed = WalkSpeed;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        trail = GetComponentInChildren<TrailRenderer>();

        trail.enabled = false;
    }

    void Update()
    {
        if (!isAttack)
            InputKey();
        Jump();
        Anim();
        Dash();
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
        anim.SetBool("IsMoving", hAxis != 0);

        if (hAxis < 0)
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (hAxis > 0)
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    void InputKey()
    {
        hAxis = Input.GetAxisRaw("Horizontal");

        rigid.velocity = new Vector2(hAxis * Speed, rigid.velocity.y);

        // 달리기
        if (Input.GetKeyDown(KeyCode.LeftShift))
            Speed = RunSpeed;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            Speed = WalkSpeed;

        // 공격
        if (Input.GetMouseButtonDown(0) && isGround)
        {
            rigid.velocity = Vector2.zero;
            StartCoroutine("Attack");
            isAttack = true;
            anim.SetTrigger("DoAttack");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Pos.position, BoxSize);
    }

    IEnumerator Attack()
    {
        // 공격 Collider 생성
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(Pos.position, BoxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Monster")
            {
                collider.GetComponent<MonsterSet>().TakeDamage(WeaponDamage);
            }
        }

        // 공격 코드 작성
        yield return new WaitForSeconds(0.5f);
        isAttack = false;
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

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            float timeSinceLastClick = Time.time - L_lastTapTime;

            if ((Time.time - L_lastTapTime) < tapSpeed)
            {
                // Double Click
                if (ReadyDash)
                {
                    Debug.Log("A Double");
                    ReadyDash = false;
                    isDash = true;
                }
            }

            L_lastTapTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            float timeSinceLastClick = Time.time - R_lastTapTime;

            if ((Time.time - R_lastTapTime) < tapSpeed)
            {
                // Double Click
                if (ReadyDash)
                {
                    Debug.Log("D Double");
                    ReadyDash = false;
                    isDash = true;
                }
            }

            R_lastTapTime = Time.time;
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

}
