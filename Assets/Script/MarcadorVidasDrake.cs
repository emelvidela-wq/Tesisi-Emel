
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarcadorVidasDrake : MonoBehaviour
{
    [Header("Sprites de las vidas de Drake")]
    public Sprite tresVidas;
    public Sprite dosVidas;
    public Sprite unaVida;
    public Sprite ceroVidas;

    [Header("Vidas actuales")]
    public int vidas = 3;

    [Header("Escena al perder")]
    public string escenaPerder = "PerdisteNv3";

    private SpriteRenderer spriteRenderer;
    private bool yaPerdio = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        vidas = 3;
        ActualizarSprite();
    }

    public void PerderVida()
    {
        if (vidas <= 0 || yaPerdio)
            return;

        vidas--;

        ActualizarSprite();

        Debug.Log("Drake tiene " + vidas + " vidas.");

        if (vidas == 0)
        {
            yaPerdio = true;
            Debug.Log("Drake perdió. Cargando " + escenaPerder);
            SceneManager.LoadScene(escenaPerder);
        }
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
