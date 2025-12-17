using UnityEngine;

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
  
   
    void Start()
    {
        CageSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                //trigga slut-dialogen här
            }
            else
            {
                //trigga påskyndande dialog ex."You must hurry to save the forest and I"
            }
        }
    }
}
