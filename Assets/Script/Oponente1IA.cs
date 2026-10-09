
using UnityEngine;
using System.Collections;

public class Oponente1IA : MonoBehaviour
{
    [Header("Referencias")]
    public Transform pelota;
    public Transform puntoAgarreOponente;
    public Transform drake;

    [Header("Movimiento")]
    public float velocidad = 4f;
    public float velocidadPersecucion = 5f;

    [Header("Búsqueda de pelota")]
    public float rangoBusquedaPelota = 10f;
    public float distanciaAgarrar = 1.5f;
    public float distanciaDeteccion = 5f;

    [Header("Lanzamiento")]
    public float velocidadLanzamiento = 8f;

    [Header("Movimiento aleatorio")]
    public float tiempoCambioDireccion = 2f;
    public float rangoMovimiento = 2f;

    [Header("Límites y zona segura")]
    public float limiteIzquierdo = 0f;
    public float limiteDerecho = 10f;
    public float lineaCentral = 0f;
    public float distanciaSeguraLimites = 2.5f;
    public float margenGiro = 1.5f;
    public float fuerzaAlejarseLimites = 2.5f;

    [Header("Esquivar modo quemado")]
    public float distanciaEsquivarQuemada = 5f;
    public float fuerzaEsquivarQuemada = 1.5f;
    public float distanciaSeguraQuemado = 2f;

    [Header("Detección del cuerpo")]
    public Collider2D cuerpoLuke;
    public float margenAgarrar = 0.4f;

    [Header("Animación")]
    public Animator animator;
    public string animacionQuieto = "StateO3";
    public string animacionCorrer = "RunO3";
    public string animacionAgachado = "CrouchO3";

    private Rigidbody2D rb;
    private Rigidbody2D rbPelota;
    private Collider2D colliderPelota;
    private Collider2D colliderLuke;

    private PelotaJugador3 pelotaScript;

    private Vector2 movimiento;
    private Vector2 direccionAleatoria;

    private bool tienePelota = false;
    private bool lanzando = false;

    private float tiempoMovimiento;
    private string animacionActual = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        colliderLuke = GetComponent<Collider2D>();

        if (cuerpoLuke == null)
            cuerpoLuke = GetComponent<Collider2D>();

        if (pelota != null)
        {
            rbPelota = pelota.GetComponent<Rigidbody2D>();
            colliderPelota = pelota.GetComponent<Collider2D>();
            pelotaScript = pelota.GetComponent<PelotaJugador3>();
        }

        if (animator == null)
            animator = GetComponent<Animator>();

