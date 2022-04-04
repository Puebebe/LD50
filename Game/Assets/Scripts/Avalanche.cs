using System;
using UnityEngine;

public class Avalanche : MonoBehaviour
{
    public static event Action OnCatchUpWithPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            OnCatchUpWithPlayer.Invoke();
    }
}
