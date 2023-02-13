using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{ 
    public int health { get; private set; }

    public int StartHealth;

    void Awake()
    {
        health = StartHealth;
    }
    public void DoDamage(int damage)
    {
        health -= damage;
        
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public float GetRemainingHealthAtPercent()
    {
        return health > 0 ? ((float) health / StartHealth) : 0;
    }
}
