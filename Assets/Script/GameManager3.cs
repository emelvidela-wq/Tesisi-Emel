
using UnityEngine;

public class GameManager3 : MonoBehaviour
{
    [Header("Estado del nivel")]
    public bool nivelIniciado = false;

    [Header("Marcadores de vidas")]
    public MarcadorVidasDrake marcadorDrake;
    public MarcadorVidasLuke marcadorLuke;

    void Awake()
    {
        nivelIniciado = false;
        Debug.Log("GameManager3 iniciado.");
    }

    public void IniciarNivel()
    {
        nivelIniciado = true;
        Debug.Log("El nivel 3 está iniciado.");
    }

    public void DanarDrake()
    {
        if (marcadorDrake == null)
        {
            Debug.LogError(
                "Falta asignar el marcador de Drake en el nivel 3."
            );
            return;
        }

        marcadorDrake.PerderVida();
        Debug.Log("Drake perdió una vida en el nivel 3.");
    }

    public void DanarLuke()
    {
        if (marcadorLuke == null)
        {
            Debug.LogError(
                "Falta asignar el marcador de Luke en el nivel 3."
            );
            return;
        }

        marcadorLuke.PerderVida();
        Debug.Log("Luke perdió una vida en el nivel 3.");
    }
}