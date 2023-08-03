using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterSet : MonoBehaviour
{
    
    [SerializeField] protected float M_HP;
    [SerializeField] protected float Speed;
    [SerializeField] protected float M_Damage;
    [SerializeField] protected Transform player;

    protected bool isGround = true;

    public bool isHit = false;
    private bool isDead;

    protected Rigidbody2D rb;
    protected Vector2 movement;
    protected BoxCollider2D BoxCollider;
    public Animator Anim;
    Player playerset;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        playerset = GetComponent<Player>();
    }

    private void Update()
    {
        if (!isDead)
        {
            if (isHit == true)
            {
                Anim.SetTrigger("IsHit");
                isHit = false;  
            }
        }
    }

    /// <summary>
    /// Monster한테 입힐 데미지
    /// </summary>
    /// <param name="Damage"></param>
    public void TakeDamage(float Damage)
    {
        M_HP -= Damage;

        if (M_HP <= 0)
        {
            Anim.SetBool("IsDead", true);
            isHit = true;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerset.TakeDamage(M_Damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }

}
