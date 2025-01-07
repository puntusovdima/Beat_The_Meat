using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bombPrefab, enemyPrefabEasy, enemyPrefabHard;
    [SerializeField]
    private Transform spawnPosition;
    [SerializeField]
    private float spawnInterval = 2f;
    [SerializeField]
    private Transform player;
    
    private bool isSpawningHardEnemies = false;
    private bool isEasy;
    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        StartCoroutine(SpawnBombs());
        StartCoroutine(CheckHealthAndSpawnEnemies());
    }

    private IEnumerator SpawnBombs()
    {
        while (true)
        {
            if (GetComponent<Boss>().currentState == 0)
            {
                SpawnBomb();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // New coroutine to check health and manage hard enemy spawning
    private IEnumerator CheckHealthAndSpawnEnemies()
    {
        while (true)
        {
            if (enemyHealth.currentHealth <= 150 && !isSpawningHardEnemies)
            {
                spawnInterval = 1;
                isSpawningHardEnemies = true;
                StartCoroutine(SpawnHardEnemiesRoutine());
            }
            yield return new WaitForSeconds(1f); // Check health every second
        }
    }

    // New coroutine for spawning hard enemies every 15 seconds
    private IEnumerator SpawnHardEnemiesRoutine()
    {
        while (enemyHealth.currentHealth <= 150)
        {
            SpawnEnemies();
            yield return new WaitForSeconds(15f);
        }
        isSpawningHardEnemies = false;
    }

    private void SpawnBomb()
    {
        if (bombPrefab != null && player != null)
        {
            GameObject bomb = Instantiate(bombPrefab, spawnPosition.position, Quaternion.identity);
            ExplodingHead explodingHead = bomb.GetComponent<ExplodingHead>();
            if (explodingHead != null)
            {
                explodingHead.SetTarget(player.position);
            }
        }
    }

    public void SpawnEnemiesWhenHit()
    {
        if (enemyPrefabEasy && player)
        {
            Instantiate(enemyPrefabEasy, spawnPosition.position, Quaternion.identity);
        }
    }

    public void SpawnEnemies()
    {
        if (enemyPrefabHard && player)
        {
            Instantiate(enemyPrefabHard, spawnPosition.position, Quaternion.identity);
        }
    }
}