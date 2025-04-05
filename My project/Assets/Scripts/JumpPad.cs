using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using KinematicCharacterController.Examples;

public class JumpPad : MonoBehaviour
{
    public float liftHeight = 2f; // Výška pohybu
    public float forwardDistance = 3f; // Vzdálenost pohybu dopředu
    public float moveDuration = 1f; // Doba pohybu
    public float upwardForce = 10f; // Síla odrazu vzhůru
    public float forwardForce = 5f; // Síla odrazu dopředu
    public bool overrideVerticalVelocity = true; // Přepsání vertikální rychlosti
    public UnityAction<ExampleCharacterController> OnJumpComplete;

    private ExampleCharacterController player;
    private bool isMoving = false;

    private void OnTriggerEnter(Collider other)
    {
        ExampleCharacterController cc = other.GetComponent<ExampleCharacterController>();
        if (cc)
        {
            player = cc;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ExampleCharacterController cc = other.GetComponent<ExampleCharacterController>();
        if (cc && player == cc)
        {
            player = null;
        }
    }

    private void Update()
    {
        if (player != null && !isMoving && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(LiftAndMove(player));
        }
    }

    private IEnumerator LiftAndMove(ExampleCharacterController cc)
    {
        isMoving = true;
        Vector3 startPosition = cc.transform.position;
        Vector3 moveDirection = cc.transform.forward * forwardDistance;
        Vector3 liftPosition = startPosition + Vector3.up * liftHeight + moveDirection;
        float elapsedTime = 0f;

        // Pohybování hráče do požadované pozice
        while (elapsedTime < moveDuration)
        {
            cc.Motor.SetPosition(Vector3.Lerp(startPosition, liftPosition, elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cc.Motor.SetPosition(liftPosition);
        
        // Aplikování síly po dokončení pohybu
        ApplyJumpForce(cc);

        isMoving = false;
        OnJumpComplete?.Invoke(cc);
    }

    private void ApplyJumpForce(ExampleCharacterController cc)
    {
        // Získání aktuální rychlosti z KinematicCharacterMotor
        Vector3 currentVelocity = cc.Motor.BaseVelocity;
        Vector3 newVelocity;

        if (overrideVerticalVelocity)
        {
            // Přepíše vertikální rychlost a zachová horizontální komponenty
            newVelocity = new Vector3(
                currentVelocity.x,
                upwardForce,  // Vertikální síla
                currentVelocity.z
            );
        }
        else
        {
            // Přidá k současné vertikální rychlosti
            newVelocity = new Vector3(
                currentVelocity.x,
                currentVelocity.y + upwardForce,
                currentVelocity.z
            );
        }

        // Přidání dopředné síly
        newVelocity += cc.transform.forward * forwardForce;

        // Aplikování nové rychlosti
        cc.Motor.BaseVelocity = newVelocity;
    }
}
