using UnityEngine;

public class Intro : MonoBehaviour
{
    [Header("Objeto Intro")]
    public GameObject introSquare;
    public SpriteRenderer spriteIntro;

    [Header("Sprites de la introducción")]
    public Sprite[] spritesIntroduccion = new Sprite[8];

    [Header("Administrador del nivel 3")]
    public GameManager3 gameManager3;

    private int indiceDialogo = 0;
    private bool introduccionActiva = true;
    private float escalaTiempoOriginal;

    void Awake()
    {
        escalaTiempoOriginal = Time.timeScale;

        if (introSquare == null)
            introSquare = gameObject;

        if (spriteIntro == null)
            spriteIntro = introSquare.GetComponent<SpriteRenderer>();

        if (gameManager3 == null)
            gameManager3 = FindFirstObjectByType<GameManager3>();

        introSquare.SetActive(true);

        Time.timeScale = 0f;

        if (spritesIntroduccion == null ||
            spritesIntroduccion.Length != 8)
        {
            Debug.LogError(
                "Intro necesita exactamente 8 sprites."
            );
            return;
        }

        indiceDialogo = 0;
        MostrarDialogo();
    }

    void Update()
    {
        if (!introduccionActiva)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            AvanzarDialogo();
    }

    void MostrarDialogo()
    {
        if (spriteIntro == null)
        {
            Debug.LogError(
                "Intro necesita un SpriteRenderer."
            );
            return;
        }

        if (spritesIntroduccion[indiceDialogo] == null)
        {
            Debug.LogError(
                "Falta el sprite número " +
                (indiceDialogo + 1) + " de Intro."
            );
            return;
        }

        spriteIntro.sprite = spritesIntroduccion[indiceDialogo];
    }

    void AvanzarDialogo()
    {
        if (indiceDialogo < spritesIntroduccion.Length - 1)
        {
            indiceDialogo++;
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

        if (introSquare != null)
            introSquare.SetActive(false);

        if (gameManager3 != null)
            gameManager3.IniciarNivel();

        Time.timeScale = escalaTiempoOriginal;

        Debug.Log("Intro terminada. Comienza el nivel 3.");
    }

    public bool IntroduccionActiva()
    {
        return introduccionActiva;
    }

    void OnDestroy()
    {
        if (introduccionActiva)
            Time.timeScale = escalaTiempoOriginal;
    }
}