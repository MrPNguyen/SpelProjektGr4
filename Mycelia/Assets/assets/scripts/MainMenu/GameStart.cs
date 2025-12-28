using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField] private GameObject GameStartUp;

    public void GameStartedUp()
    {
        Destroy(GameStartUp);
    }
}
