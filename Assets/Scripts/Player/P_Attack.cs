using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Attack : MonoBehaviour
{
    Slime slime;

    private void Start()
    {
        slime = GetComponent<Slime>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Monster"))
        {
            slime.TakeDamage(10);
        }
    }
}
