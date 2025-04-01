using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Examples
{
    public class Teleporter : MonoBehaviour
    {
        public Teleporter TeleportTo;
        public float liftHeight = 2f; // Výška, o kterou se hráč posune nahoru
        public float liftDuration = 1f; // Celková doba pohybu (ale teď poleze nahoru jen polovinu času)

        public UnityAction<ExampleCharacterController> OnCharacterTeleport;
        public bool isBeingTeleportedTo { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!isBeingTeleportedTo)
            {
                ExampleCharacterController cc = other.GetComponent<ExampleCharacterController>();
                if (cc)
                {
                    StartCoroutine(LiftAndTeleport(cc));
                }
            }
        }

        private IEnumerator LiftAndTeleport(ExampleCharacterController cc)
        {
            isBeingTeleportedTo = true;
            Vector3 startPosition = cc.transform.position;
            Vector3 liftPosition = startPosition + Vector3.up * liftHeight;
            float elapsedTime = 0f;
            float halfDuration = liftDuration / 2f; // Polovina původního času

            // Plynulý pohyb nahoru (za poloviční dobu)
            while (elapsedTime < halfDuration)
            {
                cc.Motor.SetPosition(Vector3.Lerp(startPosition, liftPosition, elapsedTime / halfDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ujistíme se, že hráč je na správné pozici
            cc.Motor.SetPosition(liftPosition);

            // Teleportace na cílovou lokaci
            cc.Motor.SetPositionAndRotation(TeleportTo.transform.position, TeleportTo.transform.rotation);

            if (OnCharacterTeleport != null)
            {
                OnCharacterTeleport(cc);
            }

            TeleportTo.isBeingTeleportedTo = true;
            isBeingTeleportedTo = false;
        }
    }
}
