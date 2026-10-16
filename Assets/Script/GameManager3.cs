
using UnityEngine;

public class GameManager3 : MonoBehaviour
{
    [Header("Estado del nivel")]
    public bool nivelIniciado = false;

    [Header("Marcadores de vidas")]
    public MarcadorVidasDrake marcadorDrake;
    public MarcadorVidasLuke marcadorLuke;

    [Header("Sonido al perder un corazón")]
    public AudioSource audioSource;
    public AudioClip sonidoPerderCorazon;

    void Awake()
    {
        nivelIniciado = false;
        Debug.Log("GameManager3 iniciado.");
    }

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void ReproducirSonidoPerderCorazon()
    {
        if (audioSource != null && sonidoPerderCorazon != null)
        {
            audioSource.PlayOneShot(sonidoPerderCorazon);
        }
    }

    public void IniciarNivel()
    {
        nivelIniciado = true;
        Debug.Log("El nivel 3 está iniciado.");
    }

    public void DanarDrake()
    {
        if (marcadorDrake == null)
        {
            Debug.LogError(
                "Falta asignar el marcador de Drake en el nivel 3."
            );
            return;
        }

        marcadorDrake.PerderVida();
        ReproducirSonidoPerderCorazon();

        Debug.Log("Drake perdió una vida en el nivel 3.");
    }

    public void DanarLuke()
    {
        if (marcadorLuke == null)
        {
            Debug.LogError(
                "Falta asignar el marcador de Luke en el nivel 3."
            );
            return;
        }

        marcadorLuke.PerderVida();
        ReproducirSonidoPerderCorazon();

        Debug.Log("Luke perdió una vida en el nivel 3.");
    }
}
