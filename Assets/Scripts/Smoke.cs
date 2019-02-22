using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public float speed;

    private ParticleSystem _pSystem;

    private Gradient _startColor1;
    private Gradient _startColor2;

    private float _lerpRatio;

    private void Start()
    {
        _pSystem = GetComponent<ParticleSystem>();
        _startColor1 = _pSystem.main.startColor.gradientMin;
        _startColor2 = _pSystem.main.startColor.gradientMax;

    }
    // Update is called once per frame
    void Update()
    {
        if(_lerpRatio < 1)
        {
            _lerpRatio += Time.deltaTime * speed;
            ParticleSystem.MinMaxGradient color = _pSystem.main.startColor;
         

        }
    }
}
