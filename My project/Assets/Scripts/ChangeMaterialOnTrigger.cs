using UnityEngine;
using System.Collections;

public class ChangeMaterialOnTrigger : MonoBehaviour
{
    public GameObject targetObject; // Objekt, jehož materiál budeme měnit
    public Material newMaterial; // Nový materiál při vstupu do triggeru
    
    [Header("Nastavení přechodu")]
    public bool useTransition = false; // Volba mezi okamžitou změnou a plynulým přechodem
    public float transitionDuration = 0.5f; // Doba trvání přechodu v sekundách

    private Material defaultMaterial; // Původní materiál
    private Coroutine activeTransition; // Reference na aktivní coroutinu přechodu

    private void Start()
    {
        if (targetObject != null)
        {
            // Uložíme původní materiál
            Renderer renderer = targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                defaultMaterial = renderer.material;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetObject != null && newMaterial != null)
        {
            Renderer renderer = targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (useTransition)
                {
                    // Zastavení předchozího přechodu, pokud nějaký probíhá
                    if (activeTransition != null)
                    {
                        StopCoroutine(activeTransition);
                    }
                    
                    // Spuštění nového přechodu
                    activeTransition = StartCoroutine(TransitionMaterial(renderer, newMaterial));
                }
                else
                {
                    // Okamžitá změna materiálu
                    renderer.material = newMaterial;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetObject != null && defaultMaterial != null)
        {
            Renderer renderer = targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (useTransition)
                {
                    // Zastavení předchozího přechodu, pokud nějaký probíhá
                    if (activeTransition != null)
                    {
                        StopCoroutine(activeTransition);
                    }
                    
                    // Spuštění nového přechodu
                    activeTransition = StartCoroutine(TransitionMaterial(renderer, defaultMaterial));
                }
                else
                {
                    // Okamžitá změna materiálu
                    renderer.material = defaultMaterial;
                }
            }
        }
    }

    private IEnumerator TransitionMaterial(Renderer renderer, Material targetMaterial)
    {
        // Vytvoření dočasného materiálu pro přechod
        Material currentMat = renderer.material;
        Material tempMaterial = new Material(currentMat);
        renderer.material = tempMaterial;

        // Vytvoření kopie cílového materiálu
        Material targetMaterialCopy = new Material(targetMaterial);
        
        float elapsedTime = 0;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            
            // Interpolace barev
            if (tempMaterial.HasProperty("_Color") && targetMaterialCopy.HasProperty("_Color"))
            {
                tempMaterial.color = Color.Lerp(currentMat.color, targetMaterialCopy.color, t);
            }
            
            // Další vlastnosti materiálu lze interpolovat zde
            // Například:
            if (tempMaterial.HasProperty("_Metallic") && targetMaterialCopy.HasProperty("_Metallic"))
            {
                float metallic = Mathf.Lerp(
                    tempMaterial.GetFloat("_Metallic"),
                    targetMaterialCopy.GetFloat("_Metallic"),
                    t
                );
                tempMaterial.SetFloat("_Metallic", metallic);
            }
            
            if (tempMaterial.HasProperty("_Glossiness") && targetMaterialCopy.HasProperty("_Glossiness"))
            {
                float smoothness = Mathf.Lerp(
                    tempMaterial.GetFloat("_Glossiness"),
                    targetMaterialCopy.GetFloat("_Glossiness"),
                    t
                );
                tempMaterial.SetFloat("_Glossiness", smoothness);
            }
            
            yield return null;
        }
        
        // Po dokončení přechodu nastavíme cílový materiál
        renderer.material = targetMaterial;
        
        // Uvolnění prostředků
        Destroy(tempMaterial);
        Destroy(targetMaterialCopy);
        
        activeTransition = null;
    }
}