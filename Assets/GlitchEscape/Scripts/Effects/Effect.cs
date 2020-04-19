using System;

namespace GlitchEscape.Effects {
    public abstract class EffectState<TOwner, TState> where TState : EffectState<TOwner, TState> {
        private TOwner owner { get; }
        private EffectList<TOwner, TState> effects { get; }

        public EffectState(TOwner owner) {
            this.owner = owner;
            effects = new EffectList<TOwner, TState>((TState) this);
        }

        public Effect<TOwner, TState> CreateEffect(StateEffector<TOwner, TState> effect) {
            return new Effect<TOwner, TState>(effect, effects);
        }

        public void Reset() {
            effects.ClearEffects();
        }

        protected abstract void SetDefaults(TOwner user);

        public void SetDefaults() {
            SetDefaults(owner);
        }
    }

    public delegate void StateEffector<TOwner, in TState>(TState state) where TState : EffectState<TOwner, TState>;

    public class Effect<TOwner, TState> where TState : EffectState<TOwner, TState> {
        private StateEffector<TOwner, TState> applyEffects { get; }
        private EffectList<TOwner, TState> target;

        public Effect(StateEffector<TOwner, TState> applyEffects, EffectList<TOwner, TState> target) {
            this.applyEffects = applyEffects;
            this.target = target;
            this.target?.AddEffect(this);
        }

        public void Apply(TState state) {
            applyEffects(state);
        }

        public bool active;

        public void Cancel() {
            target?.RemoveEffect(this);
            target = null;
        }

        public bool cancelled => target == null;
    }

    public class EffectList<TOwner, TState> where TState : EffectState<TOwner, TState> {
        // use list to preserve ordering
        private TState state { get; }
        private List<Effect<TOwner, TState>> activeEffects;
        private bool dirty { get; set; } = true;

        public EffectList(TState state) {
            this.state = state;
        }

        public void AddEffect(Effect<TOwner, TState> effect) {
            // inefficient but not called frequently; preserves order and is and cheap for small N
            foreach (var activeEffect in activeEffects) {
                if (effect == activeEffect)
                    return;
            }

            activeEffects.Add(effect);
            if (effect.active) {
                dirty = true;
                ReapplyEffects();
            }
        }

        public void RemoveEffect(Effect<TOwner, TState> effect) {
            activeEffects.Remove(effect);
            dirty = true;
            ReapplyEffects();
        }

        private void ReapplyEffects() {
            if (dirty) {
                dirty = false;
                state.SetDefaults();
                foreach (var effect in activeEffects) {
                    if (effect.cancelled) {
                        activeEffects.Remove(effect);
                    }
                    else if (effect.active) {
                        effect.Apply(state);
                    }
                }
            }
        }

        public void ClearEffects() {
            foreach (var effect in activeEffects) {
                effect.Cancel();
            }

            activeEffects.Clear();
            dirty = true;
            ReapplyEffects();
        }
    }
}