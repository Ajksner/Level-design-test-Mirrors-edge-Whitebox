using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour
{
    [Header("Nastavení barvy")]
    public Color targetColor = Color.red; // Barva, na kterou se změní
    public bool smoothTransition = true; // Plynulé prolínání (true) nebo okamžitá změna (false)
    public float transitionSpeed = 2f; // Rychlost plynulé změny

    [Header("Trigger zóna")]
    public Collider triggerZone; // Manuálně vybraný trigger

    private Color startColor; // Původní barva
    private Renderer objRenderer;
    private bool isInsideTrigger = false;

    private void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            startColor = objRenderer.material.color; // Uložení původní barvy
        }

        // Ověření, zda je trigger nastaven
        if (triggerZone == null)
        {
            Debug.LogWarning("Nebyl přiřazen žádný trigger! Přiřaďte jej ručně v Inspectoru.", this);
        }
        else if (!triggerZone.isTrigger)
        {
            Debug.LogWarning("Přiřazený collider není trigger! Zapněte isTrigger v Inspectoru.", triggerZone);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == triggerZone) // Ověří, zda hráč vstoupil do zadané trigger zóny
        {
            isInsideTrigger = true;
            if (!smoothTransition)
            {
                objRenderer.material.color = targetColor; // Okamžitá změna barvy
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(ChangeColorOverTime(targetColor));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == triggerZone)
        {
            isInsideTrigger = false;
            if (!smoothTransition)
            {
                objRenderer.material.color = startColor; // Okamžitý návrat na původní barvu
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(ChangeColorOverTime(startColor));
            }
        }
    }

    private IEnumerator ChangeColorOverTime(Color newColor)
    {
        Color currentColor = objRenderer.material.color;
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            objRenderer.material.color = Color.Lerp(currentColor, newColor, t);
            yield return null;
        }
    }
}
