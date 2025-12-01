using UnityEngine;
using UnityEngine.Audio;

public class PlayerSFXManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpClip;
    
    
    
    private PlayerMovement playerMove;
    private Animator animator;
    void Start()
    {
        playerMove = new PlayerMovement();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        {
            audioSource.clip = jumpClip;
        }
    }
    
    
}
