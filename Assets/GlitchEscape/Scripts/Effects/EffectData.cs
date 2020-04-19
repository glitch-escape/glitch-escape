using System;
using System.Text;
using UnityEngine;

namespace GlitchEscape.Effects {
    class EffectData<TOwner, TState> : IEffect, IComparable<EffectData<TOwner, TState>> 
            where TState : EffectState<TOwner, TState> {
        public int id { get; private set; }
        private EffectState<TOwner, TState> owner { get; }
        private IEffector<TOwner, TState> effector;
        private Type effectorType;
        public IEffectBehavior effectBehavior { get; set; }

        private bool cancelled => (_flags & EFFECT_CANCELLED_FLAG) != 0;
        private int _flags;
        
        public void SetEffector<TEffector>(TEffector effector) where TEffector : struct, IEffector<TOwner, TState> {
            this.effector = effector;
            effectorType = typeof(TEffector);
        }

        public EffectData(int id, EffectState<TOwner, TState> owner, IEffectBehavior behavior = null) {
            this.id = id;
            this.owner = owner;
            this.effectBehavior = behavior;
            this._flags = 0;
        }
        public void Apply(TState state) {
            effector.Apply(state);   
        }
        public void UpdateController() {
            effectBehavior?.Update();
        }
        
        // do full blown stringification using reflection
        public override string ToString() {
            var type = effectorType;
            var fields = type.GetFields();
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} effect (id {2}) {3} {{",
                active ? "active" : "inactive",
                typeof(TOwner).Name, id, type.Name);
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
        public void Cancel() {
            if (!cancelled) {
                _flags |= SET_CANCELLED_FLAG;
                effectBehavior?.OnCancelled();
                owner.RebuildState();
            }
        }
        public int CompareTo(EffectData<TOwner, TState> other) {
            return id.CompareTo(other.id);
        }
    }
}