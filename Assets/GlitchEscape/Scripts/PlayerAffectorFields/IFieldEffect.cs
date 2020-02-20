using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFieldEffect {
    void SetupField(PlayerAffectorField barrier);
    void OnPlayerEnter(Player player);
    void OnPlayerExit(Player player);
    void OnPlayerTick(Player player);
}
