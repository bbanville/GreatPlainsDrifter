/******************************************************************************
   File Name: ThirdPersonCharacter.cs
   Author(s): Brendon Banville (brendonbanville@gmail.com)
     Project: BBTools

 Description: This file contains the functionality of a player character.
******************************************************************************/

using UnityEngine;

namespace BrendonBanville.Controllers
{
	public class ThirdPersonCharacter : ThirdPersonAnimator
	{
        void Start()
		{

		}

        public virtual void Sprint(bool value)
        {
            isSprinting = value;
        }

        public virtual void Strafe()
        {
            if (movementType == MovementType.OnlyFree) { return; }
            isStrafing = !isStrafing;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Jump()
        {
            // conditions to do this action
            bool jumpConditions = isGrounded && !isJumping;

            // return if jumpCondigions is false
            if (!jumpConditions) { return; }

            // trigger jump behaviour
            jumpCounter = jumpTimer;
            isJumping = true;

            // trigger jump animations            
            if (_rigidbody.velocity.magnitude < 1)
            {
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            }
            else
            {
                animator.CrossFadeInFixedTime("JumpMove", 0.2f);
            }
        }

        /// <summary>
        /// Rotates the player character with a given transform
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void RotatePlayerWithAnotherTransform(Transform referenceTransform)
        {
            var newRotation = new Vector3(transform.eulerAngles.x, referenceTransform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), strafeRotationSpeed * Time.fixedDeltaTime);
            targetRotation = transform.rotation;
        }
	}
}
