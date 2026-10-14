using UnityEngine;

public class Caja : MonoBehaviour
{
    [Header("Vida de la caja")]
    [SerializeField] private int vida = 50;

    public void RecibirDanio(int danio)
    {
        if (vida <= 0)
            return;

        vida -= danio;

        Debug.Log("Vida de la caja: " + vida);

        if (vida <= 0)
        {
            ResourceDropper dropper =
                GetComponent<ResourceDropper>();

            if (dropper != null)
            {
                dropper.Destruir();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
