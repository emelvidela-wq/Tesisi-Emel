
using UnityEngine;

public class ZonaPunto : MonoBehaviour
{
    public bool ladoJugador;

    [Header("Sonido del punto")]
    public AudioSource audioSource;
    public AudioClip sonidoSilbato;

    private gameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<gameManager>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        if (gameManager == null)
            Debug.LogError("No se encontró gameManager en la escena.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Pelota"))
            return;

        if (audioSource != null && sonidoSilbato != null)
            audioSource.PlayOneShot(sonidoSilbato);

        if (gameManager == null)
            return;

        if (ladoJugador)
            gameManager.PuntoBot();
        else
            gameManager.PuntoJugador();
    }
}