using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyVisionController : EnemyComponent {
    public abstract bool CanSeePlayer();
    public abstract bool HasLastKnownPlayerPosition(out Vector3 lastPosition);
    public abstract void SetVisionDistanceFactor(float factor);
    public abstract void DebugShowDetectionRadius(bool enabled);
    public abstract List<IEnemyObjectiveMarker> GetKnownObjectiveMarkers();
    public abstract void ClearKnownObjectiveMarkers();
}
