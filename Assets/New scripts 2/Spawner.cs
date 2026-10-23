using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Enemigos del día")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform[] positions;
    [SerializeField] private float timeBetweenSpawns = 5f;

    [Header("Separación entre enemigos")]
    [SerializeField] private float minDistanceBetweenEnemies = 8f;
    [SerializeField] private int attemptsPerSpawn = 10;

    private Coroutine spawningCoroutine;

    public void StartSpawning()
    {
        if (spawningCoroutine != null)
            return;

        spawningCoroutine = StartCoroutine(SpawnEnemies());
    }

    public void StopSpawning()
    {
        if (spawningCoroutine == null)
            return;

        StopCoroutine(spawningCoroutine);
        spawningCoroutine = null;
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(
                Mathf.Max(0.1f, timeBetweenSpawns)
            );
        }
    }

    private void SpawnEnemy()
    {
        if (enemy == null || positions == null || positions.Length == 0)
            return;

        List<Transform> validPositions = new List<Transform>();

        foreach (Transform point in positions)
        {
            if (point != null)
                validPositions.Add(point);
        }

        if (validPositions.Count == 0)
            return;

        Transform selectedPosition = null;
        float bestDistance = -1f;

        for (int i = 0; i < attemptsPerSpawn; i++)
        {
            Transform candidate = validPositions[
                Random.Range(0, validPositions.Count)
            ];

            float nearestEnemyDistance =
                GetNearestEnemyDistance(candidate.position);

            if (nearestEnemyDistance >= minDistanceBetweenEnemies)
            {
                selectedPosition = candidate;
                break;
            }

            if (nearestEnemyDistance > bestDistance)
            {
                bestDistance = nearestEnemyDistance;
                selectedPosition = candidate;
            }
        }

        if (selectedPosition != null)
        {
            Instantiate(
                enemy,
                selectedPosition.position,
                selectedPosition.rotation
            );

            Debug.Log(
                "Enemigo generado en: " + selectedPosition.name
            );
        }
    }

    private float GetNearestEnemyDistance(Vector3 position)
    {
        Enemy4[] enemies = FindObjectsByType<Enemy4>();

        float nearestDistance = Mathf.Infinity;

        foreach (Enemy4 currentEnemy in enemies)
        {
            if (currentEnemy == null || currentEnemy.IsDead)
                continue;

            // Ignorar a los enemigos grandes de la noche.
            if (currentEnemy.CompareTag("EnemigoNocturno"))
                continue;

            float distance = Vector3.Distance(
                position,
                currentEnemy.transform.position
            );

            if (distance < nearestDistance)
                nearestDistance = distance;
        }

        return nearestDistance;
    }

    private void OnDisable()
    {
        StopSpawning();
    }
}
