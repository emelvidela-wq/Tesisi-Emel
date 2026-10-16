
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimacionBarraEnergia : MonoBehaviour
{
    [Header("Imagen completa de la barra")]
    public Image imagenBarra;

    [Header("Animacion normal: primeros 3 sprites")]
    public Sprite[] spritesNormales;

    [Header("Recarga: del vacio al lleno")]
    public Sprite[] spritesRecarga;

    public float tiempoNormal = 0.12f;
    public float tiempoRecarga = 0.1f;

    private Coroutine rutinaNormal;
    private Coroutine rutinaDash;

    private void Start()
    {
        rutinaNormal = StartCoroutine(AnimarNormal());
    }

    public void ReproducirDash(float duracionDash, float cooldown)
    {
        if (rutinaDash != null)
            StopCoroutine(rutinaDash);

        if (rutinaNormal != null)
        {
            StopCoroutine(rutinaNormal);
            rutinaNormal = null;
        }

        rutinaDash = StartCoroutine(
            AnimarRecarga(duracionDash, cooldown)
        );
    }

    private IEnumerator AnimarRecarga(
        float duracionDash, float cooldown)
    {
        // Mostrar inmediatamente la barra vacia.
        if (imagenBarra != null &&
            spritesRecarga != null &&
            spritesRecarga.Length > 0)
        {
            imagenBarra.sprite = spritesRecarga[0];
        }

        // Esperar a que termine la duracion del dash.
        yield return new WaitForSeconds(duracionDash);

        // Recargar la barra desde vacia hasta llena.
        if (spritesRecarga != null &&
            spritesRecarga.Length > 1)
        {
            float tiempoPorSprite =
                cooldown / (spritesRecarga.Length - 1);

            for (int i = 1; i < spritesRecarga.Length; i++)
            {
                if (imagenBarra != null &&
                    spritesRecarga[i] != null)
                {
                    imagenBarra.sprite = spritesRecarga[i];
                }

                yield return new WaitForSeconds(
                    tiempoPorSprite
                );
            }
        }
        else
        {
            yield return new WaitForSeconds(cooldown);
        }

        // El dash ya esta habilitado: vuelve la animacion normal.
        rutinaDash = null;
        rutinaNormal = StartCoroutine(AnimarNormal());
    }

    private IEnumerator AnimarNormal()
    {
        int indice = 0;

        while (true)
        {
            if (imagenBarra != null &&
                spritesNormales != null &&
                spritesNormales.Length > 0)
            {
                imagenBarra.sprite = spritesNormales[indice];
                indice = (indice + 1) % spritesNormales.Length;
            }

            yield return new WaitForSeconds(tiempoNormal);
        }
    }
}
