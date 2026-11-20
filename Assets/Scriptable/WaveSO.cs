using UnityEngine;

[CreateAssetMenu(fileName = "WaveSO", menuName = "Scriptable Objects/WaveSO")]
public class WaveSO : ScriptableObject
{
    [Tooltip("La cantidad de enmigos que se instancean")]
    public EnemyType[] enemies;
    public WaveType waveType;

    public Vector3[] staticPositions;

    public bool instantWave;
    public float timeBetweenEnemies;
}

public enum EnemyType
{
    Regular,
    Tank,
    Quick,
}

public enum WaveType
{
    Static,
    Random,
}