using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject enemy;
    [SerializeField] private float SecondsToWait;
    [SerializeField] private Animator agaricAnim;
    [SerializeField] private EnemyMovement enemyMovement;
    private bool isRespawning = false;
    // Update is called once per frame
    void Update()
    {
        if (enemy.activeSelf == false && !isRespawning)
        {
            StartCoroutine(RespawnEnemy());
        }
    }
    
    IEnumerator RespawnEnemy()
    {
        isRespawning = true;
        enemyMovement.waitBeforeWalking = 2f;
        yield return new WaitForSeconds(SecondsToWait);
        agaricAnim.SetTrigger("Revive");       
        yield return null;
        
        float riseLength = agaricAnim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(riseLength);
        enemy.SetActive(true);
        enemyMovement.StartMovingAfterRespawn(riseLength);
        agaricAnim.SetBool("isWalking", true);
        
        isRespawning = false;

    }
}
