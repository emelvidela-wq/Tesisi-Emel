
using UnityEngine;
using System.Collections;

public class PelotaJugador3 : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public Transform puntoAgarre;

    [Header("Agarrar pelota")]
    public float distanciaParaAgarrar = 1.5f;

    [Header("Lanzamiento")]
    public float velocidadLanzamiento = 6f;

    [Header("Sonido de lanzamiento")]
    public AudioClip sonidoLanzamiento;
    public AudioSource audioSource;

    [Header("Modo Quemado 🔥")]
    public float velocidadQuemada = 8f;
    public float duracionModoQuemado = 2f;

    [Header("Tamaño")]
    public float escalaPelota = 0.5f;

    [Header("Animación")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Daño ❤️")]
    public float tiempoEntreDaños = 0.5f;

    private Rigidbody2D rb;
    private Collider2D colliderPelota;

    private bool agarrada = false;
    private bool modoQuemado = false;

    private Vector2 ultimaDireccion = Vector2.right;

    private enum Lanzador
    {
        Ninguno,
        Drake,
        Luke
    }

    private Lanzador ultimoLanzador = Lanzador.Ninguno;

    private GameManager3 gameManager;

    private float tiempoUltimoDaño = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        colliderPelota = GetComponent<Collider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        gameManager = FindFirstObjectByType<GameManager3>();

        transform.localScale = new Vector3(
            escalaPelota,
            escalaPelota,
            escalaPelota
        );

        modoQuemado = false;
        agarrada = false;
        ultimoLanzador = Lanzador.Ninguno;

        DetenerAnimacion();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 direccionMovimiento = new Vector2(
            horizontal,
            vertical
        );

        if (direccionMovimiento != Vector2.zero)
            ultimaDireccion = direccionMovimiento.normalized;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!agarrada)
                IntentarAgarrar();
            else
                Lanzar();
        }

        if (agarrada)
        {
            if (puntoAgarre != null)
                transform.position = puntoAgarre.position;

            DetenerAnimacion();
        }
        else
        {
            if (rb != null && rb.linearVelocity.magnitude > 0.1f)
            {
                if (!modoQuemado)
                    ReproducirAnimacionNormal();
            }
            else if (!modoQuemado)
            {
                DetenerAnimacion();
            }
        }
    }

    void IntentarAgarrar()
    {
        if (modoQuemado || agarrada || puntoAgarre == null)
            return;

        float distancia = Vector2.Distance(
            transform.position,
            puntoAgarre.position
        );

        if (distancia <= distanciaParaAgarrar)
            Agarrar();
    }

    void Agarrar()
    {
        if (modoQuemado || rb == null)
            return;

        agarrada = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        if (colliderPelota != null)
            colliderPelota.enabled = false;

        transform.SetParent(puntoAgarre);
        transform.localPosition = Vector3.zero;

        ReproducirAnimacionNormal();
    }

    void Lanzar()
    {
        if (!agarrada || rb == null)
            return;

        agarrada = false;
        transform.SetParent(null);

        rb.simulated = true;

        if (colliderPelota != null)
            colliderPelota.enabled = true;

        ultimoLanzador = Lanzador.Drake;

        ReproducirSonidoLanzamiento();

        ActivarModoQuemado(ultimaDireccion);
    }

    public void RegistrarLanzamientoLuke()
    {
        ultimoLanzador = Lanzador.Luke;
    }

    public void ReproducirSonidoLanzamiento()
    {
        if (audioSource != null && sonidoLanzamiento != null)
            audioSource.PlayOneShot(sonidoLanzamiento);
    }

    public void ActivarModoQuemado(Vector2 direccion)
    {
        if (rb == null)
            return;

        if (direccion != Vector2.zero)
            ultimaDireccion = direccion.normalized;

        StopCoroutine(nameof(ModoQuemado));
        StartCoroutine(ModoQuemado());
    }

    IEnumerator ModoQuemado()
    {
        modoQuemado = true;

        rb.linearVelocity = ultimaDireccion * velocidadQuemada;

        if (spriteRenderer != null)
            spriteRenderer.flipX = ultimaDireccion.x < 0;

        if (animator != null)
        {
            animator.Play("modoquemado", 0, 0f);
            animator.speed = 1f;
        }

        yield return new WaitForSeconds(duracionModoQuemado);

        modoQuemado = false;

        if (rb != null)
            rb.linearVelocity = ultimaDireccion * velocidadLanzamiento;

        ReproducirAnimacionNormal();
    }

    public bool EstaQuemada()
    {
        return modoQuemado;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Solo puede hacer daño durante el modo quemado.
        if (!modoQuemado)
            return;

        // Evita registrar varios golpes casi simultáneos.
        if (Time.time - tiempoUltimoDaño < tiempoEntreDaños)
            return;

        // Buscar el GameManager si todavía no se encontró.
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager3>();

        if (gameManager == null)
        {
            Debug.LogError("No se encontró el GameManager3.");
            return;
        }

        // Detectar a Drake, incluso si el collider está en un hijo.
        bool golpeoDrake =
            collision.gameObject.CompareTag("Player") ||
            collision.gameObject.GetComponentInParent<MovimientoJugador3>() != null;

        // Detectar a Luke, incluso si el collider está en un hijo.
        bool golpeoLuke =
            collision.gameObject.CompareTag("Bot") ||
            collision.gameObject.GetComponentInParent<Oponente1IA>() != null;

        // Luke lanzó la pelota y golpeó a Drake.
        if (golpeoDrake && ultimoLanzador == Lanzador.Luke)
        {
            tiempoUltimoDaño = Time.time;

            gameManager.DanarDrake();

            Debug.Log("🔥 Luke golpeó a Drake. La vida fue descontada.");
        }
        // Drake lanzó la pelota y golpeó a Luke.
        else if (golpeoLuke && ultimoLanzador == Lanzador.Drake)
        {
            tiempoUltimoDaño = Time.time;

            gameManager.DanarLuke();

            Debug.Log("🔥 Drake golpeó a Luke. La vida fue descontada.");
        }
    }

    void ReproducirAnimacionNormal()
    {
        if (animator == null)
            return;

        animator.speed = 1f;
        animator.Play("PelotaAnim3", 0, 0f);
    }

    void DetenerAnimacion()
    {
        if (animator == null)
            return;

        animator.speed = 0f;
    }
}
