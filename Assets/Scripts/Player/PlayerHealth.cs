using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int PlayerHP;
    [SerializeField]
    private int PlayerMaxHP;
    
    MonsterSet monsterSet;
    
    void Start()
    {
        PlayerHP = PlayerMaxHP;
    }

    /// <summary>
    /// Player Damage
    /// </summary>
    /// <param name="Damage"></param>
    public void TakeDamage(int Damage)
    {
        PlayerHP -= Damage;

        if (PlayerHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
