using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmotionBar : MonoBehaviour
{
	public GameObject background;

    public Image barPos;
    public Image barNeg;

    public TextMeshProUGUI title;
    public TextMeshProUGUI subtitle;
    public TextMeshProUGUI plus;
    public TextMeshProUGUI minus;

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

    private Vector3 _startPosition;


    private void Start()
    {
        _startPosition = GetComponent<RectTransform>().anchoredPosition;
    }

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
        _infoIndex = 0;
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

    public void SetText(string sTitle, string sSubtitle)
    {
        title.text = sTitle;
        subtitle.text = sSubtitle;
    }

    public void ShowOrHideText(bool hide)
    {
		title.gameObject.SetActive(!hide);
		subtitle.gameObject.SetActive(!hide);
		plus.gameObject.SetActive(!hide);
		minus.gameObject.SetActive(!hide);
    }

	public void ShowOrHideBackground(bool show)
	{
		background.SetActive(show);
	}

    public void ResetPosition()
    {
        GetComponent<RectTransform>().anchoredPosition = _startPosition;
    }

    public void UpdateBar(long frame)
    {
        if(frame >= _infos[_infoIndex].keyFrame)
        {
            if (_infos[_infoIndex].hide)
            {
                gameObject.SetActive(false);
            }
            else
            {
                transform.position = _infos[_infoIndex].position;
                transform.rotation = Quaternion.Euler(_infos[_infoIndex].rotation);
            }
            ++_infoIndex;
        }
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
    }

}
