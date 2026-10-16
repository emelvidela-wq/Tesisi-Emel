
using UnityEngine;

public class DialogoProfesorNivel1 : MonoBehaviour
{
    [Header("Objeto que muestra el diálogo")]
    public GameObject panelDialogo;

    [Header("Componente que muestra las imágenes")]
    public SpriteRenderer imagenDialogo;

    [Header("Las 8 imágenes del profesor")]
    public Sprite[] imagenesDialogo = new Sprite[8];

    [Header("Personaje que empieza la carrera")]
    public NPCMovement luke;

    private int indiceImagen = 0;
    private bool dialogoActivo = true;

    private void Start()
    {
        // Evitar que Luke empiece antes de terminar el diálogo.
        if (luke != null)
        {
            luke.canStart = false;
        }
        else
        {
            Debug.LogError(
                "Falta asignar a Luke en el Inspector."
            );
        }

        // Pausar el juego al comenzar el nivel.
        Time.timeScale = 0f;

        // Comenzar desde la primera imagen.
        indiceImagen = 0;
        dialogoActivo = true;

        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
        }

        MostrarImagen();
    }

    private void Update()
    {
        if (!dialogoActivo)
        {
            return;
        }

        // Avanzar solamente con la flecha hacia abajo.
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AvanzarDialogo();
        }
    }

    private void MostrarImagen()
    {
        if (imagenDialogo == null)
        {
            Debug.LogError(
                "Falta asignar Imagen Dialogo en el Inspector."
            );
            return;
        }

        if (imagenesDialogo == null ||
            imagenesDialogo.Length != 8)
        {
            Debug.LogError(
                "Debes configurar las 8 imágenes del diálogo."
            );
            return;
        }

        if (imagenesDialogo[indiceImagen] == null)
        {
            Debug.LogError(
                "Falta la imagen número " + (indiceImagen + 1)
            );
            return;
        }

        imagenDialogo.sprite = imagenesDialogo[indiceImagen];
    }

    private void AvanzarDialogo()
    {
        indiceImagen++;

        // Mostrar la siguiente imagen si todavía quedan.
        if (indiceImagen < imagenesDialogo.Length)
        {
            MostrarImagen();
        }
        else
        {
            FinalizarDialogo();
        }
    }

    private void FinalizarDialogo()
    {
        if (!dialogoActivo)
        {
            return;
        }

        dialogoActivo = false;

        // Ocultar el diálogo del profesor.
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }

        // Reanudar el tiempo del juego.
        Time.timeScale = 1f;

        // Dar la orden a Luke para que comience a correr.
        if (luke != null)
        {
            luke.StartRace();
        }
    }

    private void OnDisable()
    {
        // Evitar dejar el juego pausado accidentalmente.
        if (dialogoActivo)
        {
            Time.timeScale = 1f;
        }
    }
}

