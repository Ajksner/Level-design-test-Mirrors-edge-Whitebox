using UnityEngine;
using System.Collections.Generic;

public class LadderMover : MonoBehaviour
{
    public enum MoveDirection { Vertical, Horizontal } // Možnost výběru směru
    public MoveDirection direction = MoveDirection.Vertical;

    public List<Transform> platforms;  // Seznam všech pohyblivých plošin
    public float speed = 3f;           // Rychlost pohybu
    private bool playerInside = false; // Je hráč uvnitř triggeru?
    private List<Vector3> startPositions = new List<Vector3>(); // Původní pozice plošin

    private void Start()
    {
        startPositions.Clear();
        foreach (Transform platform in platforms)
        {
            startPositions.Add(platform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Hráč vstoupil do triggeru!");

            for (int i = 0; i < platforms.Count; i++)
            {
                platforms[i].position = startPositions[i];
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
        if (!playerInside) return;

        float move = 0;

        if (direction == MoveDirection.Vertical)
        {
            if (Input.GetKey(KeyCode.W)) move = speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) move = -speed * Time.deltaTime;
        }
        else if (direction == MoveDirection.Horizontal)
        {
            if (Input.GetKey(KeyCode.D)) move = speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A)) move = -speed * Time.deltaTime;
        }

        if (move == 0) return;

        for (int i = 0; i < platforms.Count; i++)
        {
            Vector3 moveVector = (direction == MoveDirection.Vertical)
                ? new Vector3(0, move, 0)
                : new Vector3(move, 0, 0);

            platforms[i].position += moveVector;

            bool outOfBounds = (direction == MoveDirection.Vertical && 
                                (platforms[i].position.y <= -8 || platforms[i].position.y >= 8)) ||
                               (direction == MoveDirection.Horizontal && 
                                (platforms[i].position.x <= -8 || platforms[i].position.x >= 8));

            if (outOfBounds)
            {
                platforms[i].position = startPositions[i];
                Debug.Log("Platforma " + i + " resetována na výchozí pozici!");
            }
        }
    }
}
