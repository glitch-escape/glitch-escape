namespace GlitchEscape.Effects {
    public interface IEffect {
        IEffectBehavior effectBehavior { get; set; }
        bool active { get; set; }
        bool finished { get; }
        void Cancel();
    }
}