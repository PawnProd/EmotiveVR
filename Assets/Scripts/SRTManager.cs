using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SRTManager : MonoBehaviour
{
    public string path;
    public TextMeshProUGUI Text;
    public TextMeshProUGUI Text2;

    [Range(0, 1)]
    public float FadeTime;

    public bool ready = false;
    public Dictionary<string, List<Subtitle>> sequenceSubtitles = new Dictionary<string, List<Subtitle>>();
    public List<Subtitle> currentSubtitles;

    public void Init(List<Sequence> sequences)
    {
        AddSubtitles(sequences);
        ready = true;
    }

    public void AddSubtitles(List<Sequence> sequences)
    {
        Reader reader = new Reader(path);
        foreach (Sequence sequence in sequences)
        {
            if (!sequenceSubtitles.ContainsKey(sequence.name))
                sequenceSubtitles.Add(sequence.name, reader.Read(sequence.name));
        }
    }

    public class Reader
    {
        private string _path;

        public Reader(string path)
        {
            _path = path;
        }

        public List<Subtitle> Read(string filename)
        {
            TextAsset srtFile = Resources.Load<TextAsset>(_path + filename);

            if (srtFile == null)
            {
                Debug.LogWarning("<b> [SRTReader] </b> SRT file " + (_path + filename +".srt") + " doesn't exists.");
                return null;
            }

            List<Subtitle> subtitles = new List<Subtitle>();
            ReaderState state = ReaderState.INDEX;
            string text = string.Empty;

            float currentFrom = 0;
            float currentTo = 0;
            int index = 0;
            string[] lines = srtFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for(int i = 0; i < lines.Length; ++i)
            {
                
                TimeSpan timeFrom;
                TimeSpan timeTo;

                switch(state)
                {
                    case ReaderState.INDEX:
                        if (int.TryParse(lines[i], out index))
                        {
                            state = ReaderState.TIMESTAMP;
                        }
                        else
                        {
                            Debug.LogWarning("<b> [SRTReader] </b> Index is not a integer at line " + (i + 1));
                        }

                    break;

                    case ReaderState.TIMESTAMP:

                        string[] parts = lines[i].Replace(',', '.').Split(new[] { "-->" }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 2)
                        {
                            if (TimeSpan.TryParse(parts[0], out timeFrom))
                            {
                                if (TimeSpan.TryParse(parts[1], out timeTo))
                                {
                                    currentFrom = (float)timeFrom.TotalSeconds;
                                    currentTo = (float)timeTo.TotalSeconds;
                                    state = ReaderState.TEXT;
                                }
                                else
                                {
                                    Debug.LogWarning("<b> [SRTReader] </b> 2nd Timestamp to is not at a correct format at line " + (i + 1));
                                }
                            }
                            else
                            {
                                Debug.LogWarning("<b> [SRTReader] </b> 1st Timestamp is not at a correct format at line " + (i + 1));
                            }
                        }
                        else
                        {
                            Debug.LogWarning("<b> [SRTReader] </b> Timestamp is missing at line " + (i + 1));
                        }

                    break;

                    case ReaderState.TEXT:

                        text += lines[i] + " ";
                        if (string.IsNullOrEmpty(lines[i]) || i == lines.Length - 1)
                        {
                            subtitles.Add(new Subtitle(index, currentFrom, currentTo, text));

                            text = string.Empty;
                            state = ReaderState.INDEX;
                        }

                    break;
                }
            }

            return subtitles;
        }
    }

    public IEnumerator Begin()
    {
        var currentlyDisplayingText = Text;
        var fadedOutText = Text2;

        currentlyDisplayingText.text = string.Empty;
        fadedOutText.text = string.Empty;

        currentlyDisplayingText.gameObject.SetActive(true);
        fadedOutText.gameObject.SetActive(true);

        yield return FadeTextOut(currentlyDisplayingText);
        yield return FadeTextOut(fadedOutText);

        var startTime = Time.time;
        Subtitle currentSubtitle = null;
        while (true)
        {
            var elapsed = Time.time - startTime;
            var subtitle = GetForTime(elapsed);
            if (subtitle != null)
            {
                if (!subtitle.Equals(currentSubtitle))
                {
                    currentSubtitle = subtitle;

                    // Swap references around
                    var temp = currentlyDisplayingText;
                    currentlyDisplayingText = fadedOutText;
                    fadedOutText = temp;

                    // Switch subtitle text
                    currentlyDisplayingText.text = currentSubtitle.text;

                    // And fade out the old one. Yield on this one to wait for the fade to finish before doing anything else.
                    StartCoroutine(FadeTextOut(fadedOutText));

                    // Yield a bit for the fade out to get part-way
                    yield return new WaitForSeconds(FadeTime / 3);

                    // Fade in the new current
                    yield return FadeTextIn(currentlyDisplayingText);
                }
                yield return null;
            }
            else
            {
                StartCoroutine(FadeTextOut(currentlyDisplayingText));
                yield return FadeTextOut(fadedOutText);
                currentlyDisplayingText.gameObject.SetActive(false);
                fadedOutText.gameObject.SetActive(false);
                yield break;
            }
        }
    }

    void OnValidate()
    {
        FadeTime = ((int)(FadeTime * 10)) / 10f;
    }

    IEnumerator FadeTextOut(TextMeshProUGUI text)
    {
        var toColor = text.color;
        toColor.a = 0;
        yield return Fade(text, toColor, Ease.OutSine);
    }

    IEnumerator FadeTextIn(TextMeshProUGUI text)
    {
        var toColor = text.color;
        toColor.a = 1;
        yield return Fade(text, toColor, Ease.InSine);
    }

    IEnumerator Fade(TextMeshProUGUI text, Color toColor, Ease ease)
    {
        yield return DOTween.To(() => text.color, color => text.color = color, toColor, FadeTime).SetEase(ease).WaitForCompletion();
    }

    public void SetSubtitles(string seqName)
    {
        if(!sequenceSubtitles.TryGetValue(seqName, out currentSubtitles))
        {
            Debug.LogError("<b> [SRTManager] </b> Sequence " + seqName + " doesn't have subtitles");
        }
    }

    public Subtitle GetForTime(float time)
    {
        if (currentSubtitles != null && currentSubtitles.Count > 0)
        {
            var subtitle = currentSubtitles[0];
            if (time >= subtitle.timeTo)
            {
                currentSubtitles.RemoveAt(0);

                if (currentSubtitles.Count == 0)
                    return null;

                subtitle = currentSubtitles[0];
            }

            if (subtitle.timeFrom > time)
                return new Subtitle(0,0,0, string.Empty);

            return subtitle;
        }
        return null;
    }

}

[System.Serializable]
public class Subtitle
{
    public int index;
    public float timeFrom;
    public float timeTo;
    public string text;

    public float Lenght
    {
        get
        {
            return timeTo - timeFrom;
        }
    }

    public Subtitle(int index, float from, float to, string text)
    {
        this.index = index;
        timeFrom = from;
        timeTo = to;
        this.text = text;
    }
}

public enum ReaderState
{
    INDEX,
    TIMESTAMP,
    TEXT
}
