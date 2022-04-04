using UnityEngine;

public class GameController : MonoBehaviour
{
    void Start()
    {
        Avalanche.OnCatchUpWithPlayer += GameOver;
    }

    void GameOver()
    {
        Debug.LogWarning("Game over!");
    }

    void OnDestroy()
    {
        Avalanche.OnCatchUpWithPlayer -= GameOver;
    }
}
