using UnityEngine;

public class BotiquinCurativo : MonoBehaviour
{
    [Header("Curación")]
    [SerializeField] private int cantidadCuracion = 30;

    private bool utilizado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (utilizado)
            return;

        // Comprueba si el objeto que chocó es el jugador.
        PlayerController6 jugador = other.GetComponentInParent<PlayerController6>();

        if (jugador == null)
            return;

        // No cura a un jugador muerto.
        if (jugador.IsDead)
            return;

        // No consume el botiquín si la vida está completa.
        if (jugador.CurrentHealth >= 200)
        {
            Debug.Log("¡Tienes la vida al máximo!");
            return;
        }

        utilizado = true;

        // Recupera vida sin superar el máximo.
        jugador.Heal(cantidadCuracion);

        Debug.Log("¡Has recogido un botiquín!");

        // Destruye el botiquín después de recogerlo.
        Destroy(gameObject);
    }
}
