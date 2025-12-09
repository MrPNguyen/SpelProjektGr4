using UnityEngine;

public class CameraPositionChange : MonoBehaviour
{
    private Vector3 originalOffset;
    [SerializeField] private Vector3 newOffset;
    [SerializeField] private CameraFollow cameraFollow;
    
    void Start()
    {
        originalOffset = cameraFollow.offset;
    }
    
    void OnTriggerStay2D(Collider2D other)
    { if (!other.gameObject.CompareTag("Player"))
            return;
        
        cameraFollow.offset = newOffset;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        cameraFollow.offset = originalOffset;
    }
}
