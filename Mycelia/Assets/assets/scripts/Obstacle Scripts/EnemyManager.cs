using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject enemy;
    [SerializeField] private float SecondsToWait;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.activeSelf == false)
        {
            StartCoroutine(RespawnEnemy());
        }
    }

    IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(SecondsToWait);
        enemy.SetActive(true);
    }
}
