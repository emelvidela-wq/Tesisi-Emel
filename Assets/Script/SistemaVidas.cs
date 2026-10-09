using UnityEngine;

public class SistemaVidas : MonoBehaviour
{
    [Header("Vidas")]
    public int vidasDrake = 3;
    public int vidasLuke = 3;

    [Header("Animadores de corazones")]
    public Animator animatorVidasDrake;
    public Animator animatorVidasLuke;


    // =========================================================
    // QUITAR VIDA A DRAKE
    // =========================================================

    public void DanarDrake()
    {
        if (vidasDrake <= 0)
            return;

        vidasDrake--;

        Debug.Log("🔥 DRAKE PERDIÓ UNA VIDA. Vidas restantes: " + vidasDrake);

        ActualizarVidasDrake();
    }


    // =========================================================
    // QUITAR VIDA A LUKE
    // =========================================================

    public void DanarLuke()
{
    Debug.Log("🔥🔥🔥 DANAR LUKE FUE LLAMADO");

    if (vidasLuke <= 0)
        return;

    vidasLuke--;

    Debug.Log("❤️ Luke ahora tiene: " + vidasLuke);

    ActualizarVidasLuke();
}


    // =========================================================
    // ACTUALIZAR CORAZONES DE DRAKE
    // =========================================================

    void ActualizarVidasDrake()
    {
        if (animatorVidasDrake == null)
            return;

        switch (vidasDrake)
        {
            case 3:
                animatorVidasDrake.Play("Vida3");
                break;

            case 2:
                animatorVidasDrake.Play("Vida2");
                break;

            case 1:
                animatorVidasDrake.Play("Vida1");
                break;

            case 0:
                animatorVidasDrake.Play("Vida0");
                break;
        }
    }


    // =========================================================
    // ACTUALIZAR CORAZONES DE LUKE
    // =========================================================

    void ActualizarVidasLuke()
{
    if (animatorVidasLuke == null)
        return;

    switch (vidasLuke)
    {
        case 3:
            animatorVidasLuke.Play("Vida3luke");
            break;

        case 2:
            animatorVidasLuke.Play("Vida2luke");
            break;

        case 1:
            animatorVidasLuke.Play("Vida1luke");
            break;

        case 0:
            animatorVidasLuke.Play("Vida0luke");
            break;
    }
}


    // =========================================================
    // INICIAR
    // =========================================================

    void Start()
    {
        ActualizarVidasDrake();
        ActualizarVidasLuke();
    }
}