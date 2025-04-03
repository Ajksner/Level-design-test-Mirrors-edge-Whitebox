using UnityEngine;
using System.Collections.Generic;

public class LadderMover : MonoBehaviour
{
    public List<Transform> platforms;  // Seznam všech pohyblivých plošin
    public float speed = 3f;           // Rychlost pohybu nahoru/dolů
    private bool playerInside = false; // Je hráč uvnitř triggeru?
    private List<Vector3> startPositions = new List<Vector3>(); // Původní pozice plošin

    private void Start()
    {
        // Uloží výchozí pozice všech platforem
        startPositions.Clear(); // Vyčistí seznam pro jistotu
        foreach (Transform platform in platforms)
        {
            startPositions.Add(platform.position); // Uloží pozici pro každou platformu
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Hráč vstoupil do triggeru!");

            // Resetuje pozici platformy na její původní pozici při vstupu do triggeru
            for (int i = 0; i < platforms.Count; i++)
            {
                platforms[i].position = startPositions[i]; // Vrátí platformu na její výchozí pozici
                Debug.Log("Platforma " + i + " resetována na výchozí pozici při vstupu hráče!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Hráč opustil trigger!");
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            float move = 0;

            if (Input.GetKey(KeyCode.W)) move = speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) move = -speed * Time.deltaTime;

            if (move != 0)
            {
                // Pohyb všech platforem v seznamu
                for (int i = 0; i < platforms.Count; i++)
                {
                    // Pohyb platformy
                    platforms[i].position += new Vector3(0, move, 0);

                    // Pokud platforma překročí limit Y = -8 nebo Y = 8, resetuje se na původní pozici
                    if (platforms[i].position.y <= -8 || platforms[i].position.y >= 8)
                    {
                        platforms[i].position = startPositions[i]; // Vrátí na původní pozici
                        Debug.Log("Platforma " + i + " resetována na výchozí pozici!");
                    }
                }
            }
        }
    }
}
