using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrage : MonoBehaviour
{
    public List<Sprite> calibrationImages;
    public SpriteRenderer imgRenderer;
    public int nbImgToShow = 5;
    public float timeShowImg = 10;

    private bool changeImg = true;
    private float _timer = 0;
    private int indexImg = 0;

    private void Start()
    {
        DirectorSequencer.Instance.delay = (nbImgToShow * timeShowImg) + DirectorSequencer.Instance.fadeAnimator.GetCurrentAnimatorStateInfo(0).length;
    }


    // Update is called once per frame
    void Update()
    {
        if(DirectorSequencer.Instance.play)
        {
            if (indexImg < calibrationImages.Count)
            {
                if (changeImg)
                {
                    imgRenderer.sprite = calibrationImages[indexImg];
                    ++indexImg;
                    changeImg = false;
                }
                _timer += Time.deltaTime;

                if (_timer >= timeShowImg)
                {
                    _timer = 0;
                    changeImg = true;
                }
            }
        }
        
    }
}
