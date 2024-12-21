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


    public void playParticle()
    {
        musicParticles.Play();

        // MIERDA DE DIFERENCIAR EN FUNCION A LA PUNTUACION, QUE NO SE HACERLO
        // SEGURO QUE ES FACIL A ALGUIEN SE LE HABRA OCURRIDO HACER UN SINGLETON PARA SACAR LA PUNTUACION ACTUAL O ALGO
        // QUE ALGUIEN LLAME A ALGUIEN
        terribleParticles.Play();
        coolParticles.Play();
        perfectParticles.Play();
        heavyParticles.Play();
    }
}
