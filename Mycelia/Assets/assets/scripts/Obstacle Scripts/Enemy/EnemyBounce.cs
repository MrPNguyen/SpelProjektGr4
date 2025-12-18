using UnityEngine;

public class EnemyBounce : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float Bounce;
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip BigBounce;

    void Start()
    {
        audio.clip = BigBounce;
    }
    void Update()
    {
        if (!audio.isPlaying)
        {
            audio.Stop();
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (playerMovement.multiplier >= 2 && Bounce > 0)
            {
                audio.Play();
                playerMovement.hasHardDropped = false;
                playerMovement.velocity.y = Bounce;
            }
            
        }
    }
}
