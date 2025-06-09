using UnityEngine;

public class PartyMove : MonoBehaviour
{
    public float velocidad = 3f;
    public float escalaPorPersona;
    public float escalaPorBorracho;
    public float escalaMinima;
    public float escalaMaxima;

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // Leer input (WASD o flechas)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Crear vector de movimiento
        Vector2 movimiento = new Vector2(horizontal, vertical).normalized;

        // Aplicar movimiento al GameObject
        transform.Translate(movimiento * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Persona"))
        {
            // Aumentar escala
            CambiarEscala(escalaPorPersona);
            GameManager.Instance.RemoverPersona((Vector2)other.transform.position);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Borracho"))
        {
            // Disminuir escala
            CambiarEscala(-escalaPorBorracho);
            GameManager.Instance.RemoverPersona((Vector2)other.transform.position);
            Destroy(other.gameObject);
        }
    }

    void CambiarEscala(float delta)
    {
        // Cambiar la escala del GameObject
        Vector3 nuevaEscala = transform.localScale + new Vector3(delta, delta, 0f);
        // Asegurarse de que la escala se mantenga dentro de los límites
        nuevaEscala.x = Mathf.Clamp(nuevaEscala.x, escalaMinima, escalaMaxima);
        nuevaEscala.y = Mathf.Clamp(nuevaEscala.y, escalaMinima, escalaMaxima);
        transform.localScale = nuevaEscala;
    }
}