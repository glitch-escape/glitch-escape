namespace GlitchEscape.Effects {
    struct BasicEffectBehavior : IEffectBehavior {
        public bool active { get; set; }
        public bool finished => false;

        public void OnCancelled() {}
        public void Update() {}
        public static BasicEffectBehavior Create() {
            return new BasicEffectBehavior { active = true };
        }
    }
}