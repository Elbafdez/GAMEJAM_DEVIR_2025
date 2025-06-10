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

    [Header("Jugador y mapa")]
    public Transform jugador;
    public float limiteMapa = 30f;
    public float distanciaMinima = 5f;
    public float distanciaEntrePersonas = 1f;
    public GameObject gameOverText;
    public TextMeshProUGUI textoPersonas;
    public TextMeshProUGUI textoTiempo;

    [Header("Spawning General")]
    public float tiempoMin = 1f;
    public float tiempoMax = 3f;

    private List<Vector2> posicionesOcupadas = new List<Vector2>();

    void Awake() => Instance = this;

    void Start()
    {
        StartCoroutine(SpawnearGrupos());
        gameOverText.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
        textoPersonas.text = "Participantes: " + PartyMove.Instance.personas;    // Mostrar el número de personas
        float tiempoTranscurrido = Time.timeSinceLevelLoad / 60f; // Convertir a minutos
        textoTiempo.text = "Tiempo de faena: " + string.Format("{0:00}:{1:00}", (int)tiempoTranscurrido, (int)((tiempoTranscurrido - (int)tiempoTranscurrido) * 60));
    }
}