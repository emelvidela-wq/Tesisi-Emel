
using UnityEngine;

public class Pelota : MonoBehaviour
{
    [Header("Física de la pelota")]
    public float fuerzaHorizontal = 5.5f;
    public float fuerzaVertical = 7f;
    public float velocidadMaxima = 10f;

    [Header("Sonido del golpe")]
    public AudioSource audioSource;
    public AudioClip sonidoGolpe;

    [Header("Sonido del silbato")]
    public AudioClip sonidoSilbato;

    private Rigidbody2D rb;
    private gameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

    void FixedUpdate()
    {
        if (rb != null &&
            rb.linearVelocity.magnitude > velocidadMaxima)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized * velocidadMaxima;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameManager != null)
                gameManager.ToqueJugador();

            ReproducirSonidoGolpe();
            MoverPelota(collision);
        }
        else if (collision.gameObject.CompareTag("Bot"))
        {
            if (gameManager != null)
                gameManager.ToqueBot();

            ReproducirSonidoGolpe();
            MoverPelota(collision);
        }
        else if (collision.gameObject.CompareTag("red"))
        {
            Debug.Log("La pelota tocó la red.");
        }
    }

    void ReproducirSonidoGolpe()
    {
        if (audioSource != null && sonidoGolpe != null)
            audioSource.PlayOneShot(sonidoGolpe);
    }

    void MoverPelota(Collision2D collision)
    {
        if (rb == null)
            return;

        float direccion =
            collision.transform.position.x < transform.position.x
            ? 1f : -1f;

        float velocidadVertical = rb.linearVelocity.y;

        rb.linearVelocity = new Vector2(
            direccion * fuerzaHorizontal,
            velocidadVertical * 0.35f
        );

        rb.AddForce(
            new Vector2(0f, fuerzaVertical),
            ForceMode2D.Impulse
        );
    }
}