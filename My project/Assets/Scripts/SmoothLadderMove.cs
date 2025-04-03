using UnityEngine;
using System.Collections;

public class SmoothLadderMove : MonoBehaviour
{
    [Header("Nastavení pohybu")]
    public Transform endPoint;     // Cílový bod (můžete vybrat ručně)
    public Transform controlPoint; // Kontrolní bod (určuje oblouk trajektorie)
    public float moveDuration = 1.5f; // Doba přesunu

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && endPoint != null && controlPoint != null)
        {
            StartCoroutine(MoveAlongCurve(other.transform));
        }
    }

    private IEnumerator MoveAlongCurve(Transform player)
    {
        Vector3 start = player.position;
        Vector3 control = controlPoint.position;
        Vector3 end = endPoint.position;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            player.position = QuadraticBezier(start, control, end, t);
            yield return null;
        }

        player.position = end; // Ujistíme se, že hráč skončí přesně v cíli
    }

    private Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
}
