using System.Collections;
using UnityEngine;

public class Spawner2 : MonoBehaviour
{
    [SerializeField] private GameObject regularPrefab;
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject quickPrefab;
    [SerializeField] private Transform[] positions;




    public void StartSpawing(WaveSO _waveInformation)
    {
        StartCoroutine(Spawing(_waveInformation));
    }
    private IEnumerator Spawing(WaveSO waveInformation)
    {
        for (int i = 0; i < waveInformation.enemies.Length; i++)
        {
            GameObject selectedEnemy = null;

            switch (waveInformation.enemies[i])
            {

                case EnemyType.Regular:
                    {
                        selectedEnemy = regularPrefab;
                        break;
                    }

                case EnemyType.Tank:
                    {
                        selectedEnemy = tankPrefab;
                        break;
                    }

                case EnemyType.Quick:
                    {
                        selectedEnemy = quickPrefab;
                        break;
                    }
            }

            if (waveInformation.waveType == WaveType.Random)
            {
                int random = Random.Range(0, positions.Length - 1);
                Instantiate(selectedEnemy, positions[i].position, Quaternion.identity);
            }
            else
            {
                Instantiate(selectedEnemy, waveInformation.staticPositions[i], Quaternion.identity);
            }


            if (waveInformation.instantWave)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(waveInformation.timeBetweenEnemies);
            }
        }

    }
}
