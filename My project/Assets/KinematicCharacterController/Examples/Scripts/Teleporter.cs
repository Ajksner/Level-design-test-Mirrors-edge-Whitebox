using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Examples
{
    public class Teleporter : MonoBehaviour
    {
        [Header("Teleport Settings")]
        public Teleporter TeleportTo;
        public bool useTeleportTarget = true;

        [Header("Lift Settings")]
        public float liftHeight = 2f;
        public float totalMoveDuration = 1f;

        [Tooltip("Po zvednutí se postava ještě posune dopředu (před teleportem nebo pushem)")]
        public bool moveForwardAfterLift = false;
        public float forwardMoveDistance = 1f;
        public float forwardMoveDuration = 0.5f;

        [Header("Push Settings (if no teleport)")]
        public float forcePower = 10f;
        public float forcePushDelay = 0.5f;

        public UnityAction<ExampleCharacterController> OnCharacterTeleport;
        public bool isBeingTeleportedTo { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!isBeingTeleportedTo)
            {
                ExampleCharacterController cc = other.GetComponent<ExampleCharacterController>();
                if (cc)
                {
                    StartCoroutine(useTeleportTarget ? LiftAndTeleport(cc) : LiftAndPush(cc));
                }
            }
        }

        private IEnumerator LiftAndTeleport(ExampleCharacterController cc)
        {
            isBeingTeleportedTo = true;
            Vector3 startPosition = cc.transform.position;
            Vector3 liftPosition = startPosition + Vector3.up * liftHeight;

            float halfDuration = totalMoveDuration / 2f;

            // Pohyb nahoru
            float elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                cc.Motor.SetPosition(Vector3.Lerp(startPosition, liftPosition, elapsedTime / halfDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            cc.Motor.SetPosition(liftPosition);

            // Volitelný pohyb dopředu před teleportem
            if (moveForwardAfterLift)
            {
                Vector3 forwardTarget = liftPosition + cc.transform.forward * forwardMoveDistance;
                elapsedTime = 0f;
                while (elapsedTime < forwardMoveDuration)
                {
                    cc.Motor.SetPosition(Vector3.Lerp(liftPosition, forwardTarget, elapsedTime / forwardMoveDuration));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                cc.Motor.SetPosition(forwardTarget);
                liftPosition = forwardTarget; // Pro návaznost
            }

            // Teleportace
            if (TeleportTo != null)
            {
                cc.Motor.SetPositionAndRotation(TeleportTo.transform.position, TeleportTo.transform.rotation);
                TeleportTo.isBeingTeleportedTo = true;
                OnCharacterTeleport?.Invoke(cc);
            }

            isBeingTeleportedTo = false;
        }

        private IEnumerator LiftAndPush(ExampleCharacterController cc)
        {
            Vector3 startPosition = cc.transform.position;
            Vector3 liftPosition = startPosition + Vector3.up * liftHeight;
            float liftDuration = totalMoveDuration;

            float elapsedTime = 0f;
            while (elapsedTime < liftDuration)
            {
                cc.Motor.SetPosition(Vector3.Lerp(startPosition, liftPosition, elapsedTime / liftDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            cc.Motor.SetPosition(liftPosition);

            // Volitelný pohyb dopředu před pushem
            if (moveForwardAfterLift)
            {
                Vector3 forwardTarget = liftPosition + cc.transform.forward * forwardMoveDistance;
                elapsedTime = 0f;
                while (elapsedTime < forwardMoveDuration)
                {
                    cc.Motor.SetPosition(Vector3.Lerp(liftPosition, forwardTarget, elapsedTime / forwardMoveDuration));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                cc.Motor.SetPosition(forwardTarget);
                liftPosition = forwardTarget;
            }

            // Delay + push
            yield return new WaitForSeconds(forcePushDelay);

            Rigidbody rb = cc.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = (cc.transform.forward + Vector3.up).normalized;
                rb.AddForce(pushDirection * forcePower, ForceMode.VelocityChange);
            }
        }
    }
}
