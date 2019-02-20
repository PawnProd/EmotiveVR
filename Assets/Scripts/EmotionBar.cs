using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionBar : MonoBehaviour
{
    public Image bar;

    public float speedLerp;

    private float _startAmountValue;
    private float _endAmountValue;
    private float _fillRatio;
    private bool _lerp;

    private List<Sequence.BarPositionInfo> _infos = new List<Sequence.BarPositionInfo>();
    private int _infoIndex;
    private float _timer;
    private float _waitNextPlanTime;
    private bool _waitTimer;

    public void UpdateEmotionBar(float valenceValue)
    {
        Debug.Log("Start Amount = " + bar.fillAmount);
        Debug.Log("End Amount = " + valenceValue);
        _startAmountValue = bar.fillAmount;
        _endAmountValue = valenceValue;
        _lerp = true;
        _fillRatio = 0;

    }

    public void MapBarInfo(List<Sequence.BarPositionInfo> barInfos)
    {
        _timer = 0;
        _infoIndex = 0;
        _waitNextPlanTime = 0;
        _waitTimer = false;
        _infos = barInfos;
    }

    private void Update()
    {
        if(_lerp)
        {
            _fillRatio += Time.deltaTime * speedLerp;

            _lerp = _fillRatio < 1;

            bar.fillAmount = Mathf.Lerp(_startAmountValue, _endAmountValue, _fillRatio);
        }

        if(_infoIndex < _infos.Count)
        {
            _timer += Time.deltaTime;

            _waitTimer = _timer <= _waitNextPlanTime;
            if (!_waitTimer)
            {
                Debug.Log("Info Index = " + _infoIndex);
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
