using UnityEngine;

namespace GlitchEscape.Effects.Duration {
    class DurationEffectWrapper : IDurationEffect {
        private IEffect effect;
        public IDurationEffectBehavior durationBehavior { get; set; }
        public IEffectBehavior effectBehavior {
            get => durationBehavior;
            set {
                if (value is IDurationEffectBehavior behavior) {
                    durationBehavior = behavior;
                } else {
                    durationBehavior = null;
                }
                effect.effectBehavior = value;
            }
        }
        public DurationEffectWrapper(IEffect effect, IDurationEffectBehavior behavior) {
            this.effect = effect;
            this.durationBehavior = behavior;
        }
        public float duration {
            get => durationBehavior?.duration ?? 0f;
            set { if (durationBehavior != null) durationBehavior.duration = value; }
        }
        public float endTime {
            get => durationBehavior?.endTime ?? 0f;
            set { if (durationBehavior != null) durationBehavior.endTime = value; }
        }
        public float startTime => durationBehavior?.startTime ?? 0f;
        public float elapsedTime => durationBehavior?.elapsedTime ?? 0f;
        public float remainingTime => durationBehavior?.remainingTime ?? 0f;
        public bool started => durationBehavior?.started ?? false;
        public bool paused => durationBehavior?.paused ?? false;
        public bool active {
            get => effect.active;
            set => effect.active = value;
        }
        public bool finished => effect.finished;
        public void Cancel() { effect.Cancel(); }
        public void Start() { effect.active = true; }
        public void Pause() { effect.active = false; }
    }

    public interface IDurationEffectActual {
        float duration { get; set; }
        void OnStarted();
        void OnFinished();
        void OnCancelled();
        void Update();
    }

    public struct DurationEffectBridge : IDurationEffectBehavior {
        private IDurationEffectActual behavior;
        public DurationEffectBridge(IDurationEffectActual behavior) {
            this.behavior = behavior;
            _flags = 0;
            _startTime = 0;
            _endTime = 0;
        }
        public float duration {
            get => behavior.duration;
            set => behavior.duration = value;
        }
        public void OnStarted() {
            if (!started) {
                _startTime = Time.time;
                _flags |= FLAG_STARTED;
                behavior.OnStarted();
            } else if (!active) {
                _startTime = Time.time - elapsedTime;
            }
            _flags |= FLAG_ACTIVE;
        }
        public void OnCancelled() {
            if (!wasSetFinished) {
                _flags |= FLAG_CANCELLED;
                behavior.OnCancelled();
            }
        }
        public void OnFinished() {
            if (!wasSetFinished) {
                _flags |= FLAG_FINISHED;
                _endTime = Time.time;
                behavior.OnFinished();
            }
        }
        public void OnPaused() {
            if (active) {
                _flags &= ~FLAG_ACTIVE;
                _endTime = Time.time;
                behavior.OnPaused();
            }
        }
        public void Update() {
            if (!started || wasSetFinished) return;
            if (finished) OnFinished();
            else behavior.Update();
        }

        private int   _flags;
        private float _startTime;
        private float _endTime;

        private const int FLAG_STARTED = 0x1;
        private const int FLAG_ACTIVE = 0x2;
        private const int FLAG_FINISHED = 0x4;
        private const int FLAG_CANCELLED = 0x8;
        public bool started => (_flags & FLAG_STARTED) != 0;
        private bool wasSetFinished => (_flags & (FLAG_FINISHED | FLAG_CANCELLED)) != 0;
        public bool finished => wasSetFinished || started && elapsedTime >= duration;
        public bool paused => (_flags & FLAG_ACTIVE) == 0;
        public bool active {
            get => started && !paused;
            set {
                if (wasSetFinished) return;
                if (value) {
                    OnStarted();
                } else if (started) {
                    OnPaused();
                }
            }
        }
        public float startTime => _startTime;
        public float endTime {
            get => 
                !active ? _endTime :
                started ? _startTime + duration : Time.time + duration;
            set {
                if (!started || active) duration = Mathf.Clamp(value - Time.time, 0f, Mathf.Infinity);
                else if (!finished) duration = Mathf.Clamp(value - _endTime, 0f, Mathf.Infinity); 
            }
        }
        public float elapsedTime =>
            !started ? 0f : active ? Time.time - _startTime : _endTime - _startTime;
        public float remainingTime => duration - elapsedTime;
    }
}