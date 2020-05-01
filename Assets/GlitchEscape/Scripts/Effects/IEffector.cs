namespace GlitchEscape.Effects {
    public interface IEffector<TOwner, TState> where TState : EffectState<TOwner, TState> {
        void Apply(TState state);
    }
}