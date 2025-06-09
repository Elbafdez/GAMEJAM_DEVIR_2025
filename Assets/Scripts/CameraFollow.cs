using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objetivo;     // El objeto a seguir (tu personaje / fiesta)
    public float suavizado = 0.1f; // Suavizado del movimiento (opcional)

    private Vector3 offset;
    private Vector3 velocidad = Vector3.zero;

    void Start()
    {
        if (objetivo != null)
        {
            offset = transform.position - objetivo.position;
        }
    }

    void LateUpdate()
    {
        if (objetivo != null)
        {
            Vector3 posicionDeseada = objetivo.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
        }
    }
}