using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoBehaviour
{
    public PostProcessLayer layer;
    public PostProcessVolume volume;

    public float speed;

    public Color positiveValenceColor;
    public Color negativeValenceColor;
    public Color neutralColor;

    private ColorGrading _colorGradingLayer;

    private float _colorRatio;
    private Color _filterColorStart;
    private Color _filterColorEnd;
    private bool _lerpFilterColor;  

    private void Awake()
    {
        layer = GetComponent<PostProcessLayer>();
        volume = GetComponent<PostProcessVolume>();

        
    }

    public void SetPostProcess(bool active, PostProcessProfile profile)
    {
        layer.enabled = active;
        volume.enabled = active;
        volume.profile = profile;

    }

    public void UpdateFilterColor(float valence)
    {
        if(_colorGradingLayer == null)
        {
            volume.profile.TryGetSettings(out _colorGradingLayer);
            _colorGradingLayer.enabled.value = true;
        }
        Debug.Log("Update Color ! = " + valence);
        _colorRatio = 0;
        _filterColorStart = _colorGradingLayer.colorFilter.value;
        _filterColorEnd = (valence < 0.5f) ? InterpolateColor(valence, negativeValenceColor, neutralColor) : InterpolateColor(valence, neutralColor, positiveValenceColor);

        Debug.Log("End color = (" + _filterColorEnd.r + ", " + _filterColorEnd.g + ", " + _filterColorEnd.b + ")");
        _lerpFilterColor = true;

    }

    public Color InterpolateColor(float valence, Color minColor, Color maxColor)
    {
        float r = InterpolateValence(valence, minColor.r, maxColor.r);
        float g = InterpolateValence(valence, minColor.g, maxColor.g);
        float b = InterpolateValence(valence, minColor.b, maxColor.b);

        return new Color(r, g, b, 1);
    }

    private float InterpolateValence(float valence, float newMinRange, float newMaxRange)
    {
        return (valence * (newMaxRange - newMinRange)) / 1 + newMinRange;
    }


    private void Update()
    {
        if(_lerpFilterColor)
        {
            _lerpFilterColor = _colorRatio < 1;
            _colorRatio += Time.deltaTime * speed;
            _colorGradingLayer.colorFilter.value = Color.Lerp(_filterColorStart, _filterColorEnd, _colorRatio);
        }
    }
}
