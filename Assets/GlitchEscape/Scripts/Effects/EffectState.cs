using System.Text;
using UnityEngine;

namespace GlitchEscape.Effects {
    public abstract class EffectState<TOwner, TState> where TState : EffectState<TOwner, TState> {
        private TOwner owner { get; }
        private EffectList<TOwner, TState> effects { get; }

        public EffectState(TOwner owner) {
            this.owner = owner;
            effects = new EffectList<TOwner, TState>(this);
            SetDefaults(owner);
        }

        public IEffect CreateEffect<TEffector>(TEffector effector)
            where TEffector : struct, IEffector<TOwner, TState> {
            return effects.AddEffect(effector, BasicEffectBehavior.Create());
        }

        public void Reset() {
            _rebuildingState = true;
            Debug.Log("State: Reset()");
            effects.Clear();
            _rebuildingState = false;
            RebuildState();
        }

        protected abstract void SetDefaults(TOwner user);

        public void SetDefaults() {
            // Debug.Log("State: SetDefaults()");
            SetDefaults(owner);
        }
        public void Update() {
            // Debug.Log("State: Update()");
            effects.UpdateControllers();
        }

        private bool _rebuildingState = false;
        public void RebuildState() {
            if (_rebuildingState) return;
            _rebuildingState = true;
            Debug.Log("State: RebuildState()");
            SetDefaults();
            effects.ApplyStateEffects((TState)this);
            _rebuildingState = false;
        }
        public override string ToString() {
            var sb = new StringBuilder();
            var type = typeof(TState);
            sb.AppendFormat("{0} state {1} {{", typeof(TOwner).Name, type.Name);
            var first = true;
            foreach (var field in type.GetFields()) {
                var fmt = first ? "{0} = {1}" : ", {0} = {1}";
                sb.AppendFormat(fmt, field.Name, field.GetValue(this));
                first = false;
            }
            sb.AppendFormat("}}\n{0} active effect(s):\n", effects.Count);
            foreach (var effect in effects) {
                sb.AppendLine(effect.ToString());
            }
            return sb.ToString();
        }
    }
}