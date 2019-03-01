using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionBar : MonoBehaviour
{
    public Image barPos;
    public Image barNeg;

    public float speedLerp;

    private float _startAmountValue;
    private float _endAmountValue;
    private float _fillRatioYellow;
    private float _fillRatioBlue;
    private bool _lerpBlue;
    private bool _lerpYellow;
    private bool _yellowToBlue;
    private bool _blueToYellow;

    private List<Sequence.BarPositionInfo> _infos = new List<Sequence.BarPositionInfo>();
    private int _infoIndex;
    private float _timer;
    private float _waitNextPlanTime;
    private bool _waitTimer;

    public void UpdateEmotionBar(float valenceValue)
    {
        _yellowToBlue = false;
        _blueToYellow = false;
        _lerpBlue = false;
        _lerpYellow = false;

        _fillRatioBlue = 0;
        _fillRatioYellow = 0;

        _startAmountValue = (barPos.fillAmount > 0) ? barPos.fillAmount : barNeg.fillAmount;

        if(valenceValue >= 0.5f)
        {
            _endAmountValue = (((valenceValue - 0.5f) * 1) / 0.5f) + 0;
        }
        else
        {
            _endAmountValue = (((valenceValue - 0) * 1) / 0.5f) + 0;
        }

        if(valenceValue >= 0.5f && barNeg.fillAmount == _startAmountValue)
        {
            _blueToYellow = true;
            _lerpBlue = true;
            _lerpYellow = true;
        }
        else if(valenceValue < 0.5f && barPos.fillAmount == _startAmountValue)
        {
            _yellowToBlue = true;
            _lerpBlue = true;
            _lerpYellow = true;
        }
        else if(valenceValue >= 0.5f && barPos.fillAmount == _startAmountValue)
        {
            _lerpYellow = true;
        }
        else
        {
            _lerpBlue = true;
        }
    }

    public void MapBarInfo(List<Sequence.BarPositionInfo> barInfos)
    {
        _timer = 0;
        _infoIndex = 0;
        _waitNextPlanTime = 0;
        _waitTimer = false;
        _infos = barInfos;
    }

    private void LerpYellow(float timeToLerp, float startValue, float endValue)
    {
        _fillRatioYellow += Time.deltaTime / timeToLerp;
        _lerpYellow = _fillRatioYellow < 1;
        barPos.fillAmount = Mathf.Lerp(startValue, endValue, _fillRatioYellow);
    }

    private void LerpBlue(float timeToLerp, float startValue, float endValue)
    {
        _fillRatioBlue += Time.deltaTime / timeToLerp;
        _lerpBlue = _fillRatioBlue < 1;
        barNeg.fillAmount = Mathf.Lerp(startValue, endValue, _fillRatioBlue);
    }

    private void Update()
    {

        if(_yellowToBlue)
        {
            if(_lerpYellow)
            {
                LerpYellow(DirectorSequencer.Instance.updateValenceTime / 2, _startAmountValue, 0);
            }
            else if(_lerpBlue)
            {
                LerpBlue(DirectorSequencer.Instance.updateValenceTime / 2, 0, _endAmountValue);
            }
        }
        else if(_blueToYellow)
        {
            if (_lerpBlue)
            {
                LerpBlue(DirectorSequencer.Instance.updateValenceTime / 2, _startAmountValue, 0);
            }
            else if (_lerpYellow)
            {
                LerpYellow(DirectorSequencer.Instance.updateValenceTime / 2, 0, _endAmountValue);
            }
        }
        else
        {
            if (_lerpYellow)
            {
                LerpYellow(DirectorSequencer.Instance.updateValenceTime,_startAmountValue, _endAmountValue);
            }
            else if(_lerpBlue)
            {
                LerpBlue(DirectorSequencer.Instance.updateValenceTime, _startAmountValue, _endAmountValue);
            }
        }
       

        if(_infoIndex < _infos.Count)
        {
            _timer += Time.deltaTime;

            _waitTimer = _timer <= _waitNextPlanTime;
            if (!_waitTimer)
            {
                Sequence.BarPositionInfo info = _infos[_infoIndex];
                transform.position = info.position;
                transform.rotation = Quaternion.Euler(info.rotation);
                if(_infoIndex + 1 < _infos.Count)
                    _waitNextPlanTime = _infos[_infoIndex + 1].keyTime;

                _waitTimer = true;
                ++_infoIndex;
            }
            
            
        }
    }

}
