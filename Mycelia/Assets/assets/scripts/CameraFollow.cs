using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    [SerializeField] private float CameraDepth = -3.5f;
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    
    [SerializeField] private Transform target;
    
    [SerializeField] private PlayerManager playerManager;

    private void Start()
    {
        offset = new Vector3(0f, 0f, CameraDepth);
    }

    void Update()
    {
        if (playerManager.currentHealth > 0)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
