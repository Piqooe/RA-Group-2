
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform target;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float minSpawnRadius = 3f;
    [SerializeField] private float spawnInterval = 2f;

    private float spawnTimer;
    private Vector3 lastSpawnCenter;
    private bool hasSpawnCenter;

    private void Update()
    {
        if (prefab == null) return;

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            target = playerObject != null ? playerObject.transform : null;
        }

        if (target != null && target.gameObject.activeInHierarchy)
        {
            Health playerHealth = target.GetComponentInParent<Health>();
            if (playerHealth != null && !playerHealth.IsAlive)
            {
                target = null;
                spawnTimer = 0f;
                return;
            }

            lastSpawnCenter = target.position;
            hasSpawnCenter = true;
        }
        else
        {
            spawnTimer = 0f;
            return;
        }

        if (!hasSpawnCenter) return;

        spawnTimer += Time.deltaTime;

        float interval = Mathf.Max(0.05f, spawnInterval);
        while (spawnTimer >= interval)
        {
            SpawnPrefab();
            spawnTimer -= interval;
        }
    }

    private void SpawnPrefab()
    {
        float maxRadius = Mathf.Max(spawnRadius, minSpawnRadius);
        float minRadius = Mathf.Min(minSpawnRadius, maxRadius);
        float spawnAngle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 randomOffset = new Vector2(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle))
            * Random.Range(minRadius, maxRadius);

        Vector3 spawnPosition = new Vector3(
            lastSpawnCenter.x + randomOffset.x,
            lastSpawnCenter.y + randomOffset.y,
            lastSpawnCenter.z
        );

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}
