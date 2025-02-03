using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehavior
{
    public Kinematic character;
    public GameObject target;

    float maxAngularAcceleration = 1000f;
    float maxRotation = 360f; // maxAngularVelocity

    // The radius for arriving at the target
    float targetRadius = 1f;

    // The radius for beginning to slow down
    float slowRadius = 10f;

    // The time over which to achieve target speed
    float timeToTarget = 0.05f;

    // Returns the angle in degrees that we want to align with
    public virtual float getTargetAngle()
    {
        // Use the direction of movement (velocity) to determine the target angle
        Vector3 velocity = character.linearVelocity;
        if (velocity.magnitude == 0) // If velocity is zero, use the current orientation
        {
            return character.transform.eulerAngles.y;
        }

        // Otherwise calculate the angle based on velocity direction
        float targetAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        return targetAngle; // return the calculated angle directly
    }

    public override SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        // Get the naive direction to the target (velocity-based target angle)
        float rotation = Mathf.DeltaAngle(character.transform.eulerAngles.y, getTargetAngle());
        float rotationSize = Mathf.Abs(rotation);

        // Get the current angular velocity
        float currentAngularVelocity = float.IsNaN(character.angularVelocity) ? 0f : character.angularVelocity;

        // If we are within the target radius, stop rotating
        if (rotationSize < targetRadius)
        {
            result.angular = -currentAngularVelocity; // Smoothly reduce angular velocity to zero
            return result;
        }

        // If we are outside the slow radius, use the maximum rotation speed
        float targetRotation = 0.0f;
        if (rotationSize > slowRadius)
        {
            targetRotation = maxRotation;
        }
        else // Otherwise use a scaled rotation
        {
            targetRotation = maxRotation * rotationSize / slowRadius;
        }

        // The final targetRotation combines speed and direction
        targetRotation *= rotation / rotationSize;

        // Apply damping to smooth out the rotation
        result.angular = (targetRotation - currentAngularVelocity) * 1.0f;
        result.angular /= timeToTarget;

        // Check if the angular acceleration is too great
        float angularAcceleration = Mathf.Abs(result.angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            result.angular /= angularAcceleration;
            result.angular *= maxAngularAcceleration;
        }

        result.linear = Vector3.zero;
        return result;
    }
}