using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ZiplineBarNoBump : MonoBehaviour
{
    [Header("Rope Anchors")]
    public Transform anchorStart;
    public Transform anchorEnd;

    [Header("Friction & Brake")]
    public float frictionCoefficient = 0.03f;
    public Animator bananaAnim;
    public float brakeForce = 20f;

    [Header("Character (optional)")]
    public Rigidbody bananaRoot;
    public JumpToBar bananaControl;
    public Collider ropeRoot;
    public JumpToBar jumpToBar;

    Rigidbody rb;
    Vector3 ropeDir;
    Vector3 ropeDirNormal;
    float ropeLen;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        bananaRoot.useGravity = true;
        ropeRoot.isTrigger = false;
    }

    void FixedUpdate()
    {
        Vector3 delta = anchorEnd.position - anchorStart.position;
        ropeLen = delta.magnitude;
        ropeDir = delta.normalized;
        ropeDirNormal = Vector3.Cross(delta, Vector3.forward).normalized;
        float brakeZoneDistance = ropeLen * 1f;

        Vector3 gAlong = Vector3.Project(Physics.gravity, ropeDir);
        rb.AddForce(gAlong * (bananaRoot.mass + rb.mass), ForceMode.Force);
        rb.AddForce(-Physics.gravity * bananaRoot.mass, ForceMode.Force);

        float speed = Vector3.Dot(rb.velocity, ropeDir);
        Vector3 friction = -Vector3.Dot((Mathf.Sign(speed)
                           * frictionCoefficient
                           * (rb.mass + bananaRoot.mass)
                           * Physics.gravity), ropeDirNormal)
                           * ropeDir;

        if (frictionCoefficient > 0f && Mathf.Abs(speed) > 0.01f)
        {
            rb.AddForce(friction, ForceMode.Force);
        }

        float distFromStart = Vector3.Dot(transform.position - anchorStart.position, ropeDir);
        float distToEnd = ropeLen - distFromStart;
        if (distToEnd < brakeZoneDistance && distToEnd > 0.1)
        {
            rb.AddForce(-brakeForce * ropeDir * (distFromStart / ropeLen), ForceMode.Force);
        }

        if (distToEnd <= 0.5)
        {
            bananaRoot.freezeRotation = true;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            bananaRoot.AddForce(0 * ropeDir, ForceMode.Impulse);
            bananaRoot.useGravity = true;
            bananaControl.isGrabbing = false;
        }
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
