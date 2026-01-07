using UnityEngine;

public class DisableAnimEvent : MonoBehaviour
{
    [SerializeField] private GameObject Anim;

    public void DestroyObject()
    {
        Destroy(Anim);
    }

    public void Disable()
    {
        Anim.SetActive(false);
    }
}
