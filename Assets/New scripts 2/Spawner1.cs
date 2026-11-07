using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner1 : MonoBehaviour
{
    private Coroutine spawingCoroutine;

    [Header("Spawing")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform[] positions;
    [SerializeField] private int amount;
    [SerializeField] private float timeBetweenSpawns;


    private void Awake()
    {
        StartSpawing();
    }
    public void StartSpawing()
    {
        if (spawingCoroutine != null) { return; }
        spawingCoroutine = StartCoroutine(Spawing());
    }
    private IEnumerator Spawing()
    {
        for (int i = 0; i < amount; i++)
        {
            int spawingLocation = Random.Range(0, positions.Length - 1);

            Instantiate(enemy, positions[spawingLocation].position, Quaternion.identity);
            Debug.Log(i);

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        yield return null;
    }
}
