using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;
    public Wave[] waves;
    public EnemyController enemy;
    public float startTime;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;
    int enemiesRemainingAlive;
    int enemiesRemainingToSpawn;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampChecktime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;
    bool isBreak;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampChecktime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();

        isBreak = false;
    }

    void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampChecktime)
            {
                nextCampChecktime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn --;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }

                NextWave();
            }
        }

        if (isBreak)
        {
            StopCoroutine("SpawnEnemy");
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                GameObject.Destroy(enemy.gameObject);
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(startTime);
        float spawndelay = 1;
        float tileFalshSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetRandomOpenTile();
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawndelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFalshSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }
        EnemyController SpawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as EnemyController;
        SpawnedEnemy.OnDeath += OnEnemyDeath;
    }

    public void BreakTime()
    {
        isBreak = true;
    }


    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive --;

        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        // playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave()
    {
        
        currentWaveNumber ++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves [currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);  
            }
            ResetPlayerPosition();
        }
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
