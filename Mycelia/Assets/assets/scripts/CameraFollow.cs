using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float CameraSpeed = 2f;
    [SerializeField] private float CameraDepth = -5f;
    [SerializeField] private PlayerManager Player;


    public Transform target;
    // Update is called once per frame
    void Update()
    {
        if (Player.currentHealth > 0)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y, CameraDepth);

            if (newPos.x < -4f)
            {
                newPos.x = -4f;
            }
            transform.position = Vector3.Slerp(transform.position, newPos, CameraSpeed * CameraSpeed*Time.deltaTime);
        }
    }
}
