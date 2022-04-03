using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class DistanceShakeAware : MonoBehaviour
{
    [SerializeField]
    private GameObject referenceGameObject;
    [SerializeField]
    private AnimationCurve shakeLooseCurve;
    [SerializeField]
    private float zeroShakeDistance;
    private CinemachineBasicMultiChannelPerlin multiChannelPerlin;

    // Start is called before the first frame update
    void Start()
    {
        multiChannelPerlin = gameObject.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
    }

    // Update is called once per frame
    void Update()
    {
        var distance = Vector3.Distance(transform.position, referenceGameObject.transform.position);
        var progressToShakeLoose = Mathf.Max(1 - distance / zeroShakeDistance, 0f);
        multiChannelPerlin.m_AmplitudeGain = shakeLooseCurve.Evaluate(progressToShakeLoose);
    }
}
