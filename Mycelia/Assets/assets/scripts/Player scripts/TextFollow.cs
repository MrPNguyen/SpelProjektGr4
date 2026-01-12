using UnityEngine;

public class TextFollow : MonoBehaviour
{
    public Transform target;  
    public Vector3 offset;    
    
    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }

        if (transform.position.x < 0)
        {
            transform.position = new Vector3(transform.position.x  * -1, transform.position.y, transform.position.z);
        }
    }
}
