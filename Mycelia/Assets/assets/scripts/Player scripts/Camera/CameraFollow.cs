using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    
    [SerializeField] private float smoothSpeed = 0.125f;
    public Vector3 offset;
    [SerializeField] private PlayerManager playerManager;
    public bool followPlayer = false;

    [Header("Camera Preferences")] 
    [SerializeField] private float MinX;
    [SerializeField] private float MaxX;
    [SerializeField] private float MinY;
    [SerializeField] private float MaxY;
    


    void Start()
    {
        followPlayer = true;
    }
    private void FixedUpdate()
    {
        if (playerManager.currentHealth > 0 && followPlayer)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, MinX, MaxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, MinY, MaxY);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
