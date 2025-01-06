using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bombPrefab; // Assign the Bomb prefab in the Inspector

    [SerializeField]
    private Transform spawnPosition; // The position where the bomb will spawn

    [SerializeField]
    private float spawnInterval = 2f; // Time between spawns

    [SerializeField]
    private Transform player; // Assign the Player's Transform in the Inspector

    private void Start()
    {
        StartCoroutine(SpawnBombs());
    }

    private IEnumerator SpawnBombs()
    {
        while (true)
        {
            SpawnBomb();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnBomb()
    {
        if (bombPrefab != null && player != null)
        {
            // Instantiate the bomb at the spawn position
            GameObject bomb = Instantiate(bombPrefab, spawnPosition.position, Quaternion.identity);

            // Pass the player reference to the Bomb script
            ExplodingHead explodingHead = bomb.GetComponent<ExplodingHead>();
            if (explodingHead != null)
            {
                explodingHead.SetTarget(player.position);
            }
        }
    }
}