using UnityEngine;

public class Metronome : MonoBehaviour
{
    public float frequency = 440f;
    public float duration = 0.1f;
    private AudioSource audioSource;
    private AudioClip beep;
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        BeatManager._instance.OnPulse.AddListener(OnPulse);
        BeatManager._instance.OnMeasure.AddListener(OnMeasure);

        int sampleRate = 44100; // Frecuencia de muestreo estándar
        int samples = Mathf.FloorToInt(duration * sampleRate);
        float[] samplesArray = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float time = (float)i / sampleRate;
            samplesArray[i] = Mathf.Sin(2 * Mathf.PI * frequency * time); //seno
        }

        beep = AudioClip.Create("Beep", samples, 1, sampleRate, false);
        beep.SetData(samplesArray, 0);
    }

    void OnPulse()
    {
        //Debug.Log($"Pulso {BeatManager._instance.current_beat + 1}/{BeatManager._instance.numerador}. Llamado desde BeatManager._instance.OnPulse");
        if (BeatManager._instance.current_measure > 0)
        {
            audioSource.pitch = 1f; audioSource.PlayOneShot(beep);
        }
    }

    void OnMeasure()
    {
        //Debug.Log($"Compás N.{BeatManager._instance.current_measure}. Llamado desde BeatManager._instance.OnMeasure");
        audioSource.pitch = 1.5f;
        audioSource.PlayOneShot(beep);
    }
}
