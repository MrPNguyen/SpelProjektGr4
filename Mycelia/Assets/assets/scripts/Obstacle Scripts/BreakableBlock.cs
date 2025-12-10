using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableBlock : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (playerMovement.multiplier >= playerMovement.HardDropPower && !playerMovement.IsGrounded)
            {
                StartCoroutine(BreakableCoroutine());
            }
        }
    }

    IEnumerator BreakableCoroutine()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<TilemapRenderer>().enabled = false; 

        yield return new WaitForSeconds(2f);

        GetComponent<Collider2D>().enabled = true;
        GetComponent<TilemapRenderer>().enabled = true;
    }
}
