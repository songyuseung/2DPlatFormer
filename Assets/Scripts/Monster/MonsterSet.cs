using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSet : MonoBehaviour
{
    public int M_HP;
    public float Speed;
    public float JumpPower;

    public bool isHit = false;
    public bool isGround = true;

    protected Rigidbody2D rb;
    protected BoxCollider2D BoxCollider;
    public GameObject HitBoxCollider;
    public Animator Anim;
    PlayerHealth player;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        player = GetComponent<PlayerHealth>();
    }

    /// <summary>
    /// Monster한테 데미지
    /// </summary>
    /// <param name="Damage"></param>
    public void TakeDamage(int Damage)
    {
        M_HP -= Damage;

        if (M_HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
