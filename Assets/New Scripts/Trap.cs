using UnityEngine;

public class Trap : MonoBehaviour, IInteractable
{
    [SerializeField] private ParticleSystem vfx;

    public void Interact()
    {
        vfx.Play();
    }
}
