namespace GlitchEscape.Effects.Duration {
    public interface IDurationEffect : IEffect {
        IDurationEffectBehavior durationBehavior { get; set; }
        float duration { get; set; }
        
        bool started { get; }
        bool paused { get; }
        
        float startTime { get; }
        float endTime { get; set; }
        float elapsedTime { get; }
        float remainingTime { get; }
        void Start();
        void Stop();
    }
    public interface IDurationEffectBehavior {
        float duration { get; set; }
        void OnStarted();
        void OnEnded();
        void OnPausedOrResumed(bool active);
        void OnCancelled();
        void OnReset();
        void Update();
    }
    struct BasicDurationBehavior : IDurationEffectBehavior {
        public float duration { get; set; }
        public void OnStarted() {}
        public void OnEnded() {}
        public void OnPausedOrResumed(bool active) {}
        public void OnReset() {}
        public void OnCancelled() {}
        public void Update() {}
    }
    
    public delegate float DurationGetter();
    struct BasicDurationFunctionBehavior : IDurationEffectBehavior {
        public DurationGetter getDuration;
        public float duration {
            get => getDuration?.Invoke() ?? 0f;
            set { }
        }
        public void OnStarted() {}
        public void OnEnded() {}
        public void OnPausedOrResumed(bool active) {}
        public void OnReset() {}
        public void OnCancelled() {}
        public void Update() {}
    }
    
    
}