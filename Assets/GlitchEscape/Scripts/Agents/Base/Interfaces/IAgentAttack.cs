using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgentAttack {
    float damage { get; }
    AgentType targetType { get; }
}
