namespace GlitchEscape.Effects {
    public abstract class EffectState<TOwner, TState> where TState : EffectState<TOwner, TState> {
        private TOwner owner { get; }
        private EffectList<TOwner, TState> effects { get; }

        public EffectState(TOwner owner) {
            this.owner = owner;
            effects = new EffectList<TOwner, TState>((TState) this);
        }

        public Effect<TOwner, TState> CreateEffect(StateEffector<TOwner, TState> effect) {
            return new Effect<TOwner, TState>(effect, effects);
        }

        public void Reset() {
            effects.ClearEffects();
        }

        protected abstract void SetDefaults(TOwner user);

        public void SetDefaults() {
            SetDefaults(owner);
        }
    }
}