using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthEnemy : MonoBehaviour
{
    public const int maxHealthEnemy = 100;
    public int currentHealthEnemy = maxHealthEnemy; 
    public int maxHp = 100;

    EnemyController enemyController;

    int currentHp;
    public Slider slider;
    
    void Start()
    {
        slider.maxValue = maxHp;
        currentHp = maxHp;
        slider.value = currentHp;
        enemyController = GetComponent<EnemyController>();
    }

    // public void OnTriggerEnter(Collider collision)
    // {
    //     if (collision.gameObject.tag == "Bullet")
    //     {
    //         // currentHp -= 1;
    //         // slider.value = currentHp;
    //     }
    // }

    public void TakenDamage(int amount)
    {
        currentHealthEnemy -= amount;
        if (currentHealthEnemy <= 0)
        {
            currentHealthEnemy = 0;
            enemyController.StartCoroutine("Dead");
        }
        currentHp -= 1;
        slider.value = currentHp;
    }
}
