using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWhereGoing : Align
{
    // Override Align's getTargetAngle to look where we're going instead of matching our target's orientation
    public override float getTargetAngle()
    {
        // Get the velocity of the character
        Vector3 velocity = character.linearVelocity;
        if (velocity.magnitude == 0)
        {
            // If velocity is zero, just return the current orientation
            return character.transform.eulerAngles.y;
        }

        // Calculate the target angle based on velocity direction
        float targetAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        return targetAngle; // No need to multiply by Rad2Deg again
    }
}