using UnityEngine;

public class SpeakerParticle : MonoBehaviour
{
    [SerializeField]
    ParticleSystem _particleSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playParticle()
    {
        //_particleSystem.Clear();
        //_particleSystem.Stop();
        _particleSystem.Play();
    }
}
