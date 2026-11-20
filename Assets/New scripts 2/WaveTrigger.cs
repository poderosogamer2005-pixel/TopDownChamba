using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    [SerializeField] private bool waveSpawned = false;
    [SerializeField] private Spawner2 spawner;
    [SerializeField] private WaveSO waveInformation;


    private void OnTriggerEnter(Collider other)
    {
        if (waveSpawned) { return; }
        if (!other.CompareTag("Player")) { return; }

        spawner.StartSpawing(waveInformation);
        waveSpawned = true;
    }
}
