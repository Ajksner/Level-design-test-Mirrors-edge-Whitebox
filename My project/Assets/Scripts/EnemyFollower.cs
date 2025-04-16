using System.Collections;
using UnityEngine;
using KinematicCharacterController.Examples;

public class EnemyFollower : MonoBehaviour
{
    [Header("Player Detection")]
    public Transform player;
    public float detectionRadius = 10f;
    public float moveSpeed = 3f;
    public float catchDistance = 1f;
    public float catchHeightRange = 1.5f; // Nově přidaná vertikální tolerance

    [Header("Push Settings")]
    public float upwardForce = 10f;
    public float forwardForce = 15f;
    public bool overrideVerticalVelocity = true;
    public float pushCooldown = 2f;

    [Header("Visual Feedback")]
    public float stunDuration = 0.5f;
    public bool flashColorOnPush = true;
    public Color pushColor = Color.red;

    [Header("Debug Options")]
    public bool printDebugInfo = true;

    private Rigidbody rb;
    private bool isFollowing = false;
    private bool isPushing = false;
    private float lastPushTime = -10f;
    private Color originalColor;
    private Renderer enemyRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.isKinematic = false;

        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null && flashColorOnPush)
        {
            originalColor = enemyRenderer.material.color;
        }

        Collider coll = GetComponent<Collider>();
        if (coll == null)
        {
            Debug.LogWarning("EnemyFollower: No collider found. Adding a default sphere collider.");
            SphereCollider newColl = gameObject.AddComponent<SphereCollider>();
            newColl.radius = 0.5f;
        }

        if (player == null)
        {
            Debug.LogError("EnemyFollower: No player assigned! Please assign player in inspector.");

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("EnemyFollower: Found player by tag.");
            }
        }
    }

    void Update()
    {
        if (player == null || isPushing)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float verticalDifference = Mathf.Abs(transform.position.y - player.position.y);

        if (distanceToPlayer <= detectionRadius)
            isFollowing = true;
        else
            isFollowing = false;

        if (isFollowing && distanceToPlayer <= catchDistance && verticalDifference <= catchHeightRange && !isPushing && Time.time > lastPushTime + pushCooldown)
        {
            if (printDebugInfo)
                Debug.Log("EnemyFollower: Player caught! Attempting push...");

            PushPlayer();
        }
    }

    void FixedUpdate()
    {
        if (player == null || isPushing || !isFollowing)
            return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;

        if (rb != null && !rb.isKinematic)
        {
            rb.MovePosition(rb.position + move);
        }
        else
        {
            transform.position += move;
        }

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  5f * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (player == null || isPushing || Time.time <= lastPushTime + pushCooldown)
            return;

        ExampleCharacterController cc = other.GetComponent<ExampleCharacterController>();
        if (cc != null || other.transform == player || other.transform.root == player)
        {
            if (printDebugInfo)
                Debug.Log("EnemyFollower: Player entered trigger! Attempting push...");

            PushPlayer();
        }
    }

    void PushPlayer()
    {
        if (isPushing)
            return;

        lastPushTime = Time.time;

        if (printDebugInfo)
            Debug.Log("EnemyFollower: Pushing player...");

        ExampleCharacterController cc = player.GetComponent<ExampleCharacterController>();
        if (cc == null)
            cc = player.GetComponentInParent<ExampleCharacterController>();

        if (cc != null && cc.Motor != null)
        {
            ApplyKCCForce(cc);
            StartCoroutine(StunAfterPush());
        }
        else
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDir = (player.position - transform.position).normalized;
                pushDir.y = 0.5f;
                playerRb.AddForce(pushDir * forwardForce + Vector3.up * upwardForce, ForceMode.Impulse);

                StartCoroutine(StunAfterPush());
            }
        }
    }

    private void ApplyKCCForce(ExampleCharacterController cc)
    {
        Vector3 currentVelocity = cc.Motor.BaseVelocity;
        Vector3 newVelocity;

        Vector3 pushDir = (player.position - transform.position).normalized;
        pushDir.y = 0;

        if (overrideVerticalVelocity)
        {
            newVelocity = new Vector3(
                currentVelocity.x,
                upwardForce,
                currentVelocity.z
            );
        }
        else
        {
            newVelocity = new Vector3(
                currentVelocity.x,
                currentVelocity.y + upwardForce,
                currentVelocity.z
            );
        }

        newVelocity += pushDir * forwardForce;

        cc.Motor.BaseVelocity = newVelocity;

        if (printDebugInfo)
            Debug.Log("EnemyFollower: Applied force to KCC! New velocity: " + newVelocity);
    }

    private IEnumerator StunAfterPush()
    {
        isPushing = true;

        if (enemyRenderer != null && flashColorOnPush)
        {
            enemyRenderer.material.color = pushColor;
        }

        yield return new WaitForSeconds(stunDuration);

        if (enemyRenderer != null && flashColorOnPush)
        {
            enemyRenderer.material.color = originalColor;
        }

        isPushing = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Kreslí dvě koule nahoře a dole pro znázornění svislého rozsahu
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        Vector3 up = Vector3.up * catchHeightRange;
        Gizmos.DrawWireSphere(center + up, catchDistance);
        Gizmos.DrawWireSphere(center - up, catchDistance);
    }
}
