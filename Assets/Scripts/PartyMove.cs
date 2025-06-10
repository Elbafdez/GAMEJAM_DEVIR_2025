using UnityEngine;
using System.Collections;
using TMPro;

public class PartyMove : MonoBehaviour
{
    public static PartyMove Instance;
    public float velocidad = 3f;
    public float escalaPorPersona;
    public float escalaPorBorracho;
    public float escalaMinima;
    public float escalaMaxima;
    private float limiteMapa = 30f;
    public int personas = 0;

    public Camera mainCamera;
    public Color[] coloresFondo;
    public float intervaloCambioColor;
    public float tamañoParaColorCamara = 2f;

    private bool cambioColorActivo = false;
    public TextMeshProUGUI contadorPersonas;


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        Move();
        
        contadorPersonas.text = personas.ToString();    // Mostrar el número de personas

        // Si llegamos a la escala minima game over
        if (transform.localScale.x <= escalaMinima) //Es la misma para x e y
        {
            GameManager.Instance.GameOver();
        }

        // Iniciar cambio de fondo si escala >= 2 y aún no está activo
        if (!cambioColorActivo && transform.localScale.x >= tamañoParaColorCamara)
        {
            cambioColorActivo = true;
            StartCoroutine(CambiarColorFondo());
        }
        
        // Detener cambio de fondo si escala < 2 y está activo
        else if (cambioColorActivo && transform.localScale.x < tamañoParaColorCamara)
        {
            cambioColorActivo = false;
            StopCoroutine(CambiarColorFondo());
            mainCamera.backgroundColor = new Color(0.737f, 0.737f, 0.737f); // #BCBCBC
        }

    }

    private void Move()
    {
        // Leer input (WASD o flechas)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Crear vector de movimiento
        Vector3 movimiento = new Vector3(horizontal, vertical, 0f).normalized * velocidad * Time.deltaTime;

        // Aplicar movimiento
        transform.position += movimiento;

        // Limitar dentro del mapa
        float clampedX = Mathf.Clamp(transform.position.x, -limiteMapa, limiteMapa);
        float clampedY = Mathf.Clamp(transform.position.y, -limiteMapa, limiteMapa);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    IEnumerator CambiarColorFondo()
    {
        int index = 0;
        while (cambioColorActivo)
        {
            if (coloresFondo.Length == 0) yield break;
    
            mainCamera.backgroundColor = coloresFondo[index];
            index = (index + 1) % coloresFondo.Length;
            yield return new WaitForSeconds(intervaloCambioColor);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Persona"))
        {
            // Aumentar escala
            CambiarEscala(escalaPorPersona);
            GameManager.Instance.RemoverPersona((Vector2)other.transform.position);
            Destroy(other.gameObject);
            personas++;
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