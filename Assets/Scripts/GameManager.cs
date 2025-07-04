using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class TipoNPC
{
    public string nombre;
    public GameObject[] prefabs;
    public int minPorTanda;
    public int maxPorTanda;
    [Range(0f, 1f)] public float probabilidad;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("NPC Types")]
    public List<TipoNPC> tiposNPC;

    [Header("Ajuste dinámico de probabilidad")]
    private float tiempoParaAumentarProbabilidad = 10f; // cada 10 segundos
    private float incrementoProbabilidad = 0.08f; // subir un 5% cada vez
    private float probabilidadMaxima = 0.9f; // máximo 80%

    [Header("Jugador y mapa")]
    public Transform jugador;
    public float limiteMapa = 30f;
    public float distanciaMinima = 5f;
    public float distanciaEntrePersonas = 1f;
    public GameObject gameOverText;
    public TextMeshProUGUI textoPersonas;
    public TextMeshProUGUI textoTiempo;
    public AudioSource audioSource;
    public AudioClip musicaFiestaClip;

    [Header("Spawning General")]
    public float tiempoMin = 1f;
    public float tiempoMax = 3f;

    private List<Vector2> posicionesOcupadas = new List<Vector2>();

    void Awake() => Instance = this;

    void Start()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo no esté pausado
        StartCoroutine(SpawnearGrupos());
        StartCoroutine(AumentarProbabilidadBorrachos());
        gameOverText.SetActive(false);

        if (audioSource != null && musicaFiestaClip != null)   // Asegurarse de que la música esté configurada
        {
            audioSource.clip = musicaFiestaClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reiniciar el juego
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
    

    IEnumerator SpawnearGrupos()
    {
        while (true)
        {
            float espera = Random.Range(tiempoMin, tiempoMax);

            foreach (var tipo in tiposNPC)
            {
                if (tipo.prefabs.Length == 0) continue;
                if (Random.value > tipo.probabilidad) continue;

                int cantidad = Random.Range(tipo.minPorTanda, tipo.maxPorTanda + 1);
                int intentosMax = 100;

                for (int i = 0; i < cantidad; i++)
                {
                    int intentos = 0;
                    Vector2 posicion = Vector2.zero;

                    do
                    {
                        Vector2 offset = Random.insideUnitCircle * 15f;
                        Vector2 posiblePos = (Vector2)jugador.position + offset;

                        posiblePos.x = Mathf.Clamp(posiblePos.x, -limiteMapa, limiteMapa);
                        posiblePos.y = Mathf.Clamp(posiblePos.y, -limiteMapa, limiteMapa);

                        posicion = posiblePos;
                        intentos++;
                    }
                    while (
                        (Vector2.Distance(posicion, jugador.position) < distanciaMinima ||
                        EstaDemasiadoCercaDeOtraPersona(posicion)) &&
                        intentos < intentosMax
                    );

                    if (intentos < intentosMax)
                    {
                        GameObject prefab = tipo.prefabs[Random.Range(0, tipo.prefabs.Length)];
                        Instantiate(prefab, posicion, Quaternion.identity);
                        posicionesOcupadas.Add(posicion);
                    }
                }
            }
            yield return new WaitForSeconds(espera);
        }
    }

    IEnumerator AumentarProbabilidadBorrachos()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoParaAumentarProbabilidad);

            foreach (var tipo in tiposNPC)
            {
                if (tipo.nombre.ToLower().Contains("borracho")) // o usa == "Borracho"
                {
                    tipo.probabilidad = Mathf.Min(tipo.probabilidad + incrementoProbabilidad, probabilidadMaxima);
                    Debug.Log("Probabilidad de borrachos aumentada a: " + tipo.probabilidad);
                }
            }
        }
    }

    bool EstaDemasiadoCercaDeOtraPersona(Vector2 nuevaPos)
    {
        foreach (Vector2 pos in posicionesOcupadas)
            if (Vector2.Distance(nuevaPos, pos) < distanciaEntrePersonas)
                return true;

        return false;
    }

    public void RemoverPersona(Vector2 posicion)
    {
        posicionesOcupadas.Remove(posicion);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        gameOverText.SetActive(true);
        Time.timeScale = 0f;
        textoPersonas.text = "Fiesteros: " + PartyMove.Instance.personas;    // Mostrar el número de personas
        float tiempoTranscurrido = Time.timeSinceLevelLoad / 60f; // Convertir a minutos
        textoTiempo.text = "Tiempo de desmadre: " + string.Format("{0:00}:{1:00}", (int)tiempoTranscurrido, (int)((tiempoTranscurrido - (int)tiempoTranscurrido) * 60));
    }
}