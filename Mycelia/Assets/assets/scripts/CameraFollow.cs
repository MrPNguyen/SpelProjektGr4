using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float CameraSpeed = 2f;
    [SerializeField] private float CameraDepth = -5f;


    public Transform target;
    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, CameraDepth);

        if (newPos.x < -0.1f)
        {
            newPos.x = -0.1f;
        }
        transform.position = Vector3.Slerp(transform.position, newPos, CameraSpeed * CameraSpeed*Time.deltaTime);
    }
}
