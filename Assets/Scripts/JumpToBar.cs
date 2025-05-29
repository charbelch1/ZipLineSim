using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class JumpToBar : MonoBehaviour
{
    public Rig handRig;
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform Hand;
    public Canvas displayJumpText;

    public HingeJoint hj;

    public Transform barTransform;
    public bool isGrabbing = false;

    public Transform grabBar;

    public Rigidbody barBody;
    public ZiplineBarNoBump startRide;

    [Header("Movement Settings")]
    public float jumpForce = 7f;
    public Transform ground;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public float epsilon = 1f;

    bool isJointCreated;
    public Rigidbody bananaBody;
    private bool isJumping = false;

    void Start()
    {
        bananaBody = GetComponent<Rigidbody>();
        if (handRig != null)
            handRig.weight = 0f;

        isJointCreated = false;
    }

    void FixedUpdate()
    {
        HandleGrabbing();
        HandleMovement();
    }

    void HandleGrabbing()
    {
        if (Mathf.Abs(Hand.position.y - grabBar.position.y) < epsilon)
        {
            Debug.Log("hi");
            isGrabbing = true;
            if (!isJointCreated)
            {
                isJointCreated = true;
                hj = gameObject.GetComponent<HingeJoint>();
                if (hj == null) hj = gameObject.AddComponent<HingeJoint>();

                hj.connectedBody = barBody;
                hj.anchor = new Vector3(0f, 0.15f, 0f);
                hj.axis = Vector3.right;
                hj.autoConfigureConnectedAnchor = true;
                hj.connectedAnchor = new Vector3(3.72529e-07f, -0.01199937f, 1.194f);

                hj.useSpring = false;
                hj.useMotor = false;
                hj.useLimits = false;

                JointLimits lim = hj.limits;
                lim.min = 0f;
                lim.max = 0f;
                lim.bounciness = 0f;
                lim.bounceMinVelocity = float.MaxValue;
                lim.contactDistance = 0f;
                hj.limits = lim;

                hj.breakForce = 5000;
                hj.breakTorque = Mathf.Infinity;

                hj.enableCollision = false;
                hj.enablePreprocessing = true;
                hj.massScale = 1f;
                hj.connectedMassScale = 1f;
            }
        }

        if (isGrabbing)
        {
            leftHandTarget.position = barTransform.position + barTransform.up * 0.2f;
            rightHandTarget.position = barTransform.position + barTransform.up * -0.2f;
            handRig.weight = 1f;

            startRide.enabled = true;
        }
        else
        {
            handRig.weight = 0f;
        }
    }

    void HandleMovement()
    {
        
        if (Input.GetKey(KeyCode.Space) && IsGrounded() && !isGrabbing && !isJumping)
        {
            isGrabbing = true;
            isJumping = true;
            bananaBody.velocity = new Vector3(bananaBody.velocity.x + 0.1f * jumpForce, bananaBody.velocity.y + jumpForce, bananaBody.velocity.z);
            displayJumpText.enabled = false;

        }
    }

    bool IsGrounded()
    {
        return Mathf.Abs(groundCheck.position.y - ground.position.y) < groundCheckRadius;
    }
}
