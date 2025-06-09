using UnityEngine;

public class BoringMove : MonoBehaviour
{
    public float velocidadNormal = 0.5f;
    public float velocidadPersecucion;
    public float distanciaPerseguir = 6f;
    public float cambioDireccionCada = 2f;

    private Vector2 direccion;
    private float temporizador;
    private Transform jugador;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        ElegirNuevaDireccion();
        Invoke(nameof(Destruir), 20f); // Destruir tras 10 segundos
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, jugador.position) <= distanciaPerseguir)
        {
            // Persecución
            Vector2 direccionAlJugador = (jugador.position - transform.position).normalized;
            transform.Translate(direccionAlJugador * velocidadPersecucion * Time.deltaTime);
        }
        else
        {
            // Movimiento aleatorio
            temporizador -= Time.deltaTime;
            if (temporizador <= 0f)
            {
                ElegirNuevaDireccion();
            }

            transform.Translate(direccion * velocidadNormal * Time.deltaTime);
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

    void Destruir()
    {
        GameManager.Instance.RemoverPersona((Vector2)transform.position);
        Destroy(gameObject);
    }
}