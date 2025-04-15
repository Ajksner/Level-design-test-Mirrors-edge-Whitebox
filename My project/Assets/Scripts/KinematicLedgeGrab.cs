using UnityEngine;

public class KinematicLedgeGrab : MonoBehaviour
{
    [Header("Nastavení")]
    public float hangOffset = 0.3f;
    public float climbUpHeight = 1.5f;
    public float moveSpeed = 2f;

    [Header("Status")]
    public bool isHanging = false;

    private Rigidbody rb;
    private CharacterController charController;
    private Vector3 hangPosition;
    private Quaternion hangRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isHanging)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ClimbUp();

            if (Input.GetKeyDown(KeyCode.S))
                LetGo();

            float horizontal = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontal) > 0.1f)
                MoveHorizontally(horizontal);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isHanging) return;
        if (!other.CompareTag("LedgeTrigger")) return;

        Transform ledge = other.transform;
        Vector3 ledgePoint = ledge.position;
        Vector3 ledgeNormal = -ledge.forward;

        GrabLedge(ledgePoint, ledgeNormal);
    }

    void GrabLedge(Vector3 ledgePoint, Vector3 normal)
    {
        isHanging = true;

        hangPosition = ledgePoint - normal * hangOffset - Vector3.up * 0.5f;
        hangRotation = Quaternion.LookRotation(-normal);

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (charController != null)
            charController.enabled = false;

        transform.position = hangPosition;
        transform.rotation = hangRotation;
    }

    void ClimbUp()
    {
        if (!isHanging) return;

        transform.position += Vector3.up * climbUpHeight + transform.forward * 0.5f;

        isHanging = false;

        if (rb != null) rb.isKinematic = false;
        if (charController != null) charController.enabled = true;
    }

    void LetGo()
    {
        if (!isHanging) return;

        isHanging = false;

        if (rb != null) rb.isKinematic = false;
        if (charController != null) charController.enabled = true;
    }

    void MoveHorizontally(float input)
    {
        Vector3 moveDir = transform.right * input * moveSpeed * Time.deltaTime;
        transform.position += moveDir;
    }
}
