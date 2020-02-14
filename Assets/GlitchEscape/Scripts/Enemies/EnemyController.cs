using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface that all enemy controller subcomponents must implement
public interface IEnemyControllerComponent {
    void SetupControllerComponent(EnemyController controller);
}

public class EnemyController : MonoBehaviour {

    public Enemy enemy;
    public Player player;

    private void Awake() {
        // Get all references
        if (!enemy) { Debug.LogError("EnemyController: Enemy reference missing!"); }
        if (!player) { player = GameObject.FindObjectOfType<Player>(); }

        // Setup enemy's controller reference
        enemy.controller = this;
        SetupSubControllers(enemy.GetComponents<IEnemyControllerComponent>());
        SetupSubControllers(GetComponents<IEnemyControllerComponent>());
    }

    private void OnEnable() {
        SetupSubControllers(enemy.GetComponents<IEnemyControllerComponent>());
        SetupSubControllers(GetComponents<IEnemyControllerComponent>());
    }

    private void SetupSubControllers(IEnemyControllerComponent[] components) {
        foreach (var component in components) {
            component.SetupControllerComponent(this);
        }
    }
}
