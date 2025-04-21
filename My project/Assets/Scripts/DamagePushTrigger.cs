using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DamagePushTrigger : MonoBehaviour {
    [Header("Aktivace triggeru")]
    public bool isActive = true;
    
    [Header("Který trigger má být aktivní")]
    public Collider allowedTrigger;
    
    [Header("Push efekt")]
    public float pushForce = 10f;
    
    [Header("UI efekt")]
    public GameObject damageOverlayObject;
    public float fadeDuration = 1f;
    
    private Image damageOverlayImage;
    
    private void Start()
    {
        // Pokud nebyl trigger explicitně nastaven, použije se ten aktuální objekt
        if (allowedTrigger == null)
        {
            allowedTrigger = GetComponent<Collider>();
        }
        
        // Získání Image komponenty a deaktivace overlay objektu
        if (damageOverlayObject != null)
        {
            damageOverlayImage = damageOverlayObject.GetComponent<Image>();
            // Začínáme s deaktivovaným overlay
            damageOverlayObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        
        if (this.GetComponent<Collider>() != allowedTrigger) return;
        
        if (other.CompareTag("Player"))
        {
            // Push effect
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (other.transform.position - transform.position).normalized;
                rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
            }
            
            // Damage overlay efekt
            if (damageOverlayObject != null)
            {
                StopAllCoroutines();
                StartCoroutine(ShowDamageOverlay());
            }
        }
    }
    
    private IEnumerator ShowDamageOverlay()
    {
        // Aktivujeme overlay
        damageOverlayObject.SetActive(true);
        
        // Nastavíme plnou viditelnost
        if (damageOverlayImage != null)
        {
            Color color = damageOverlayImage.color;
            color.a = 0.8f;
            damageOverlayImage.color = color;
        }
        
        // Počkáme moment, aby byl efekt viditelný
        yield return new WaitForSeconds(0.1f);
        
        // Fade efekt
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (damageOverlayImage != null)
            {
                Color color = damageOverlayImage.color;
                color.a = Mathf.Lerp(0.8f, 0f, elapsed / fadeDuration);
                damageOverlayImage.color = color;
            }
            yield return null;
        }
        
        // Deaktivujeme overlay
        damageOverlayObject.SetActive(false);
    }
}