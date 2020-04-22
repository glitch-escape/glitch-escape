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
            return effects.AddEffect(effector, new BasicEffectBehavior(false));
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
        
        // TODO: consider adding optimizations to do lazy state rebuilds:
        // super low priority but we're technically rebuilding state multiple times per frame as this method is called
        // eagerly by the effects impl whenever anything changes.
        //
        // if this ever is an issue, should be able to trivially optimize this by just adding a _dirtyState flag when
        // you call this method + moving the actual state rebuild to Update(), but this could cause some potentially
        // surprising issues as state updates would then be somewhat async, so in general this simple approach is
        // less efficient but safer, as it will guarantee that effects + state are always updated immediately if
        // any active effect changes / is added / etc.
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