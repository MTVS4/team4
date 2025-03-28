using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PortalPhysicsObject : PortalTraveller
{
    new Rigidbody rigidbody;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        base.Teleport(fromPortal, toPortal, pos, rot);
        rigidbody.linearVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(rigidbody.linearVelocity));
        rigidbody.angularVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(rigidbody.angularVelocity));
    }
}