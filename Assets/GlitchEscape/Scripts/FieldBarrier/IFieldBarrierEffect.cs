using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFieldBarrierEffect {
    void SetupFieldBarrier(FieldBarrier barrier);
    void OnPlayerEnter(Player player);
    void OnPlayerExit(Player player);
    void OnPlayerTick(Player player);
}
