using UnityEngine;

namespace GlitchEscape.Effects {
    public interface IDuration : IResettable {
        float duration { get; set; }
        void OnStarted();
        void OnStopped();
        void OnFinished();
        void OnCancelled();
        void Update();
    }

    public struct DurationEffect : IDurationEffect {
        private IEffect effect;
        private DurationEffectBehavior behavior;
        public DurationEffect(IEffect effect, IDuration duration) {
            behavior = new DurationEffectBehavior(effect, duration);
            this.effect = effect.WithCustomBehavior(behavior);
        }
        public bool active {
            get => effect.active;
            set => effect.active = value;
        }
        public bool finished => effect.finished;

        public float duration {
            get => behavior.duration;
            set => behavior.duration = value;
        }
        public float elapsedTime => behavior.elapsedTime;
        public float remainingTime => behavior.remainingTime;
        public bool started => behavior.started;
        
        public IDurationEffect Start () { 
            effect.active = true;
            return this;
        }
        public void Cancel () { effect.Cancel(); }
        public void Stop () { effect.active = false; }
        public void Reset() { effect.Reset(); behavior.Reset(); }
        public void Restart() { Reset(); Start(); }
    }
    public class DurationEffectBehavior : IEffectBehavior {
        private IDuration _duration;
        private bool _started = false;
        private bool _finished = false;
        private bool _active = false;
        private float _startTime = 0f;
        private float _elapsedTime = 0f;
        
        public DurationEffectBehavior(IEffect effect, IDuration duration) {
            _active = effect.active;
            _startTime = Time.time;
            _duration = duration;
            if (_active) {
                _started = true;
                _startTime = Time.time;
                _duration?.OnStarted();
            }
        }
        public bool finished => CheckFinished();
        public bool started { get; private set; }

        public float duration {
            get => _duration?.duration ?? 0f;
            set {
                if (_duration != null) {
                    _duration.duration = value;
                }
            }
        }
        public float elapsedTime => active ? 
            (Time.time - _startTime) + _elapsedTime : 
            _elapsedTime;
        public float remainingTime => 
            Mathf.Clamp(duration - elapsedTime, 0f, Mathf.Infinity);
        public bool active {
            get => _active && started && !finished;
            set {
                if (_active == value) return;
                if (CheckFinished()) {
                    _active = false;
                    return;
                }
                _active = value;
                if (_active) {
                    _started = true;
                    _startTime = Time.time;
                    _duration?.OnStarted();
                } else {
                    _elapsedTime += (Time.time - _startTime);
                    _duration?.OnStopped();
                }
            }
        }
        public void OnCancelled() {
            _finished = true;
            _duration?.OnCancelled();
        }
        public bool CheckFinished() {
            if (_finished) return true;
            if (active && elapsedTime >= duration) {
                _active = false;
                _duration?.OnFinished();
                return true;
            }
            return false;
        }
        public void Update() {
            if (!CheckFinished()) {
                _duration?.Update();
            }
        }
        public void Reset() {
            _started = false;
            _finished = false;
            _active = false;
            _startTime = 0f;
            _elapsedTime = 0f;
            _duration?.Reset();
        }
    }
}