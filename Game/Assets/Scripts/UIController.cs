using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    GameObject GameOverUI;

    public void SetGameOverScreen(bool state)
    {
        GameOverUI.SetActive(state);
    }
}
