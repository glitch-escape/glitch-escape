using System;
using System.Collections;
using System.Collections.Generic;
using GlitchEscape.Effects;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, IResettable {
    public static PlayerController instance { get; private set; }
    [SerializeField] [InjectComponent] public Player player;
    [SerializeField][InjectComponent] public PlayerControls playerControls;
    private interface IPlayerAbilityController : IResettable {
        void Update(PlayerController provider);
    }
    private List<IPlayerAbilityController> controllers;
    
    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // pattern to use on enable + on disable to handle code assembly reloading
    // (TODO: figure out a better / more efficient way to do this - though note, this class should never get disabled
    // + re-enabled normally anyways, so we're not expecting these to be called outside of file save => partial assembly
    // rebuild + reload (potentially breaking existing references) iff editing code while in unity play mode; ditto
    // use of this pattern elsewhere that subscribes / unsubscribes to events, etc)
    private void OnEnable() {
        controllers =
            new List<IPlayerAbilityController> {
                new PlayerDashController(this), 
                new PlayerJumpController(this),
            };
    }
    private void OnDisable() {
        controllers = null;
    }
    private void Update() {
        controllers?.ForEach(controller => controller.Update(this));
    }
    public void Reset() {
        controllers?.ForEach(controller => controller.Reset());
    }
    // ability controller that starts dash when button pressed + dash can be started
    // + extends duration depending on time pressed until release
    private class PlayerDashController : IPlayerAbilityController {
        private PlayerController provider;
        public PlayerDashController (PlayerController provider) { this.provider = provider; }
        enum State {None,Pressed}
        private State state;
        private float startPressTime;
        private float usedStamina;
        
        // used player dash duration
        public float GetDuration() {
            return state == State.None
                ? 0f
                : provider.player.dash.GetAbilityDurationFromPressDuration(Time.time - startPressTime);
        }
        public void Update(PlayerController provider) {
            if (provider.playerControls.dash.wasPressedThisFrame) {
                // var effect = provider.player.dash.TryUseAbilityAsEffect()?.WithDuration(this.GetDuration);
                // if (effect != null) {
                //     effect.Restart();
                var prevDuration = provider.player.dash.currentPressDuration;
                provider.player.dash.currentPressDuration = 0f;
                if (provider.player.TryUseAbility(provider.player.dash)) {
                    state = State.Pressed;
                    startPressTime = Time.time;
                    usedStamina = provider.player.dash.GetStaminaCostFromPressDuration(0f);
                } else {
                    state = State.None;
                    provider.player.dash.currentPressDuration = prevDuration;
                }
            }
            if (state == State.Pressed) {
                var duration = Time.time - startPressTime;
                provider.player.dash.currentPressDuration = duration;
                var currentStaminaCost = provider.player.dash.GetStaminaCostFromPressDuration(duration);
                if (currentStaminaCost > usedStamina) {
                    if ((provider.player.stamina.value -= currentStaminaCost - usedStamina) <= 0f) {
                        provider.player.dash.CancelAbility();
                        state = State.None;
                    }
                }
                usedStamina = currentStaminaCost;
            }
            if (provider.playerControls.dash.wasReleasedThisFrame) {
                state = State.None;
            }
        }
        public void Reset() {
            state = State.None;
        }
    }
    // basic ability controller that just starts an ability when jump pressed + ability can be started
    class PlayerJumpController : IPlayerAbilityController {
        private PlayerController provider;
        public PlayerJumpController (PlayerController provider) { this.provider = provider; }

        public void Update(PlayerController provider) {
            if (provider.playerControls.jump.wasPressedThisFrame) {
                // provider.player.jump.TryUseAbilityAsEffect()?.Restart();
                provider.player.TryUseAbility(provider.player.jump);
            }
        }
        public void Reset() { }
    }
}
