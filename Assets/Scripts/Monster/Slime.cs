using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonsterSet
{
    SpriteRenderer spriteRenderer;

    [SerializeField]
    private float nextMove;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Think();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        } 
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            KnockBack();

            rb.velocity = new Vector2(nextMove, rb.velocity.y);

            Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.5f, rb.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayhit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));
            if (rayhit.collider == null)
                Turn();
        }
    }

    void KnockBack()
    {
        
    }

    void Think()
    {
        Move();

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Move()
    {
        nextMove = Random.Range(-1, 2);
        Animate();
    }

    void Turn()
    {
        nextMove *= -1;
        Animate();

        CancelInvoke();
        Invoke("Think", 5f);
    }

    void Animate()
    {
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
    }
}
