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
        public float forwardOffset = 1f; // Posunutí dopředu
        public float totalMoveDuration = 1f; // Celková doba přesunu (nahoru + dopředu)

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
            
            Vector3 finalPosition;
            if (TeleportTo != null)
            {
                finalPosition = TeleportTo.transform.position + TeleportTo.transform.forward * forwardOffset;
            }
            else
            {
                finalPosition = liftPosition + cc.transform.forward * forwardOffset;
            }

            float halfDuration = totalMoveDuration / 2f; // Doba pro každou část (nahoru i dopředu)

            // Plynulý pohyb nahoru (polovina času)
            float elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                cc.Motor.SetPosition(Vector3.Lerp(startPosition, liftPosition, elapsedTime / halfDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            cc.Motor.SetPosition(liftPosition);

            if (TeleportTo != null)
            {
                cc.Motor.SetPositionAndRotation(TeleportTo.transform.position, TeleportTo.transform.rotation);
                TeleportTo.isBeingTeleportedTo = true;
                OnCharacterTeleport?.Invoke(cc);
            }

            // Plynulý pohyb dopředu (druhá polovina času)
            elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                cc.Motor.SetPosition(Vector3.Lerp(cc.transform.position, finalPosition, elapsedTime / halfDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            cc.Motor.SetPosition(finalPosition);

            isBeingTeleportedTo = false;
        }
    }
}
