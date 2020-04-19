namespace GlitchEscape.Effects {
    public interface IEffectBehavior {
        bool active { get; set; }
        bool finished { get; }
        void OnCancelled();
        void Update();
    }
}