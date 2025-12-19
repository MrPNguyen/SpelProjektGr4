using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KingCage : MonoBehaviour
{
    private SpriteRenderer CageSprite; 
    [SerializeField]private Sprite OpenCageSprite;
    
    [Header("Locks")]
   [SerializeField] private GameObject RedLock;
   [SerializeField] private GameObject GreenLock;
   [SerializeField] private GameObject YellowLock;
   
   [Header("Keys")]
   [SerializeField] private GameObject RedKey;
   [SerializeField] private GameObject GreenKey;
   [SerializeField] private GameObject YellowKey;
   
   [Header("Ending Dialogue")]
   [SerializeField] private DialogueTrigger saved;
   [SerializeField] private DialogueManager dialogueManager;
   [SerializeField] private CameraFollow cameraFollow;
   [SerializeField] private GameObject player;
   [SerializeField] private Animator end;
   [SerializeField] private GameObject playerUI;
   private bool EndTheGame;
   private bool coroutineDone = false;
    void Start()
    {
        EndTheGame = false;
        CageSprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (EndTheGame)
        {
            if (dialogueManager.DialogueEnd && !coroutineDone)
            {
                StartCoroutine(Epiloguecoroutine());
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (YellowKey.activeSelf == false)
            {
                YellowLock.SetActive(false);
            }
            if (RedKey.activeSelf == false)
            {
                RedLock.SetActive(false);
               
            }
            if (GreenKey.activeSelf == false)
            {
                GreenLock.SetActive(false);
            }
            if (RedLock.activeSelf == false && GreenLock.activeSelf == false && YellowLock.activeSelf == false)
            { 
                CageSprite.sprite = OpenCageSprite;
              
               saved.TriggerDialogue();
               cameraFollow.offset.z = -3;
               player.transform.position = new Vector3(player.transform.position.x, 2.019093f, player.transform.position.z);
               Debug.Log("cameraFollow");
               EndTheGame = true;
            }
            else
            {
                //trigga påskyndande dialog ex."You must hurry to save the forest and I"
            }
        }
    }

    private IEnumerator Epiloguecoroutine()
    {
        coroutineDone = false;
        playerUI.SetActive(false);
        end.SetTrigger("End");
        yield return new WaitForSeconds(5.5f);
        SceneManager.LoadSceneAsync(4, LoadSceneMode.Single);
        coroutineDone = true;
    }
}
