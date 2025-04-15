using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFollower : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10f;
    public float moveSpeed = 3f;

    private Rigidbody rb;
    private bool isFollowing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false; // důležité!
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
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.fixedDeltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
