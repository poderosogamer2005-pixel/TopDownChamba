using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform jugador;

    [Header("Cámara")]
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 12f, -8f);

    [SerializeField] private float suavizado = 8f;
    [SerializeField] private bool mirarAlJugador = true;

    private void Start()
    {
        if (jugador == null)
        {
            PlayerController5 player =
                FindAnyObjectByType<PlayerController5>();

            if (player != null)
                jugador = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (jugador == null)
            return;

        Vector3 posicionObjetivo = jugador.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            posicionObjetivo,
            suavizado * Time.deltaTime);

        if (mirarAlJugador)
        {
            Vector3 direccion =
                jugador.position - transform.position;

            if (direccion.sqrMagnitude > 0.001f)
            {
                Quaternion rotacionObjetivo =
                    Quaternion.LookRotation(direccion);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    rotacionObjetivo,
                    suavizado * Time.deltaTime);
            }
        }
    }
}
