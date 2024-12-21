using UnityEngine;

public class SpeakerParticle : MonoBehaviour
{
    [SerializeField]
    ParticleSystem musicParticles;
    [SerializeField]
    ParticleSystem terribleParticles;
    [SerializeField]
    ParticleSystem coolParticles;
    [SerializeField]
    ParticleSystem perfectParticles;
    [SerializeField]
    ParticleSystem heavyParticles;


    public void playParticle(SCORE score)
    {
        musicParticles.Play();
        switch (score)
        {
            case SCORE.HEAVY:
                heavyParticles.Play();
                break;
            case SCORE.PERFECT:
                perfectParticles.Play();
                break;
            case SCORE.COOL:
                coolParticles.Play();
                break;
            case SCORE.TERRIBLE:
                terribleParticles.Play();
                break;
            case SCORE.NONE:
                break;
        }
    }
}
