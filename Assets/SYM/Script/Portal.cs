using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {
    public Portal linkedPortal;
    List<PortalTraveller> trackedTravellers = new List<PortalTraveller>();

    void LateUpdate() {
        for (int i = 0; i < trackedTravellers.Count; i++) {
            var traveller = trackedTravellers[i];
            var travellerT = traveller.transform;
            var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offset = travellerT.position - transform.position;
            if (Vector3.Dot(offset, transform.forward) * Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward) < 0) {
                traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
                linkedPortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt(i);
                i--;
            } else {
                traveller.previousOffsetFromPortal = offset;
            }
        }
    }

    void OnTravellerEnterPortal(PortalTraveller traveller) {
        if (!trackedTravellers.Contains(traveller)) {
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out PortalTraveller traveller)) {
            OnTravellerEnterPortal(traveller);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out PortalTraveller traveller) && trackedTravellers.Contains(traveller)) {
            trackedTravellers.Remove(traveller);
        }
    }
}
