using UnityEngine;

public class RandomMove : MonoBehaviour
{
    private float velocidad = 0.5f;
    private float cambioDireccionCada = 2f; // cada cuántos segundos cambia dirección

    private Vector2 direccion;
    private float temporizador;

    void Start()
    {
        ElegirNuevaDireccion();
    }

    void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            ElegirNuevaDireccion();
        }

        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    void ElegirNuevaDireccion()
    {
        float chanceDeQuedarseQuieto = 0.2f; // 20% de probabilidad
    
        if (Random.value < chanceDeQuedarseQuieto)
        {
            direccion = Vector2.zero; // Se queda quieto
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
