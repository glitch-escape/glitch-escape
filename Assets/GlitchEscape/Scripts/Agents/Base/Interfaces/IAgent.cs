using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgent : IResettable {
    AgentType agentType { get; }
    IAgentAbility[] abilities { get; }
    GameObject gameObject { get; }
    bool isTargetableBy(AgentType type);
    void TakeDamage(float damage);
    void Kill();
}
