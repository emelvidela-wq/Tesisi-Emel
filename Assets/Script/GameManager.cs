
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    [Header("MARCADOR DE DRAKE")]
    public SpriteRenderer marcadorJugador;
    public Sprite[] numerosJugador = new Sprite[5];

    [Header("MARCADOR DE ADA")]
    public SpriteRenderer marcadorBot;
    public Sprite[] numerosBot = new Sprite[5];

    [Header("SAQUES")]
    public Transform saqueJugador;
    public Transform saqueBot;
    public Pelota pelota;

    [Header("POSICIONES INICIALES")]
    public Transform inicioJugador;
    public Transform inicioBot;

    [Header("JUGADORES")]
    public Transform jugador;
    public Transform bot;

    [Header("ESCENAS FINALES")]
    public string escenaVictoria = "GanasteNv2";
    public string escenaDerrota = "PerdisteNv2";

    [Header("INTRODUCCIÓN DEL ENTRENADOR")]
    public GameObject entrenadorSquare;
    public SpriteRenderer spriteEntrenador;
    public Sprite[] spritesIntroduccion = new Sprite[8];

    private int indiceDialogo = 0;
    private bool introduccionActiva = true;
    private float escalaTiempoOriginal;

    private int puntosJugador = 0;
    private int puntosBot = 0;

    private bool puntoEnCurso = false;

    public bool PartidoTerminado { get; private set; } = false;

    void Awake()
    {
        escalaTiempoOriginal = Time.timeScale;

        if (entrenadorSquare != null)
        {
            if (spriteEntrenador == null)
                spriteEntrenador =
                    entrenadorSquare.GetComponent<SpriteRenderer>();

            entrenadorSquare.SetActive(true);
        }

        indiceDialogo = 0;
        MostrarDialogo();

        Time.timeScale = 0f;
    }

    void Start()
    {
        if (pelota == null)
            pelota = FindFirstObjectByType<Pelota>();

        ActualizarMarcador();
    }

    void Update()
    {
        if (!introduccionActiva)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            AvanzarDialogo();
    }

    void MostrarDialogo()
    {
        if (spriteEntrenador == null)
        {
            Debug.LogError(
                "Asigná el Sprite Renderer del entrenador."
            );
            return;
        }

        if (spritesIntroduccion == null ||
            spritesIntroduccion.Length != 8)
        {
            Debug.LogError(
                "Asigná los 8 sprites de la introducción."
            );
            return;
        }

        if (spritesIntroduccion[indiceDialogo] == null)
        {
            Debug.LogError(
                "Falta asignar el sprite " + indiceDialogo +
                " de la introducción."
            );
            return;
        }

        spriteEntrenador.sprite =
            spritesIntroduccion[indiceDialogo];

        spriteEntrenador.enabled = true;
    }

    void AvanzarDialogo()
    {
        indiceDialogo++;

        if (indiceDialogo < spritesIntroduccion.Length)
            MostrarDialogo();
        else
            FinalizarIntroduccion();
    }

    void FinalizarIntroduccion()
    {
        introduccionActiva = false;

        if (entrenadorSquare != null)
            entrenadorSquare.SetActive(false);

        Time.timeScale = escalaTiempoOriginal;

        Debug.Log("Introducción terminada. Comienza el partido.");
    }

    public bool IntroduccionActiva()
    {
        return introduccionActiva;
    }

    public bool EsJugador(Transform objeto)
    {
        if (objeto == null || jugador == null)
            return false;

        return objeto == jugador ||
               objeto.IsChildOf(jugador);
    }

    public bool EsBot(Transform objeto)
    {
        if (objeto == null || bot == null)
            return false;

        return objeto == bot ||
               objeto.IsChildOf(bot);
    }

    public bool ToqueJugador()
    {
        if (PartidoTerminado || puntoEnCurso || pelota == null)
            return false;

        int toques = pelota.RegistrarToqueJugador();

        if (toques > 4)
        {
            Debug.Log(
                "Drake superó los 4 toques. Punto para Ada."
            );

            PuntoBot();
            return false;
        }

        Debug.Log("Toques de Drake: " + toques);
        return true;
    }

    public bool ToqueBot()
    {
        if (PartidoTerminado || puntoEnCurso || pelota == null)
            return false;

        int toques = pelota.RegistrarToqueBot();

        if (toques > 4)
        {
            Debug.Log(
                "Ada superó los 4 toques. Punto para Drake."
            );

            PuntoJugador();
            return false;
        }

        Debug.Log("Toques de Ada: " + toques);
        return true;
    }

    // Punto para Ada.
    public void PuntoBot()
    {
        if (PartidoTerminado || puntoEnCurso)
            return;

        puntoEnCurso = true;
        puntosBot = Mathf.Min(puntosBot + 1, 4);

        ActualizarMarcador();

        Debug.Log("Punto para Ada: " + puntosBot);

        if (puntosBot >= 4)
        {
            PartidoTerminado = true;
            Debug.Log("¡Ada ganó el partido!");

            CargarEscenaFinal(escenaDerrota);
            return;
        }

        PrepararSaque(saqueBot);
    }

    // Punto para Drake.
    public void PuntoJugador()
    {
        if (PartidoTerminado || puntoEnCurso)
            return;

        puntoEnCurso = true;
        puntosJugador = Mathf.Min(puntosJugador + 1, 4);

        ActualizarMarcador();

        Debug.Log("Punto para Drake: " + puntosJugador);

        if (puntosJugador >= 4)
        {
            PartidoTerminado = true;
            Debug.Log("¡Drake ganó el partido!");

            CargarEscenaFinal(escenaVictoria);
            return;
        }

        PrepararSaque(saqueJugador);
    }

    // Cargar la escena correspondiente al resultado.
    void CargarEscenaFinal(string nombreEscena)
    {
        Time.timeScale = 1f;

        Debug.Log("Cargando escena: " + nombreEscena);

        SceneManager.LoadScene(nombreEscena);
    }

    public void NuevaJugada()
    {
        if (!PartidoTerminado)
            puntoEnCurso = false;
    }

    void ActualizarMarcador()
    {
        // MARCADOR DE DRAKE.
        if (numerosJugador == null ||
            numerosJugador.Length < 5)
        {
            Debug.LogError(
                "Asigná los 5 sprites de Drake, del 0 al 4."
            );
        }
        else if (marcadorJugador == null)
        {
            Debug.LogError(
                "Falta el Sprite Renderer del marcador de Drake."
            );
        }
        else if (numerosJugador[puntosJugador] == null)
        {
            Debug.LogError(
                "Falta el sprite " + puntosJugador +
                " del marcador de Drake."
            );
        }
        else
        {
            marcadorJugador.gameObject.SetActive(true);
            marcadorJugador.enabled = true;
            marcadorJugador.sprite = numerosJugador[puntosJugador];
        }

        // MARCADOR DE ADA.
        if (numerosBot == null ||
            numerosBot.Length < 5)
        {
            Debug.LogError(
                "Asigná los 5 sprites de Ada, del 0 al 4."
            );
        }
        else if (marcadorBot == null)
        {
            Debug.LogError(
                "Falta el Sprite Renderer del marcador de Ada."
            );
        }
        else if (numerosBot[puntosBot] == null)
        {
            Debug.LogError(
                "Falta el sprite " + puntosBot +
                " del marcador de Ada."
            );
        }
        else
        {
            marcadorBot.gameObject.SetActive(true);
            marcadorBot.enabled = true;
            marcadorBot.sprite = numerosBot[puntosBot];
        }
    }

    void PrepararSaque(Transform posicionSaque)
    {
        ReiniciarJugador(jugador, inicioJugador);
        ReiniciarJugador(bot, inicioBot);

        if (pelota == null)
        {
            Debug.LogError("No se encontró la pelota.");
            return;
        }

        if (posicionSaque == null)
        {
            Debug.LogError("Falta asignar la posición de saque.");
            return;
        }

        pelota.ReiniciarParaSaque(posicionSaque);
    }

    void ReiniciarJugador(Transform personaje, Transform inicio)
    {
        if (personaje == null || inicio == null)
        {
            Debug.LogError(
                "Asigná el personaje y su posición inicial."
            );
            return;
        }

        Rigidbody2D fisica =
            personaje.GetComponent<Rigidbody2D>();

        if (fisica != null)
        {
            fisica.linearVelocity = Vector2.zero;
            fisica.angularVelocity = 0f;
            fisica.position = inicio.position;
        }
        else
        {
            personaje.position = inicio.position;
        }
    }

    void OnDestroy()
    {
        if (introduccionActiva)
            Time.timeScale = escalaTiempoOriginal;
    }
}