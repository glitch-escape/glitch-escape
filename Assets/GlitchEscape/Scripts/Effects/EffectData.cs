using System;
using System.Text;
using GlitchEscape.Effects.Duration;
using UnityEngine;

namespace GlitchEscape.Effects {
    class EffectData<TOwner, TState> : IEffect, IDurationEffect, IComparable<EffectData<TOwner, TState>> 
            where TState : EffectState<TOwner, TState> {
        
        #region Internal data
        #region Fields
        public int id { get; private set; }
        private EffectState<TOwner, TState> owner { get; }
        private IEffector<TOwner, TState> _effector;
        private Type _effectorType;

        // one of these is set at any given time
        private IEffectBehavior _effectBehavior;
        private IDurationEffectBehavior _durationBehavior;

        // effect state + other flags
        private int   _flags = 0;
        private float _startTime;
        private float _endTime;
        
        #endregion Fields
        #region Flags
        // guard flags to block setter chains from recursing into oblivion
        private const int SETTING_ACTIVE_FLAG = 0x1;
        private const int SETTING_FINISHED_FLAG = 0x2;
        
        // internal state
        private const int EFFECT_IS_DURATION_FLAG = 0x4;
        private const int EFFECT_WAS_EFFECT_STARTED_FLAG = 0x8;
        private const int EFFECT_WAS_EFFECT_CANCELLED_FLAG = 0x10;
        private const int EFFECT_WAS_EFFECT_FINISHED_FLAG = 0x20;
        private const int EFFECT_WAS_EFFECT_PAUSED_FLAG = 0x40;
        
        private void SetFlag(int flag, bool value) {
            if (value) _flags |= flag;
            else _flags &= ~flag;
        }
        private bool GetFlag(int flag) {
            return (_flags & flag) != 0;
        }
        #endregion Flags
        #region Internal properties
        private bool isDurationEffect {
            get => GetFlag(EFFECT_IS_DURATION_FLAG);
            set => SetFlag(EFFECT_IS_DURATION_FLAG, value);
        }
        private bool wasCancelled {
            get => GetFlag(EFFECT_WAS_EFFECT_CANCELLED_FLAG);
            set => SetFlag(EFFECT_WAS_EFFECT_CANCELLED_FLAG, value);
        }
        private bool wasFinished {
            get => GetFlag(EFFECT_WAS_EFFECT_FINISHED_FLAG);
            set => SetFlag(EFFECT_WAS_EFFECT_FINISHED_FLAG, value);
        }
        private bool wasStarted {
            get => GetFlag(EFFECT_WAS_EFFECT_STARTED_FLAG);
            set => SetFlag(EFFECT_WAS_EFFECT_STARTED_FLAG, value);
        }
        private bool wasPaused {
            get => GetFlag(EFFECT_WAS_EFFECT_PAUSED_FLAG);
            set => SetFlag(EFFECT_WAS_EFFECT_PAUSED_FLAG, value);
        }
        #endregion Flags getters and setters
        #endregion Internal data
        #region Constructors
        public EffectData(int id, EffectState<TOwner, TState> owner) {
            this.id = id;
            this.owner = owner;
        }
        public EffectData(int id, EffectState<TOwner, TState> owner, IEffectBehavior behavior) : this(id, owner) {
            SetBehavior(behavior);
        }
        public EffectData(int id, EffectState<TOwner, TState> owner, IDurationEffectBehavior behavior) : this(id, owner) {
            SetBehavior(behavior);
        }
        #endregion
        #region Effector setter
        public void SetEffector<TEffector>(TEffector effector) where TEffector : struct, IEffector<TOwner, TState> {
            _effector = effector;
            _effectorType = typeof(TEffector);
        }
        #endregion
        #region Behavior getters + setters
        public IEffectBehavior effectBehavior {
            get => !isDurationEffect ? _effectBehavior : null;
            set {
                if (!isDurationEffect) _effectBehavior = value;
            }
        }
        public IDurationEffectBehavior durationBehavior {
            get => isDurationEffect ? _durationBehavior : null;
            set {
                if (isDurationEffect) _durationBehavior = value;
            }
        }
        private void SetBehavior(IEffectBehavior behavior) {
            _effectBehavior = behavior;
            isDurationEffect = false;
        }
        private void SetBehavior(IDurationEffectBehavior behavior) {
            _durationBehavior = behavior;
            isDurationEffect = true;
        }
        public IEffect WithBehavior(IEffectBehavior behavior) { SetBehavior(behavior); return this; }
        public IDurationEffect WithDuration(IDurationEffectBehavior behavior) { SetBehavior(behavior); return this; }
        public IDurationEffect WithDuration(float duration) {
            return WithDuration(new BasicDurationBehavior {duration = duration});
        }
        public IDurationEffect WithDuration(DurationGetter getDuration) {
            return WithDuration(new BasicDurationFunctionBehavior {getDuration = getDuration});
        }
        #endregion
        #region State application
        public void Apply(TState state) {
            _effector.Apply(state);   
        }
        #endregion State application
        #region Stringification + comparison
        // do full blown stringification using reflection
        public override string ToString() {
            var type = _effectorType;
            var fields = type.GetFields();
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} effect (id {2}) {3} {{",
                active ? "active" : "inactive",
                typeof(TOwner).Name, id, type.Name);
            var first = true;
            foreach (var field in fields) {
                var fmt = first ? "{0} {1} = {2}" : ", {0} {1} = {2}";
                sb.AppendFormat(fmt, field.FieldType.Name, field.Name, field.GetValue(_effector));
                first = false;
            }
            sb.Append("}");
            return sb.ToString();
        }
        public int CompareTo(EffectData<TOwner, TState> other) {
            return id.CompareTo(other.id);
        }
        #endregion
        
        #region Behavior callbacks
        private void OnStarted() {
            if (isDurationEffect) _durationBehavior.OnStarted();
        }
        private void OnFinished() {
            if (isDurationEffect) _durationBehavior.OnEnded();
        }
        private void OnActiveChanged(bool active) {
            if (isDurationEffect) _durationBehavior.OnPausedOrResumed(active);
        }
        private void OnCancelled() {
            if (isDurationEffect) _durationBehavior.OnCancelled();
            else _effectBehavior?.OnCancelled();
        }
        private void OnReset() {
            if (isDurationEffect) _durationBehavior.Reset();
            else _effectBehavior?.Reset();
        }
        #endregion
        
        #region Duration methods + properties
        public void Start() {
            if (wasStarted) return;
            wasStarted = true;
            if (wasPaused) {
                _startTime = Time.time - elapsedTime;
            } else {
                _startTime = Time.time;
            }
            wasPaused = false;
            if (isDurationEffect)
                _durationBehavior?.OnStarted();
        }
        public void Stop() {
            if (wasPaused) return;
            wasPaused = true;
            if (wasStarted) _endTime = Time.time;
            if (isDurationEffect) _durationBehavior?.OnEnded();
        }
        public float startTime =>
            wasStarted ? _startTime : 0f;
        
        public float elapsedTime =>
            !wasStarted ? 0f : Mathf.Clamp(
                wasFinished || wasPaused ? 
                    _endTime - startTime :
                    Time.time - startTime,
                0f, Mathf.Infinity);
        public float remainingTime =>
            Mathf.Clamp(duration - elapsedTime, 0f, Mathf.Infinity);

        public float endTime {
            get => 
                !wasStarted ? 0f :
                wasFinished || wasPaused ? _endTime : Time.time;
            set {
                if (wasStarted)
                    duration = value - _startTime;
            }
        }
        public float duration {
            get => isDurationEffect && durationBehavior != null
                ? durationBehavior.duration
                : Mathf.Clamp(_endTime - _startTime, 0f, Mathf.Infinity);
            set {
                if (isDurationEffect && durationBehavior != null) {
                    durationBehavior.duration = value;
                } else if (wasStarted) {
                    _endTime = _startTime + value;
                }
            }
        }
        #endregion


        public void Stop() {
            
        }
        public void Cancel() {
            if (!isCancelled) {
                
            }
        }
        public void Reset() {
            
        }
        public void Update() {
            if (!active) return;
            if (isDurationEffect) _durationBehavior.Update();
            else _effectBehavior.Update();
        }
        #endregion IEffect + IDurationEffect methods
        
        public void Cancel() {
            if (!wasCancelled) {
                wasCancelled = true;
                if (isDurationEffect)
                    _durationBehavior?.OnCancelled();
                else
                    _effectBehavior?.OnCancelled();
                owner.RebuildState();
            }
        }

        
        
        // sets cancelled _and_ disables setting active or finished, as this effect is now cancelled and propagating
        // value assignments that will have no effects is meaningless
        private const int SET_CANCELLED_FLAG = 0xf;
        public bool active {
            get => !cancelled && (effectBehavior?.active ?? true);
            set {
                Debug.Log("Set active = "+value+": "+this);
                if (cancelled || effectBehavior == null || (_flags & SETTING_ACTIVE_FLAG) != 0
                    || effectBehavior.active == value)
                    return;
                Debug.Log(" => effect changed");
                _flags |= SETTING_ACTIVE_FLAG;
                effectBehavior.active = value;
                _flags &= ~SETTING_ACTIVE_FLAG;
                owner.RebuildState();
            }
        }
        public bool finished {
            get => cancelled || (effectBehavior?.finished ?? false);
            set {
                Debug.Log("Set finished = "+value+": "+this);
                if (effectBehavior == null || (_flags & SETTING_FINISHED_FLAG) == 0
                    || effectBehavior.finished == value)
                    return;
                Debug.Log(" => effect changed");
                if (value) Cancel();
            }
        }
        
        
    }
}