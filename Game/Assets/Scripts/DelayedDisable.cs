using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DelayedDisable : MonoBehaviour
{
    [SerializeField]
    private int delaySeconds;

    void Start()
    {
        StartCoroutine(ExecuteAfterTime());
    }

    private IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(delaySeconds);
        gameObject.SetActive(false);
    }
}
