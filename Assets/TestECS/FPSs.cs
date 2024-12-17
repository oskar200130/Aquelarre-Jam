using TMPro;
using UnityEngine;

public class FPSs : MonoBehaviour
{
    private const int FPS_SAMPLE_COUNT = 20;
    private readonly int[] _fpsSamples = new int[FPS_SAMPLE_COUNT];
    [SerializeField]
    private TMP_Text _fpsText;
    private int _sampleIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InvokeRepeating(nameof(UpdateFps), 0, 0.1f);
    }

    private void Update()
    {
        _fpsSamples[_sampleIndex++] = (int)(1.0f / Time.deltaTime);
        if (_sampleIndex >= FPS_SAMPLE_COUNT) _sampleIndex = 0;
    }
    private void UpdateFps()
    {
        var sum = 0;
        for (var i = 0; i < FPS_SAMPLE_COUNT; i++)
        {
            sum += _fpsSamples[i];
        }

        _fpsText.text = $"FPS: {sum / FPS_SAMPLE_COUNT}";
    }
}
