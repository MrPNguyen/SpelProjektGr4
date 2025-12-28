using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float ParallaxSpeed;
    private RectTransform[] rts;
    private float imgWidth;

    void Start()
    {
        rts = new RectTransform[transform.childCount];

        for (int i = 0; i < rts.Length; i++)
        {
            rts[i] = transform.GetChild(i).gameObject.GetComponent<RectTransform>();
        }
        
        imgWidth = rts[0].rect.width;   
    }
    private void Update()
    {
        foreach (RectTransform rt in rts)
        {
            rt.anchoredPosition += Vector2.right * ParallaxSpeed * Time.deltaTime;

            if (rt.anchoredPosition.x > imgWidth)
            {
                rt.anchoredPosition -= Vector2.right * imgWidth * rts.Length;
            }
        }
    }
}
