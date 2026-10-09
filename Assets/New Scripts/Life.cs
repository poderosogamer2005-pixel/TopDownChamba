using UnityEngine;

public class Life : MonoBehaviour, IInteractable, IDamagable
{
    [field: SerializeField] public int Health { get; set; }
    [SerializeField] private GameObject door;

    public void Interact()
    {
        door.SetActive(false);
    }
}