        CambiarDireccionAleatoria();
        ActualizarAnimacion();
    }

    void Update()
    {
        if (pelota == null || rbPelota == null || rb == null)
        {
            movimiento = Vector2.zero;
            ActualizarAnimacion();
            return;
        }

        // Si Luke tiene la pelota, la lanza.
        if (tienePelota)
        {
            movimiento = Vector2.zero;
            ActualizarAnimacion();
            LanzarPelota();
            return;
        }

        // Esquivar la pelota durante el modo quemado.
        if (pelotaScript != null && pelotaScript.EstaQuemada())
        {
            EsquivarPelotaQuemada();
            ActualizarAnimacion();
            return;
        }

        // Intentar agarrar la pelota.
        if (PuedeAgarrarPelota())
        {
            AgarrarPelota();
            ActualizarAnimacion();
            return;
        }

        // Perseguir la pelota.
        if (DebeIrPorLaPelota())
        {
            BuscarPelota();
            ActualizarAnimacion();
            return;
        }

        // Esquivar si la pelota se acerca.
        if (PelotaVieneHaciaMi())
        {
            EsquivarPelota();
            ActualizarAnimacion();
            return;
        }

        // Patrullar si no hay otras acciones.
        MovimientoAleatorio();
        ActualizarAnimacion();
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (tienePelota || lanzando)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }

        if (transform.position.x <= limiteIzquierdo &&
            movimiento.x < 0f)
        {
            movimiento.x = 0f;
        }

        if (transform.position.x >= limiteDerecho &&
            movimiento.x > 0f)
        {
            movimiento.x = 0f;
        }

        rb.linearVelocity = movimiento * velocidad;
    }

    // DETECTAR SI PUEDE AGARRAR LA PELOTA

    bool PuedeAgarrarPelota()
    {
        if (pelota == null || rbPelota == null)
            return false;

        if (!rbPelota.simulated)
            return false;

        if (pelotaScript != null && pelotaScript.EstaQuemada())
            return false;

        if (cuerpoLuke != null)
        {
            Vector2 puntoCercano =
                cuerpoLuke.ClosestPoint(pelota.position);

            float distanciaBorde = Vector2.Distance(
                puntoCercano,
                pelota.position
            );

            if (distanciaBorde <= margenAgarrar)
                return true;
        }

        float distanciaCentro = Vector2.Distance(
            transform.position,
            pelota.position
        );

        return distanciaCentro <= distanciaAgarrar;
    }

    // DECIDIR SI DEBE BUSCAR LA PELOTA

    bool DebeIrPorLaPelota()
    {
        if (pelota == null || rbPelota == null)
            return false;

        if (!rbPelota.simulated)
            return false;

        if (pelotaScript != null && pelotaScript.EstaQuemada())
            return false;

        float distancia = Vector2.Distance(
            transform.position,
            pelota.position
        );

        return distancia <= rangoBusquedaPelota;
    }

    // PERSEGUIR LA PELOTA

    void BuscarPelota()
    {
        Vector2 posicion = transform.position;
        Vector2 destino = pelota.position;

        Vector2 direccion = (destino - posicion).normalized;

        if (posicion.x <= limiteIzquierdo + margenGiro &&
            direccion.x < 0f)
        {
            direccion.x = Mathf.Abs(direccion.x);
        }

        if (posicion.x >= limiteDerecho - margenGiro &&
            direccion.x > 0f)
        {
            direccion.x = -Mathf.Abs(direccion.x);
        }

        movimiento = direccion.normalized;
        rb.linearVelocity = movimiento * velocidadPersecucion;
    }

    // DETECTAR SI LA PELOTA VIENE HACIA LUKE

    bool PelotaVieneHaciaMi()
    {
        if (pelota == null || rbPelota == null)
            return false;

        if (!rbPelota.simulated)
            return false;

        Vector2 velocidadPelota = rbPelota.linearVelocity;

        if (velocidadPelota.sqrMagnitude < 0.01f)
            return false;

        Vector2 haciaLuke = (
            (Vector2)transform.position -
            (Vector2)pelota.position
        ).normalized;

        bool vieneHaciaLuke =
            Vector2.Dot(velocidadPelota.normalized, haciaLuke) > 0.5f;

        float distancia = Vector2.Distance(
            transform.position,
            pelota.position
        );

        return vieneHaciaLuke && distancia <= distanciaDeteccion;
    }

    // ESQUIVAR PELOTA NORMAL

    void EsquivarPelota()
    {
        Vector2 direccion = AplicarZonaSegura(Vector2.up);
        movimiento = direccion.normalized;
    }

    // ESQUIVAR PELOTA EN MODO QUEMADO

    void EsquivarPelotaQuemada()
    {
        if (pelota == null || rbPelota == null ||
            !rbPelota.simulated)
        {
            movimiento = Vector2.zero;
            return;
        }

        Vector2 posicion = transform.position;
        Vector2 posicionPelota = pelota.position;

        Vector2 alejamiento = posicion - posicionPelota;

        if (alejamiento.sqrMagnitude > 0.001f)
            alejamiento.Normalize();
        else
            alejamiento = Vector2.up;

        float distancia = Vector2.Distance(
            posicion,
            posicionPelota
        );

        float multiplicador =
            distancia <= distanciaEsquivarQuemada
            ? fuerzaEsquivarQuemada
            : 1f;

        float centro = (limiteIzquierdo + limiteDerecho) / 2f;

        Vector2 volverAlCentro = new Vector2(
            centro - posicion.x,
            0f
        );

        if (volverAlCentro.sqrMagnitude > 0.001f)
            volverAlCentro.Normalize();

        Vector2 direccionFinal =
            alejamiento +
            volverAlCentro * fuerzaAlejarseLimites;

        direccionFinal = AplicarZonaSegura(direccionFinal);

        movimiento = direccionFinal.normalized;

        rb.linearVelocity =
            movimiento * velocidad * multiplicador;
    }

    // MANTENERSE ALEJADO DE LOS LÍMITES

    Vector2 AplicarZonaSegura(Vector2 direccion)
    {
        float x = transform.position.x;

        float distanciaIzquierda = x - limiteIzquierdo;
        float distanciaDerecha = limiteDerecho - x;

        if (distanciaIzquierda < distanciaSeguraLimites)
        {
            float influencia = 1f - Mathf.Clamp01(
                distanciaIzquierda /
                Mathf.Max(distanciaSeguraLimites, 0.01f)
            );

            direccion.x += influencia * fuerzaAlejarseLimites;
        }

        if (distanciaDerecha < distanciaSeguraLimites)
        {
            float influencia = 1f - Mathf.Clamp01(
                distanciaDerecha /
                Mathf.Max(distanciaSeguraLimites, 0.01f)
            );

            direccion.x -= influencia * fuerzaAlejarseLimites;
        }

        if (x <= limiteIzquierdo + margenGiro)
            direccion.x = Mathf.Max(direccion.x, 0.5f);

        if (x >= limiteDerecho - margenGiro)
            direccion.x = Mathf.Min(direccion.x, -0.5f);

        return direccion;
    }

    // MOVIMIENTO ALEATORIO CON PREFERENCIA POR EL CENTRO

    void MovimientoAleatorio()
    {
        tiempoMovimiento -= Time.deltaTime;

        float centro = (limiteIzquierdo + limiteDerecho) / 2f;
        float x = transform.position.x;

        if (tiempoMovimiento <= 0f)
            CambiarDireccionAleatoria();

        float diferenciaCentro = centro - x;

        Vector2 direccionCentro = new Vector2(
            Mathf.Clamp(diferenciaCentro, -3f, 3f),
            0f
        );

        movimiento = (
            direccionAleatoria + direccionCentro
        ).normalized;

        movimiento = AplicarZonaSegura(movimiento).normalized;

        if (x <= limiteIzquierdo + margenGiro)
            movimiento.x = Mathf.Abs(movimiento.x);

        if (x >= limiteDerecho - margenGiro)
            movimiento.x = -Mathf.Abs(movimiento.x);
    }

    void CambiarDireccionAleatoria()
    {
        tiempoMovimiento = tiempoCambioDireccion;

        direccionAleatoria = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-0.2f, 0.2f)
        ).normalized;
    }

    // AGARRAR LA PELOTA

    void AgarrarPelota()
    {
        if (pelota == null || rbPelota == null || tienePelota)
            return;

        if (pelotaScript != null && pelotaScript.EstaQuemada())
            return;

        tienePelota = true;

        Debug.Log("LUKE AGARRÓ LA PELOTA");

        movimiento = Vector2.zero;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rbPelota.linearVelocity = Vector2.zero;
        rbPelota.angularVelocity = 0f;
        rbPelota.simulated = false;

        if (colliderPelota != null)
            colliderPelota.enabled = false;

        Transform punto = puntoAgarreOponente != null
            ? puntoAgarreOponente
            : transform;

        pelota.SetParent(punto);
        pelota.localPosition = Vector3.zero;

        LanzarPelota();
    }

    // LANZAR LA PELOTA HACIA DRAKE

    void LanzarPelota()
    {
        if (!tienePelota || lanzando)
            return;

        if (pelota == null || rbPelota == null || drake == null)
            return;

        lanzando = true;

        Vector2 direccion = (
            (Vector2)drake.position -
            (Vector2)pelota.position
        ).normalized;

        if (direccion.sqrMagnitude < 0.001f)
            direccion = Vector2.left;

        movimiento = Vector2.zero;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        tienePelota = false;

        pelota.SetParent(null);

        rbPelota.simulated = true;

        if (colliderPelota != null)
            colliderPelota.enabled = true;

        rbPelota.WakeUp();
        rbPelota.linearVelocity =
            direccion * velocidadLanzamiento;

        if (pelotaScript != null)
        {
            pelotaScript.RegistrarLanzamientoLuke();
            pelotaScript.ActivarModoQuemado(direccion);
        }

        Debug.Log("LUKE LANZÓ LA PELOTA HACIA DRAKE");

        if (colliderPelota != null && colliderLuke != null)
        {
            Physics2D.IgnoreCollision(
                colliderPelota,
                colliderLuke,
                true
            );

            StartCoroutine(ReactivarColisionPelota());
        }

        StartCoroutine(FinalizarLanzamiento());
    }

    IEnumerator ReactivarColisionPelota()
    {
        yield return new WaitForSeconds(0.2f);

        if (colliderPelota != null && colliderLuke != null)
        {
            Physics2D.IgnoreCollision(
                colliderPelota,
                colliderLuke,
                false
            );
        }
    }

    IEnumerator FinalizarLanzamiento()
    {
        yield return new WaitForSeconds(0.2f);
        lanzando = false;
        ActualizarAnimacion();
    }

    // ANIMACIONES

    void ActualizarAnimacion()
    {
        if (animator == null)
            return;

        if (tienePelota || lanzando)
        {
            CambiarAnimacion(animacionAgachado);
        }
        else if (movimiento.sqrMagnitude > 0.01f)
        {
            CambiarAnimacion(animacionCorrer);
        }
        else
        {
            CambiarAnimacion(animacionQuieto);
        }
    }

    void CambiarAnimacion(string nombre)
    {
        if (animator == null || animacionActual == nombre)
            return;

        animator.Play(nombre);
        animacionActual = nombre;
    }
}