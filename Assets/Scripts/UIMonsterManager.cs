using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterManager : MonoBehaviour
{
    public TMP_Text Health;

    public Slider Slider;
    public void SetHealth(int health)
    {
        
        if (health <= 0)
        {
            health = 0;
        }
        Health.text = health.ToString();
    }

    public void SetSlaiderValue(float remainingHealthAtPercent)
    {
        Slider.value = remainingHealthAtPercent;
    }
}
