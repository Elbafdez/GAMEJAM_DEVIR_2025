using UnityEngine;

public class PartyMove : MonoBehaviour
{
    public float velocidad = 3f;

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
}