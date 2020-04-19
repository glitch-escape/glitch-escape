namespace GlitchEscape.Effects {
    public abstract class EffectState<TOwner, TState> where TState : EffectState<TOwner, TState> {
        private TOwner owner { get; }
        private EffectList<TOwner, TState> effects { get; }

        public EffectState(TOwner owner) {
            this.owner = owner;
            effects = new EffectList<TOwner, TState>();
            SetDefaults(owner);
        }

        public IEffectHandle CreateEffect<TEffector>(TEffector effector)
            where TEffector : struct, IEffector<TOwner, TState> {
            return effects.AddEffect(effector);
        }

        public void Reset() {
            effects.Clear();
        }

        protected abstract void SetDefaults(TOwner user);

        public void SetDefaults() {
            SetDefaults(owner);
        }
        public void Update() {
            effects.UpdateControllers();
        }
    }
}