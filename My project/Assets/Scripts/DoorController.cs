using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Nastavení dveří")]
    public Transform doorTransform; // Odkaz na objekt dveří
    public float openAngle = 90f; // Úhel otevření
    public float openSpeed = 2f; // Rychlost otáčení

    [Header("Chování dveří")]
    public bool closeOnExit = true; // Zavřít po odchodu z triggeru
    public bool openOnce = false; // Otevřít jen jednou a pak deaktivovat trigger

    private Quaternion closedRotation; // Výchozí rotace
    private Quaternion openRotation; // Cílová rotace
    private bool hasOpened = false; // Sleduje, zda už se dveře otevřely

    private void Start()
    {
        if (doorTransform == null)
        {
            doorTransform = transform; // Pokud není přiřazen objekt dveří, vezme se sám
        }

        closedRotation = doorTransform.rotation;
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
                hasOpened = true; // Dveře už se nebudou znovu aktivovat
                GetComponent<Collider>().enabled = false; // Vypne trigger
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
        while (Quaternion.Angle(doorTransform.rotation, targetRotation) > 0.1f)
        {
            doorTransform.rotation = Quaternion.Lerp(doorTransform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        doorTransform.rotation = targetRotation;
    }
}
