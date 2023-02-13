using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterManager : MonoBehaviour
{
    public UIMonsterManager uiMonsterManager;

    public Monster Monster;

    
    private void Start()
    {
        uiMonsterManager.SetHealth(Monster.health);
    }

    public void DoDamage(int damage)
    {
        Monster.DoDamage(damage);

        if (Monster.IsDead())
        {
            Monster.gameObject.SetActive(false);
        }

        uiMonsterManager.SetHealth(Monster.health);
        uiMonsterManager.SetSlaiderValue(Monster.GetRemainingHealthAtPercent());
    }
}
