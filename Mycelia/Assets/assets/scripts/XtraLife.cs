using UnityEngine;

public class XtraLife : MonoBehaviour
{
    private float time = 2;
    private SpriteRenderer sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 1)
        {
            sprite.color = Color.grey;
        }
        if (time <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
