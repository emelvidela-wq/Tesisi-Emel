using UnityEngine;

public class MovimientoJugador3 : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Agacharse")]
    public float velocidadAgachado = 2.5f;

    [Header("Collider Agachado")]
    public Vector2 tamañoNormal = new Vector2(1.40f, 2.65f);
    public Vector2 offsetNormal = new Vector2(-0.0029f, -0.075f);

    public Vector2 tamañoAgachado = new Vector2(1.40f, 1.40f);
    public Vector2 offsetAgachado = new Vector2(-0.0029f, -0.70f);

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Vector2 movimiento;

    private bool agachado;

    // Animator
    private Animator animator;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        boxCollider =
            GetComponent<BoxCollider2D>();

        animator =
            GetComponent<Animator>();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        float horizontal =
            Input.GetAxisRaw("Horizontal");

        float vertical =
            Input.GetAxisRaw("Vertical");


        movimiento =
            new Vector2(
                horizontal,
                vertical
            ).normalized;


        // Mantener C para agacharse
        agachado =
            Input.GetKey(KeyCode.C);


        ActualizarCollider();

        ActualizarAnimacion();
    }


    // =========================================================
    // FIXED UPDATE
    // =========================================================

    void FixedUpdate()
    {
        float velocidadActual =
            agachado
            ? velocidadAgachado
            : velocidad;


        rb.linearVelocity =
            movimiento *
            velocidadActual;
    }


    // =========================================================
    // COLLIDER
    // =========================================================

    void ActualizarCollider()
    {
        if (boxCollider == null)
        {
            return;
        }


        if (agachado)
        {
            boxCollider.size =
                tamañoAgachado;

            boxCollider.offset =
                offsetAgachado;
        }
        else
        {
            boxCollider.size =
                tamañoNormal;

            boxCollider.offset =
                offsetNormal;
        }
    }


    // =========================================================
    // ANIMACIÓN
    // =========================================================

    void ActualizarAnimacion()
    {
        if (animator == null)
        {
            return;
        }


        if (agachado)
        {
            animator.Play("Crouch3");
        }
        else if (movimiento != Vector2.zero)
        {
            animator.Play("Run3");
        }
        else
        {
            animator.Play("State3");
        }
    }
}