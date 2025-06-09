using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Para acceso global
    public GameObject[] personasPrefabs;
    public Transform jugador;
    public float limiteMapa = 20f;
    public float distanciaMinima = 3f;     // Distancia mínima con jugador
    public float distanciaEntrePersonas = 1f; // Separación entre NPCs
    public float tiempoMin = 1f;
    public float tiempoMax = 3f;
    public int spawnMin = 4;
    public int spawnMax = 8;

    private List<Vector2> posicionesOcupadas = new List<Vector2>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(SpawnearGrupos());
    }

    IEnumerator SpawnearGrupos()
    {
        while (true)
        {
            float espera = Random.Range(tiempoMin, tiempoMax);
            yield return new WaitForSeconds(espera);

            int cantidad = Random.Range(spawnMin, spawnMax + 1);
            int intentosMax = 100;

            for (int i = 0; i < cantidad; i++)
            {
                int intentos = 0;
                Vector2 posicion;

                do
                {
                    float x = Random.Range(-limiteMapa, limiteMapa);
                    float y = Random.Range(-limiteMapa, limiteMapa);
                    posicion = new Vector2(x, y);
                    intentos++;
                }
                while (
                    (Vector2.Distance(posicion, jugador.position) < distanciaMinima ||
                     EstaDemasiadoCercaDeOtraPersona(posicion)) &&
                    intentos < intentosMax
                );

                if (intentos < intentosMax)
                {
                    GameObject prefab = personasPrefabs[Random.Range(0, personasPrefabs.Length)];
                    GameObject persona = Instantiate(prefab, posicion, Quaternion.identity);
                    posicionesOcupadas.Add(posicion);
                }
            }
        }
    }

    bool EstaDemasiadoCercaDeOtraPersona(Vector2 nuevaPos)
    {
        foreach (Vector2 pos in posicionesOcupadas)
        {
            if (Vector2.Distance(nuevaPos, pos) < distanciaEntrePersonas)
                return true;
        }
        return false;
    }

    public void RemoverPersona(Vector2 posicion)
    {
        posicionesOcupadas.Remove(posicion);
    }
}