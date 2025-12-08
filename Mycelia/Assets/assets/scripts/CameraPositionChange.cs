using UnityEngine;

public class CameraPositionChange : MonoBehaviour
{
    [SerializeField] private Transform position;
    [SerializeField] private CameraFollow cameraFollow;
    void OnTriggerEnter2D(Collider2D other)
    { if (other.gameObject.CompareTag("Player") == false)
            return;
        
        cameraFollow.target = position;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") == false)
            return;
        cameraFollow.target = other.transform;
    }
}
