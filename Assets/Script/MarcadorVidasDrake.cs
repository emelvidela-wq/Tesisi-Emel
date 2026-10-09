
using UnityEngine;

public class MarcadorVidasDrake : MonoBehaviour
{
    [Header("Sprites de las vidas de Drake")]
    public Sprite tresVidas;
    public Sprite dosVidas;
    public Sprite unaVida;
    public Sprite ceroVidas;

    [Header("Vidas actuales")]
    public int vidas = 3;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        vidas = 3;
        ActualizarSprite();
    }

    void Update()
    {
        // PRUEBA TEMPORAL: presionar O para quitar una vida
        if (Input.GetKeyDown(KeyCode.O))
        {
            PerderVida();
        }
    }

    public void PerderVida()
    {
        if (vidas <= 0)
            return;

        vidas--;

        ActualizarSprite();

        Debug.Log("Drake tiene " + vidas + " vidas.");
    }

    void ActualizarSprite()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning(
                "No se encontró el Sprite Renderer del marcador de Drake."
            );
            return;
        }

        switch (vidas)
        {
            case 3:
                spriteRenderer.sprite = tresVidas;
                break;

            case 2:
                spriteRenderer.sprite = dosVidas;
                break;

            case 1:
                spriteRenderer.sprite = unaVida;
                break;

            case 0:
                spriteRenderer.sprite = ceroVidas;
                break;
        }
    }
}
