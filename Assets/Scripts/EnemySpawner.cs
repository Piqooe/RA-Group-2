
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;

    public float minDistance = 8f;
    public float maxDistance = 12f;
    public float spawnInterval = 2f;

    private float nextSpawnTime;

    private void Update()
    {
        if (player == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        float randomDistance = Random.Range(minDistance, maxDistance);

        Vector2 spawnPosition = (Vector2)player.position + randomDirection * randomDistance;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
