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
        FindObjectOfType<UIController>().SetGameOverScreen(true);
    }

    void OnDestroy()
    {
        Avalanche.OnCatchUpWithPlayer -= GameOver;
    }
}
