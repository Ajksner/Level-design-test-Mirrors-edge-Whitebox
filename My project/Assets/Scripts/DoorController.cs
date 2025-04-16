using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Nastavení dveří")]
    public Transform doorPivot; // Místo, kolem kterého se otáčí
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Chování dveří")]
    public bool closeOnExit = true;
    public bool openOnce = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool hasOpened = false;

    private void Start()
    {
        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        closedRotation = doorPivot.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasOpened)
        {
            StopAllCoroutines();
            StartCoroutine(RotateDoor(openRotation));

            if (openOnce)
            {
                hasOpened = true;
                GetComponent<Collider>().enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && closeOnExit && !openOnce)
        {
            StopAllCoroutines();
            StartCoroutine(RotateDoor(closedRotation));
        }
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(doorPivot.rotation, targetRotation) > 0.1f)
        {
            doorPivot.rotation = Quaternion.Lerp(doorPivot.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        doorPivot.rotation = targetRotation;
    }
}
