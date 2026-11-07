using System.Collections.Generic;
using UnityEngine;

public class Trampa : MonoBehaviour
{
    [Header("Configuración de daño")]
    [SerializeField] private int dano = 30;
    [SerializeField] private float tiempoEntreDanios = 1f;

    // Temporizador independiente para cada enemigo.
    private Dictionary<Enemy5, float> temporizadores =
        new Dictionary<Enemy5, float>();

    private void OnTriggerStay(Collider other)
    {
        // Buscar Enemy5 incluso si el collider está en un hijo.
        Enemy5 enemigo = other.GetComponentInParent<Enemy5>();

        // Ignorar cualquier objeto que no sea un enemigo.
        if (enemigo == null)
            return;

        if (enemigo.Health <= 0)
            return;

        // Comprobar si el enemigo tiene un temporizador.
        if (!temporizadores.ContainsKey(enemigo))
        {
            temporizadores.Add(enemigo, 0f);
        }

        // Esperar antes de volver a aplicar daño.
        if (temporizadores[enemigo] > 0f)
            return;

        // Aplicar daño.
        enemigo.RecibirDanio(dano);

        // Reiniciar temporizador.
        temporizadores[enemigo] = tiempoEntreDanios;

        Debug.Log(
            "¡TRAMPA ACTIVADA! " +
            enemigo.name +
            " recibió " + dano + " de daño."
        );
    }

    private void Update()
    {
        List<Enemy5> enemigos =
            new List<Enemy5>(temporizadores.Keys);

        foreach (Enemy5 enemigo in enemigos)
        {
            if (enemigo == null)
            {
                temporizadores.Remove(enemigo);
                continue;
            }

            if (temporizadores[enemigo] > 0f)
            {
                temporizadores[enemigo] -= Time.deltaTime;
            }
        }
    }
}
