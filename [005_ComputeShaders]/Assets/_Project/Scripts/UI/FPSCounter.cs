using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public enum DisplayMode
    {
        FPS,
        MS,
    }

    [SerializeField]
    private DisplayMode _mode = DisplayMode.FPS;

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField, Range(0.1f, 2.0f)]
    private float _sampleDuration = 1.0f;

    private int _frames;
    private float _duration;
    private float _durationBest = float.MaxValue;
    private float _durationWorst;

    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        _frames++;
        _duration += frameDuration;

        if (frameDuration < _durationBest)
        {
            _durationBest = frameDuration;
        }

        if (frameDuration > _durationWorst)
        { 
            _durationWorst = frameDuration;
        }

        if (_duration >= _sampleDuration)
        {
            if (_mode == DisplayMode.FPS)
            {
                _text.SetText("FPS\n{0:0}\n{1:0}\n{2:0}", 1.0f / _durationBest, _frames / _duration, 1.0f / _durationWorst);
            }
            else
            {
                _text.SetText("MS\n{0:1}\n{1:1}\n{2:1}", 1000.0f * _durationBest, 1000.0f * _duration / _frames, 1000.0f * _durationWorst);
            }
            _frames = 0;
            _duration = 0.0f;
            _durationBest = float.MaxValue;
            _durationWorst= 0.0f;
        }
    }
}
