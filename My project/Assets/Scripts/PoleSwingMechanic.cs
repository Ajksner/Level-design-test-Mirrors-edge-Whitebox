using UnityEngine;

public class PoleSwingMechanic : MonoBehaviour
{
    [Header("References")]
    public Transform pivotPoint;

    [Header("Swing Settings")]
    public float rotationSpeed = 3f;
    public float launchForce = 15f;

    [Header("Debug")]
    public bool isPlayerAttached = false;

    // Internal
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private Vector3 playerOffsetFromPivot;
    private Vector3 originalGravity;
    private float playerDistance;

    private void Start()
    {
        TryFindPlayer();

        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("Collider na PoleSwingMechanic musí být nastaven jako trigger!");
            col.isTrigger = true;
        }
    }

    private void Update()
    {
        if (!isPlayerAttached)
        {
            TryFindPlayer();
            return;
        }

        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            Vector3 rotationAxis = Vector3.right;
            playerOffsetFromPivot = Quaternion.AngleAxis(-verticalInput * rotationSpeed, rotationAxis) * playerOffsetFromPivot;

            Vector3 newPosition = pivotPoint.position + playerOffsetFromPivot;
            playerRigidbody.MovePosition(newPosition);

            Quaternion newRotation = Quaternion.LookRotation(-playerOffsetFromPivot.normalized, Vector3.up);
            playerRigidbody.MoveRotation(newRotation);
        }

        if (Input.GetButtonDown("Jump"))
        {
            DetachPlayer(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayerAttached && other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                AttachPlayer();
            }
        }
    }

    private void AttachPlayer()
    {
        if (isPlayerAttached || playerRigidbody == null) return;

        isPlayerAttached = true;
        playerOffsetFromPivot = playerTransform.position - pivotPoint.position;
        playerDistance = playerOffsetFromPivot.magnitude;

        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.useGravity = false;
        playerRigidbody.isKinematic = false;

        Quaternion lookRotation = Quaternion.LookRotation(-playerOffsetFromPivot.normalized, Vector3.up);
        playerRigidbody.MoveRotation(lookRotation);

        Debug.Log("Hráč se zachytil na tyči");
    }

    public void DetachPlayer(bool applyLaunch)
    {
        if (!isPlayerAttached || playerRigidbody == null) return;

        isPlayerAttached = false;
        playerRigidbody.useGravity = true;

        if (applyLaunch)
        {
            Vector3 launchDirection = playerTransform.forward;
            playerRigidbody.velocity = launchDirection * launchForce;
            Debug.Log("Hráč odskočil s rychlostí: " + playerRigidbody.velocity.magnitude);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayerAttached && other.transform == playerTransform)
        {
            DetachPlayer(false);
        }
    }

    private void TryFindPlayer()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                playerRigidbody = playerObj.GetComponent<Rigidbody>();

                if (playerRigidbody == null)
                {
                    Debug.LogWarning("Hráč nemá komponentu Rigidbody!");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (pivotPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pivotPoint.position, 0.2f);

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
            DrawCircle(pivotPoint.position, 2f, 32);
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        float angleStep = 360f / segments;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
