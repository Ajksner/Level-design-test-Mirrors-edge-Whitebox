using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFollower : MonoBehaviour
{
    public Transform player;            // Odkaz na hráče
    public float detectionRadius = 10f; // Radius, ve kterém tě nepřítel detekuje
    public float moveSpeed = 3f;       // Rychlost pohybu nepřítele

    public Transform teleportTarget;   // Cíl, kam bude hráč teleportován, budeš moci vybrat v Unity

    private Rigidbody rb;
    private bool isFollowing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
            isFollowing = true;
        else
            isFollowing = false;

        if (isFollowing)
        {
            // Nepřítel se pohybuje směrem k hráči
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.fixedDeltaTime);

            // Pokud nepřítel dosáhne hráče, teleportuje ho
            if (distanceToPlayer <= 1f) // Když je hráč dostatečně blízko
            {
                TeleportPlayer();
            }
        }
    }

    // Funkce pro teleportování hráče na určené místo
    void TeleportPlayer()
    {
        if (teleportTarget != null)
        {
            player.position = teleportTarget.position; // Nastaví pozici hráče na cíl teleportu
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
