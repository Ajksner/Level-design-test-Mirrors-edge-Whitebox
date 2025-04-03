using UnityEngine;

public class LadderPlatform : MonoBehaviour
{
    public float climbSpeed = 3f;  // Rychlost pohybu plošiny
    private Transform player;
    private Vector3 startPosition;
    private bool isClimbing = false;

    private void Start()
    {
        startPosition = transform.position; // Uloží výchozí pozici plošiny
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isClimbing = true;
            player = other.transform;
            player.SetParent(transform); // Hráč se "přilepí" k plošině
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isClimbing = false;
            player.SetParent(null); // Hráč se odpojí od plošiny
            transform.position = startPosition; // Plošina se vrátí na původní místo
        }
    }

    private void Update()
    {
        if (isClimbing)
        {
            float move = Input.GetAxis("Vertical") * climbSpeed * Time.deltaTime;
            transform.position += new Vector3(0, move, 0);
        }
    }
}
