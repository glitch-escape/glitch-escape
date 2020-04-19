using System;
using System.Text;

namespace GlitchEscape.Effects {
    public interface IEffector<TOwner, TState> where TState : EffectState<TOwner, TState> {
        void Apply(TState state);
    }
    public interface IEffectHandle {
        IEffectController effectController { get; set; }
        bool active { get; set; }
        bool finished { get; }
        void Cancel();
    }
    

    public interface IEffectController {
        bool active { get; set; }
        bool finished { get; }
        void OnCancelled();
        void Update();
    }
    class EffectData<TOwner, TState> : IEffectHandle, IComparable<EffectData<TOwner, TState>> 
            where TState : EffectState<TOwner, TState> {
        public int id { get; private set; }
        private IEffector<TOwner, TState> effector;
        private Type effectorType;
        public IEffectController effectController { get; set; }

        private bool cancelled => (_flags & EFFECT_CANCELLED_FLAG) != 0;
        private int _flags;
        
        public void SetEffector<TEffector>(TEffector effector) where TEffector : struct, IEffector<TOwner, TState> {
            this.effector = effector;
            effectorType = typeof(TEffector);
        }

        public EffectData(int id, IEffectController controller = null) {
            this.id = id;
            this.effectController = controller;
            this._flags = 0;
        }
        public void Apply(TState state) {
            effector.Apply(state);   
        }
        public void UpdateController() {
            effectController?.Update();
        }
        
        // do full blown stringification using reflection
        public override string ToString() {
            var type = effectorType;
            var fields = type.GetFields();
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} effect {2} {{",
                active ? "active" : "inactive",
                typeof(TOwner).Name, type.Name);
            var first = true;
            foreach (var field in fields) {
                var fmt = first ? "{0} {1} = {2}" : ", {0} {1} = {2}";
                sb.AppendFormat(fmt, field.FieldType.Name, field.Name, field.GetValue(effector));
                first = false;
            }
            sb.Append("}");
            return sb.ToString();
        }

        // flags used to block potentially recursive calls through setter chains
        // definitely not threadsafe, but unity monobehaviors aren't concurrent, so yay.
        // I would bring up atomics / CAS... except c# just apparently just flat out guarantees atomicity:
        //     https://medium.com/@wayneye/atomic-operation-in-c-a40590a4d2a8,
        // which is definitely one upside of running on a vm...
        private const int SETTING_ACTIVE_FLAG = 0x1;
        private const int SETTING_FINISHED_FLAG = 0x2;
        private const int EFFECT_CANCELLED_FLAG = 0x4;
        
        // sets cancelled _and_ disables setting active or finished, as this effect is now cancelled and propagating
        // value assignments that will have no effects is meaningless
        private const int SET_CANCELLED_FLAG = 0xf;
        public bool active {
            get => !cancelled && (effectController?.active ?? true);
            set {
                if (cancelled || effectController == null || (_flags & SETTING_ACTIVE_FLAG) != 0
                    || effectController.active == value)
                    return;
                _flags |= SETTING_ACTIVE_FLAG;
                effectController.active = value;
                _flags &= ~SETTING_ACTIVE_FLAG;
            }
        }
        public bool finished {
            get => cancelled || (effectController?.finished ?? false);
            set {
                if (effectController == null || (_flags & SETTING_FINISHED_FLAG) == 0
                    || effectController.finished == value)
                    return;
                _flags |= SETTING_FINISHED_FLAG;
                effectController.active = value;
                _flags &= ~SETTING_FINISHED_FLAG;
            }
        }
        public void Cancel() {
            if (!cancelled) {
                _flags |= SET_CANCELLED_FLAG;
                effectController.OnCancelled();
            }
        }
        public int CompareTo(EffectData<TOwner, TState> other) {
            return id.CompareTo(other.id);
        }
    }
}