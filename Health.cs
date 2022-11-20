using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public const int maxHealthPlayer = 10;
    public const int maxHp = 10;

    int currentHp;
    public Slider slider;

    public int currentHealthPlayer = maxHealthPlayer; 

    EnemyController enemyController;
    GameManager gameManager;
    Spawner spawner;
    Player player;

    public GameObject enemy;
    
    void Start()
    {
        slider.maxValue = maxHp;
        currentHp = maxHp;
        slider.value = currentHp;

        enemyController = GetComponent<EnemyController>();
        spawner = FindObjectOfType<Spawner>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = FindObjectOfType<Player>();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            currentHp -= 1;
            slider.value = currentHp;
        }
    }

    public void TakenDamage(int amount)
    {
        currentHealthPlayer -= amount;
        if (currentHealthPlayer <= 0)
        {
            currentHealthPlayer = 0;
            slider.gameObject.SetActive(false);
            spawner.BreakTime();
            player.StopPlayer();
            gameManager.GameOver();
        }
    }
}
