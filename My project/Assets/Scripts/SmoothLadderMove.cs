using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using KinematicCharacterController.Examples;

public class SmoothLadderMove : MonoBehaviour
{
    [Header("Nastavení pohybu")]
    public Transform endPoint;     // Cílový bod
    public Transform controlPoint; // Kontrolní bod
    public float moveDuration = 1.5f; // Doba přesunu
    public UnityAction<ExampleCharacterController> OnMoveComplete;

    private ExampleCharacterController player;
    private bool isMoving = false;

    private void OnTriggerEnter(Collider other)
    {
        ExampleCharacterController cc = other.GetComponent<ExampleCharacterController>();
        if (cc && !isMoving)
        {
            player = cc;
            StartCoroutine(MoveAlongCurve(player));  // Spustí pohyb ihned při vstupu do triggeru
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

    private IEnumerator MoveAlongCurve(ExampleCharacterController cc)
    {
        isMoving = true;
        Vector3 startPosition = cc.transform.position;
        Vector3 control = controlPoint.position;
        Vector3 end = endPoint.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            // Používáme Quadratic Bezier pro pohyb
            Vector3 newPosition = QuadraticBezier(startPosition, control, end, t);
            cc.Motor.SetPosition(newPosition);
            yield return null;
        }

        cc.Motor.SetPosition(end); // Ujistíme se, že hráč skončí přesně v cíli
        isMoving = false;

        OnMoveComplete?.Invoke(cc);
    }

    // Quadratic Bezier pro výpočet pohybu
    private Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
}
