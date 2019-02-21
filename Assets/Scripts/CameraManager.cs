using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoBehaviour
{
    public PostProcessLayer layer;
    public PostProcessVolume volume;

    public float speed;

    private ColorGrading _colorGradingLayer;

    private float _luminosityRatio;
    private float _postExpStart;
    private float _postExpEnd;
    private bool _lerpLuminosity;  

    private void Start()
    {
        layer = GetComponent<PostProcessLayer>();
        volume = GetComponent<PostProcessVolume>();

        volume.profile.TryGetSettings(out _colorGradingLayer);
    }

    public void SetPostProcess(bool active)
    {
        layer.enabled = active;
        volume.enabled = active;

        _colorGradingLayer.enabled.value = active;
    }

    public void UpdateLuminosity(float valence)
    {
        _postExpStart = _colorGradingLayer.postExposure.value;
        _postExpEnd = (((valence - 0) * (3 - 1)) / (1 - 0)) + 1;
        _luminosityRatio = 0;
        _lerpLuminosity = true;
    }


    private void Update()
    {
        if(_lerpLuminosity)
        {
            _lerpLuminosity = _luminosityRatio < 1;
            Debug.Log("Luminosity Ratio = " + _luminosityRatio);
            _luminosityRatio += Time.deltaTime * speed;
            _colorGradingLayer.postExposure.value = Mathf.Lerp(_postExpStart, _postExpEnd, _luminosityRatio);
        }
    }
}
