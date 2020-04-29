namespace GlitchEscape.Effects {
    public interface IEffectBehavior : IResettable {
        bool active { get; set; }
        bool finished { get; }
        void OnCancelled();
        void Update();
    }
}