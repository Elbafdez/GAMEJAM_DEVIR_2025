using UnityEngine;

public class RandomMove : MonoBehaviour
{
    private float velocidad = 0.5f;
    private float cambioDireccionCada = 2f;

    private Vector2 direccion;
    private float temporizador;
    private float limiteMapa = 20f; // Límite de la cuadrícula
    private Animator animator;

    void Start()
    {
        float timeToDestroy = Random.Range(10f, 20f);
        ElegirNuevaDireccion();
        Invoke(nameof(DestruirPersona), timeToDestroy);

        animator = GetComponent<Animator>();
    }

    void DestruirPersona()
    {
        GameManager.Instance.RemoverPersona((Vector2)transform.position);
        Destroy(gameObject);
    }

    void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            ElegirNuevaDireccion();
        }

        Vector2 nuevaPos = (Vector2)transform.position + direccion * velocidad * Time.deltaTime;

        if (Mathf.Abs(nuevaPos.x) > limiteMapa || Mathf.Abs(nuevaPos.y) > limiteMapa)
        {
            // Si se va fuera de los límites, elige nueva dirección
            ElegirNuevaDireccion();
        }
        else
        {
            transform.Translate(direccion * velocidad * Time.deltaTime);
        }
        
        // Actualizar la animación
        if (animator != null)
        {
            animator.SetBool("Walk", direccion != Vector2.zero);
        }
    }

    void ElegirNuevaDireccion()
    {
        float chanceDeQuedarseQuieto = 0.2f;

        if (Random.value < chanceDeQuedarseQuieto)
        {
            direccion = Vector2.zero;
        }
        else
        {
            Vector2[] direcciones = new Vector2[] {
                Vector2.up, Vector2.down, Vector2.left, Vector2.right,
                new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized,
                new Vector2(1, -1).normalized, new Vector2(-1, -1).normalized
            };

            direccion = direcciones[Random.Range(0, direcciones.Length)];
        }

        temporizador = Random.Range(0.5f, cambioDireccionCada);
    }
}