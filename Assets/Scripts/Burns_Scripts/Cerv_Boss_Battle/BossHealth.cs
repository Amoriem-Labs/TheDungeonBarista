using TDB;
using UnityEngine;
using UnityEngine.UI;

//=================================================================================
    // Author: Nathan B 
    // Description: The health bar for Cerevitus 
//=================================================================================

public class BossHealthUI : MonoBehaviour
{
    public EntityData boss;
    public Slider slider;

    void Start()
    {
        if (boss != null)
        {
            boss.OnDeath += OnBossDeath;
        }
    }

    void Update()
    {
        if (boss != null)
        {
            slider.value = boss.CurrentHealth / boss.MaxHealth;
        }
    }

    void OnBossDeath()
    {
        gameObject.SetActive(false); // hide bar
    }
}