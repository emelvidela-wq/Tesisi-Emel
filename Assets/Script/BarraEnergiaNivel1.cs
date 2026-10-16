
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarraEnergiaNivel1 : MonoBehaviour
{
    [Header("Imagen de la barra")]
    public Image imagenBarra;

    [Header("Sprites normales de los rayos")]
    public Sprite[] spritesNormales;

    [Header("Sprites de la animacion del dash")]
    public Sprite[] spritesEnergia;

    [Header("Velocidad de las animaciones")]
    public float velocidadRayos = 0.12f;
    public float velocidadDash = 0.06f;

    private int indiceNormal = 0;
    private bool dashActivo = false;
    private Coroutine animacionDash;

    void Start()
    {
        MostrarSpriteNormal();
    }

    void Update()
    {
        if (dashActivo || spritesNormales == null ||
            spritesNormales.Length == 0)
            return;

        // Animacion normal en bucle.
        indiceNormal = (indiceNormal + 1) %
                       spritesNormales.Length;

        MostrarSpriteNormal();

        // Esperar entre cambios de sprite.
        // La animacion se controla con una corrutina.
    }

    public void IntentarUsarDash()
    {
        if (dashActivo)
            return;

        if (animacionDash != null)
            StopCoroutine(animacionDash);

        animacionDash = StartCoroutine(AnimarDash());
    }

    private IEnumerator AnimarDash()
    {
        dashActivo = true;

        if (spritesEnergia != null)
        {
            for (int i = 0; i < spritesEnergia.Length; i++)
            {
                if (spritesEnergia[i] != null &&
                    imagenBarra != null)
                {
                    imagenBarra.sprite = spritesEnergia[i];
                }

                yield return new WaitForSeconds(velocidadDash);
            }
        }

        dashActivo = false;
        indiceNormal = 0;
        MostrarSpriteNormal();
        animacionDash = null;
    }

    public void TerminarDash()
    {
        // La secuencia de la barra termina por su cuenta.
        // No hace falta interrumpirla cuando termina el movimiento.
    }

    private void MostrarSpriteNormal()
    {
        if (imagenBarra == null ||
            spritesNormales == null ||
            spritesNormales.Length == 0)
            return;

        if (spritesNormales[indiceNormal] != null)
            imagenBarra.sprite = spritesNormales[indiceNormal];
    }
}