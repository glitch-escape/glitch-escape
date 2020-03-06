using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviorExtensions {
    public static T GetEnforcedComponentReference<T>(this MonoBehaviour self, ref T componentReference) where T : class {
        return componentReference ?? (componentReference = Enforcements.GetComponent<T>(self));
    }
    public static T GetEnforcedComponentReferenceInChildren<T>(this MonoBehaviour self, ref T componentReference) where T : class {
        return componentReference ?? (componentReference = Enforcements.GetComponentInChildren<T>(self));
    }
    public static T GetEnforcedComponentReferenceInParent<T>(this MonoBehaviour self, ref T componentReference) where T : class {
        return componentReference ?? (componentReference = Enforcements.GetComponentInParent<T>(self));
    }
}
