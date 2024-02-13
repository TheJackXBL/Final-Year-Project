using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using System.Numerics;

public class CameraController : MonoBehaviour
{

    public static CameraController instance;

    public GameObject cameraGO;
    [SerializeField] private Camera _camera;

    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private CinemachineVirtualCamera _cameraVCAM;


    [SerializeField] private List<Collider2D> confinerColliders;
    private int currentLevelConfiner = 0;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity; 
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;



    // Start is called before the first frame update
    void Start()
    {
        confinerColliders.AddRange(GetComponentsInChildren<Collider2D>());

        if (confiner != null)
        {
            confiner.m_BoundingShape2D = confinerColliders[currentLevelConfiner];
        }
        

        _cameraVCAM.m_Lens.OrthographicSize = 7.0f;

        cinemachineBasicMultiChannelPerlin = _cameraVCAM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (instance == null)
        {
            instance = this;
        }

    }

    public void MoveCameraConfiner(float ortho)
    {
        currentLevelConfiner = currentLevelConfiner + 1;

        if (confinerColliders.Count > currentLevelConfiner)
        {
            confiner.m_BoundingShape2D = confinerColliders[currentLevelConfiner];
        }

        //_cameraVCAM.m_Lens.OrthographicSize = Mathf.Lerp(_cameraVCAM.m_Lens.OrthographicSize, ortho, 0.5f);

        ChangeCameraOrtho(ortho);
        
        
    }

    public void ChangeCameraOrtho(float ortho)
    {
        if (_cameraVCAM.m_Lens.OrthographicSize != ortho)
        {
            DOTween.To(() => _cameraVCAM.m_Lens.OrthographicSize, x => _cameraVCAM.m_Lens.OrthographicSize = x, ortho, 0.5f);
        }
    }

    public int GetCurrentLevelConfiner()
    {
        return currentLevelConfiner;
    }

    public void ShakeCamera(float intensity, float time)
    {
        
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void FixedUpdate()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, (1 - shakeTimer / shakeTimerTotal));
        }
    }
}
