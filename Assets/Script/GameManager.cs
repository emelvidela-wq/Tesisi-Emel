
using UnityEngine;

public class gameManager : MonoBehaviour
{
    [Header("Marcadores de vidas")]
    public MarcadorVidasDrake marcadorDrake;
    public MarcadorVidasLuke marcadorLuke;

    [Header("Introducción del entrenador")]
    public GameObject entrenadorSquare;
    public SpriteRenderer spriteEntrenador;
    public Sprite[] spritesIntroduccion = new Sprite[8];

    private int indiceDialogo = 0;
    private bool introduccionActiva = true;
    private float escalaTiempoOriginal;

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

        // Pausar el juego durante toda la introducción.
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!introduccionActiva)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AvanzarDialogo();
        }
    }

    void MostrarDialogo()
    {
        if (spriteEntrenador == null)
        {
            Debug.LogError(
                "Asigná el Sprite Renderer del Square del entrenador."
            );
            return;
        }

        if (spritesIntroduccion == null ||
            spritesIntroduccion.Length != 8)
        {
            Debug.LogError(
                "Asigná los 8 sprites de la introducción en GameManager3."
            );
            return;
        }

        spriteEntrenador.sprite =
            spritesIntroduccion[indiceDialogo];
    }

    void AvanzarDialogo()
    {
        indiceDialogo++;

        if (indiceDialogo < spritesIntroduccion.Length)
        {
            MostrarDialogo();
        }
        else
        {
            FinalizarIntroduccion();
        }
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

    void OnDestroy()
    {
        // Evitar que la escena quede pausada al destruir el GameManager.
        if (introduccionActiva)
            Time.timeScale = escalaTiempoOriginal;
    }

    // Conservá acá las funciones de vidas y daño
    // que ya tenga tu GameManager3 actual.
    
public void ToqueJugador()
{
    Debug.Log("El jugador tocó la pelota.");
}

public void ToqueBot()
{
    Debug.Log("El bot tocó la pelota.");
}

public void PuntoBot()
{
    Debug.Log("Punto para el bot.");
}

public void PuntoJugador()
{
    Debug.Log("Punto para el jugador.");
}
}