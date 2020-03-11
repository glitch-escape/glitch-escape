using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgentAbility : IResettable {
    IAgent agent { get; }
    bool canUseAbility { get; }
    bool TryUseAbility();
}
