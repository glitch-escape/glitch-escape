using UnityEngine;

namespace GlitchEscape.Effects {
    struct EmptyEffectController : IEffectController {
        public bool active { get; set; }
        public bool finished { get; set; }
        public void OnCancelled() {}
        public void Update() {}
        public static EmptyEffectController Create() {
            return new EmptyEffectController { active = true, finished = false };
        }
    }
    
    public abstract class EffectState<TOwner, TState> where TState : EffectState<TOwner, TState> {
        private TOwner owner { get; }
        private EffectList<TOwner, TState> effects { get; }

        public EffectState(TOwner owner) {
            this.owner = owner;
            effects = new EffectList<TOwner, TState>(this);
            SetDefaults(owner);
        }

        public IEffectHandle CreateEffect<TEffector>(TEffector effector)
            where TEffector : struct, IEffector<TOwner, TState> {
            return effects.AddEffect(effector, EmptyEffectController.Create());
        }

        public void Reset() {
            _rebuildingState = true;
            Debug.Log("State: Reset()");
            effects.Clear();
            _rebuildingState = false;
            RebuildState();
        }

        protected abstract void SetDefaults(TOwner user);

        public void SetDefaults() {
            // Debug.Log("State: SetDefaults()");
            SetDefaults(owner);
        }
        public void Update() {
            // Debug.Log("State: Update()");
            effects.UpdateControllers();
        }

        private bool _rebuildingState = false;
        public void RebuildState() {
            if (_rebuildingState) return;
            _rebuildingState = true;
            Debug.Log("State: RebuildState()");
            SetDefaults();
            effects.ApplyStateEffects((TState)this);
            _rebuildingState = false;
        }
    }
}