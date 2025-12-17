using UnityEngine;

public class Page : MonoBehaviour
{
    public int normalOrder = 0;
    public int flipOrder = 10;

    SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void BringToFront()
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = flipOrder;
    }

    public void SendToBack()
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = normalOrder;
    }
    
    public void ResetOrder() 
    {
        if (spriteRenderer != null)
        spriteRenderer.sortingOrder = normalOrder;
    }
}
