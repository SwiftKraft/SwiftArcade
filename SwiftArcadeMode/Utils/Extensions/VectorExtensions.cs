namespace SwiftArcadeMode.Utils.Extensions
{
    using UnityEngine;

    public static class VectorExtensions
    {
        public static Vector3 PredictPosition(this Vector3 position, Vector3 targetPos, Vector3 targetVel, float projectileSpeed, float timer = 0f)
        {
            if (targetVel == Vector3.zero)
                return targetPos;

            float dist = Vector3.Distance(position, targetPos);
            float requiredTime;

            if (projectileSpeed > 0)
                requiredTime = dist / projectileSpeed;
            else
                requiredTime = 0f;

            return targetPos + (targetVel * (requiredTime + timer));
        }

        /// <summary>
        /// Calculates the initial velocity needed to hit a target with a gravity-affected projectile.
        /// </summary>
        /// <param name="origin">Start position.</param>
        /// <param name="target">Target position.</param>
        /// <param name="speed">Projectile launch speed.</param>
        /// <param name="useHighArc">True = high arc, False = low arc.</param>
        /// <param name="velocity">Resulting initial velocity.</param>
        /// <returns>False if the target is unreachable at the given speed.</returns>
        public static bool SolveBallisticArc(
            Vector3 origin,
            Vector3 target,
            float speed,
            bool useHighArc,
            out Vector3 velocity)
        {
            velocity = Vector3.zero;

            Vector3 delta = target - origin;
            Vector3 deltaXZ = new(delta.x, 0f, delta.z);

            float distanceXZ = deltaXZ.magnitude;
            float height = delta.y;

            float gravity = Mathf.Abs(Physics.gravity.y);
            float speed2 = speed * speed;
            float speed4 = speed2 * speed2;

            float discriminant =
                speed4 - (gravity * ((gravity * distanceXZ * distanceXZ) + (2f * height * speed2)));

            // No valid solution (target too far or too high)
            if (discriminant < 0f)
                return false;

            float sqrt = Mathf.Sqrt(discriminant);

            float angle =
                Mathf.Atan2(
                    speed2 + (useHighArc ? sqrt : -sqrt),
                    gravity * distanceXZ);

            Vector3 directionXZ = deltaXZ.normalized;

            velocity =
                (directionXZ * Mathf.Cos(angle) * speed) +
                (Vector3.up * Mathf.Sin(angle) * speed);

            return true;
        }
    }
}
