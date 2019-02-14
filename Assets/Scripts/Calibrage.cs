using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrage : MonoBehaviour
{
    public List<Sprite> allCalibrationImages;

    public SpriteRenderer imgRenderer;

    public int nbImgToShow = 5;

    public float timeShowImg = 10;

    private bool _calibrationReady = false;
    private bool changeImg = true;
    private float _timer = 0;
    private int indexImg = 0;
    private List<Sprite> _showImg = new List<Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < nbImgToShow; i++)
        {
            int randomIndex = Random.Range(0, allCalibrationImages.Count);

            if(!_showImg.Contains(allCalibrationImages[randomIndex]))
            {
                _showImg.Add(allCalibrationImages[randomIndex]);
            }
        }

        DirectorSequencer.Instance.delay = nbImgToShow * timeShowImg;
        _calibrationReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_calibrationReady && indexImg < _showImg.Count)
        {
            if(changeImg)
            {
                imgRenderer.sprite = _showImg[indexImg];
                ++indexImg;
                changeImg = false;
            }
            Debug.Log("Timer = " + _timer);
            _timer += Time.deltaTime;

            if(_timer >= timeShowImg)
            {
                _timer = 0;
                changeImg = true;
            }
        }
    }
}
