using GlitchEscape.Effects.Types;

namespace GlitchEscape.Effects {
    struct BasicEffectBehavior : IEffectBehavior {
        public bool active { get; set; }
        public BasicEffectBehavior(bool active) { this.active = active; }
        public bool finished => !active;
        
        // this is just used to let the behavior know that it was cancelled so we don't need to do anything
        public void OnCancelled() {}
        public void Update() {}
        public void Reset() {
            active = false;
        }
    }
    struct FunctionalEffectBehavior : IEffectBehavior {
        private FunctionWithNoParametersReturning<bool> getActive { get; }
        public FunctionalEffectBehavior(FunctionWithNoParametersReturning<bool> getActive) {
            this.getActive = getActive;
        }
        public bool active {
            get => getActive();
            set {}
        }
        public bool finished => !active;
        public void OnCancelled() {}
        public void Update() {}
        public void Reset() {}
    }
}