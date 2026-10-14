
using UnityEngine;

public class Pelota : MonoBehaviour
{
    [Header("FÍSICA DE LA PELOTA")]
    public float fuerzaHorizontal = 5.5f;
    public float fuerzaVertical = 7f;
    public float velocidadMaxima = 10f;

    [Header("ZONAS DEL PISO")]
    public GameObject zonaJugador;
    public GameObject zonaBot;

    [Header("CONTROL DE TOQUES")]
    public Transform lineaCentral;

    [Header("SONIDO DEL GOLPE")]
    public AudioSource audioSource;
    public AudioClip sonidoGolpe;

    [Header("SONIDO DEL SILBATO")]
    public AudioClip sonidoSilbato;

    private Rigidbody2D rb;
    private gameManager manager;

    private int toquesJugador = 0;
    private int toquesBot = 0;

    // 0 = nadie, 1 = Drake, 2 = Ada.
    private int ultimoJugadorQueToco = 0;

    private bool reiniciando = false;
    private bool puntoRegistrado = false;

    private int ladoAnterior;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        manager = FindFirstObjectByType<gameManager>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        if (manager == null)
            Debug.LogError("No se encontró gameManager en la escena.");

        if (lineaCentral != null)
        {
            ladoAnterior =
                transform.position.x < lineaCentral.position.x ? -1 : 1;
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (manager != null && manager.PartidoTerminado)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }

        if (rb.linearVelocity.magnitude > velocidadMaxima)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized * velocidadMaxima;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (reiniciando || manager == null)
            return;

        if (manager.PartidoTerminado || puntoRegistrado)
            return;

        // La pelota cayó del lado de Drake: punto para Ada.
        if (PerteneceAZona(collision.gameObject, zonaJugador))
        {
            puntoRegistrado = true;
            Debug.Log("La pelota cayó del lado de Drake.");
            manager.PuntoBot();
            return;
        }

        // La pelota cayó del lado de Ada: punto para Drake.
        if (PerteneceAZona(collision.gameObject, zonaBot))
        {
            puntoRegistrado = true;
            Debug.Log("La pelota cayó del lado de Ada.");
            manager.PuntoJugador();
            return;
        }

        // Contacto con Drake.
        if (manager.EsJugador(collision.transform))
        {
            // Desbloquear la jugada si venimos de un punto anterior.
            manager.NuevaJugada();

            RegistrarCambioDeJugador(1);

            if (!manager.ToqueJugador())
                return;

            ReproducirSonidoGolpe();
            MoverPelota(collision);
            return;
        }

        // Contacto con Ada.
        if (manager.EsBot(collision.transform))
        {
            // Desbloquear la jugada si venimos de un punto anterior.
            manager.NuevaJugada();

            RegistrarCambioDeJugador(2);

            if (!manager.ToqueBot())
                return;

            ReproducirSonidoGolpe();
            MoverPelota(collision);
            return;
        }

        if (collision.gameObject.CompareTag("red"))
        {
            Debug.Log("La pelota tocó la red.");
        }
    }

    void RegistrarCambioDeJugador(int jugadorActual)
    {
        if (ultimoJugadorQueToco == jugadorActual)
            return;

        // El jugador que recibe la pelota empieza su contador en cero.
        if (jugadorActual == 1)
        {
            toquesJugador = 0;
            Debug.Log("Drake recibió la pelota. Contador reiniciado.");
        }
        else
        {
            toquesBot = 0;
            Debug.Log("Ada recibió la pelota. Contador reiniciado.");
        }

        ultimoJugadorQueToco = jugadorActual;
    }

    bool PerteneceAZona(GameObject objeto, GameObject zona)
    {
        if (zona == null || objeto == null)
            return false;

        return objeto == zona ||
               objeto.transform.IsChildOf(zona.transform);
    }

    public int RegistrarToqueJugador()
    {
        toquesJugador++;
        return toquesJugador;
    }

    public int RegistrarToqueBot()
    {
        toquesBot++;
        return toquesBot;
    }

    void ReiniciarContadores()
    {
        toquesJugador = 0;
        toquesBot = 0;
        ultimoJugadorQueToco = 0;
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

    public void ReiniciarParaSaque(Transform posicionSaque)
    {
        if (posicionSaque == null || rb == null)
        {
            Debug.LogError("Falta asignar la posición de saque.");
            return;
        }

        reiniciando = true;

        ReiniciarContadores();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.position = posicionSaque.position;
        transform.position = posicionSaque.position;

        if (lineaCentral != null)
        {
            ladoAnterior =
                rb.position.x < lineaCentral.position.x ? -1 : 1;
        }

        Physics2D.SyncTransforms();

        // Preparar la pelota para la próxima jugada.
        puntoRegistrado = false;

        reiniciando = false;

        Debug.Log("Pelota preparada para el saque.");
    }
}
