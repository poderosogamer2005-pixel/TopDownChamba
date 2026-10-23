using UnityEngine;
using System.Collections;

public class DiaNocheManager : MonoBehaviour
{
    [Header("Duración del día")]
    [SerializeField] private float dayDuration = 60f;

    [Header("Spawners de enemigos normales")]
    [SerializeField] private Spawner[] daySpawners;

    [Header("Oleada nocturna")]
    [SerializeField] private GameObject bigEnemyPrefab;
    [SerializeField] private Transform[] nightSpawnPositions;
    [SerializeField] private int nightWaveSize = 5;
    [SerializeField] private float timeBetweenNightSpawns = 1f;

    [Header("Iluminación del escenario")]
    [SerializeField] private Light luzDelSol;

    [SerializeField]
    private Color colorDia = new Color(0.7f, 0.8f, 1f);

    [SerializeField]
    private Color colorNoche = new Color(0.08f, 0.1f, 0.25f);

    [SerializeField, Range(0f, 2f)]
    private float intensidadDia = 1f;

    [SerializeField, Range(0f, 2f)]
    private float intensidadNoche = 0.15f;

    [SerializeField]
    private float duracionTransicion = 3f;

    private bool isNight;
    private int remainingNightEnemies;
    private int dayNumber = 1;

    private Coroutine dayTimerCoroutine;
    private Coroutine transicionIluminacion;
    private Coroutine nightWaveCoroutine;

    public bool IsNight => isNight;
    public int DayNumber => dayNumber;

    private void Start()
    {
        RenderSettings.ambientLight = colorDia;

        if (luzDelSol != null) luzDelSol.intensity = intensidadDia;

        StartDay();
    }

    private void StartDay()
    {
        isNight = false;
        remainingNightEnemies = 0;

        if (nightWaveCoroutine != null)
        {
            StopCoroutine(nightWaveCoroutine);
            nightWaveCoroutine = null;
        }

        // Elimina los enemigos nocturnos que hayan quedado.
        RemoveEnemiesForPhase(true);

        Debug.Log("Comienza el día " + dayNumber);

        CambiarIluminacion(false);

        // Esto reactiva la aparición de enemigos normales.
        if (daySpawners != null)
        {
            foreach (Spawner spawner in daySpawners)
            {
                if (spawner != null)
                    spawner.StartSpawning();
            }
        }

        if (dayTimerCoroutine != null)
            StopCoroutine(dayTimerCoroutine);

        dayTimerCoroutine = StartCoroutine(DayTimer());
    }

    private IEnumerator DayTimer()
    {
        yield return new WaitForSeconds(dayDuration);

        dayTimerCoroutine = null;
        StartNight();
    }

    private void StartNight()
    {
        if (isNight)
            return;

        isNight = true;

        // Elimina a los enemigos normales del día.
        RemoveEnemiesForPhase(false);

        Debug.Log("¡Comienza la noche!");

        CambiarIluminacion(true);

        // Detener los spawners de los que aparecen en el dia.
        if (daySpawners != null)
        {
            foreach (Spawner spawner in daySpawners)
            {
                if (spawner != null)
                    spawner.StopSpawning();
            }
        }

        if (bigEnemyPrefab == null ||  nightSpawnPositions == null || nightSpawnPositions.Length == 0 ||nightWaveSize <= 0)
        {
            Debug.LogError("No se puede iniciar la noche. " + "Revisa el prefab, los puntos de aparición " + "y la cantidad de enemigos."
            );

            // En esta puede evitar quedarse atrapado en una noche sin oleada.
            dayNumber++;
            StartDay();
            return;
        }

        remainingNightEnemies = nightWaveSize;

        nightWaveCoroutine = StartCoroutine(SpawnNightWave());
    }

    private IEnumerator SpawnNightWave()
    {
        for (int i = 0; i < nightWaveSize; i++)
        {
            
            Transform spawnPoint = null;

            for (int intento = 0; intento < nightSpawnPositions.Length; intento++)
            {
                Transform candidato = nightSpawnPositions[Random.Range(0, nightSpawnPositions.Length)];

                if (candidato != null)
                {
                    spawnPoint = candidato;
                    break;
                }
            }

            if (spawnPoint == null)
            {
                Debug.LogWarning("No hay un punto de aparición nocturna válido.");

                remainingNightEnemies--;
            }
            else
            {
                GameObject enemy = Instantiate(bigEnemyPrefab, spawnPoint.position, spawnPoint.rotation);

                Enemy4 enemyScript = enemy.GetComponent<Enemy4>();

                if (enemyScript != null)
                {
                    enemyScript.ConfigureNightEnemy(this);
                }
                else
                {
                    Debug.LogError(
                        "El prefab nocturno necesita el componente Enemy4."
                    );

                    Destroy(enemy);
                    remainingNightEnemies--;
                }
            }

            yield return new WaitForSeconds(
                Mathf.Max(0.1f, timeBetweenNightSpawns)
            );
        }

        // Debemos esperar en esta parte hasta que todos los enemigos hayan sido derrotados.
        while (isNight && remainingNightEnemies > 0)
        {
            yield return null;
        }

        nightWaveCoroutine = null;

        // Al terminar la oleada, regresar al día.
        if (isNight && remainingNightEnemies <= 0)
        {
            EndNight();
        }
    }

    public void NotifyNightEnemyKilled()
    {
        if (!isNight)
            return;

        remainingNightEnemies = Mathf.Max(0, remainingNightEnemies - 1);

        Debug.Log("Enemigos nocturnos restantes: " + remainingNightEnemies);
    }

    private void EndNight()
    {
        if (!isNight)
            return;

        Debug.Log("¡Oleada derrotada! La noche ha terminado.");

        dayNumber++;

        // Inicia el siguiente día y restaura toda la iluminación que vaya aparecer en esta parte.
        StartDay();
    }

    private void RemoveEnemiesForPhase(bool removeNightEnemies)
    {
        Enemy4[] enemies = Enemy4.GetAllEnemies();

        foreach (Enemy4 enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            if (enemy.IsNightEnemy == removeNightEnemies)
            {
                enemy.DespawnForPhaseChange();
            }
        }
    }

    private void CambiarIluminacion(bool noche)
    {
        if (transicionIluminacion != null)
        {
            StopCoroutine(transicionIluminacion);
            transicionIluminacion = null;
        }

        transicionIluminacion = StartCoroutine(
            TransicionarIluminacion(noche)
        );
    }

    private IEnumerator TransicionarIluminacion(bool noche)
    {
        Color colorInicial = RenderSettings.ambientLight;
        Color colorObjetivo = noche ? colorNoche : colorDia;

        float intensidadInicial = luzDelSol != null
            ? luzDelSol.intensity
            : 0f;

        float intensidadObjetivo = noche
            ? intensidadNoche
            : intensidadDia;

        float tiempo = 0f;
        float duracion = Mathf.Max(0.01f, duracionTransicion);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(tiempo / duracion);

            RenderSettings.ambientLight = Color.Lerp(
                colorInicial,
                colorObjetivo,
                progreso
            );

            if (luzDelSol != null)
            {
                luzDelSol.intensity = Mathf.Lerp(
                    intensidadInicial,
                    intensidadObjetivo,
                    progreso
                );
            }

            yield return null;
        }

        RenderSettings.ambientLight = colorObjetivo;

        if (luzDelSol != null)
            luzDelSol.intensity = intensidadObjetivo;

        transicionIluminacion = null;
    }
}
