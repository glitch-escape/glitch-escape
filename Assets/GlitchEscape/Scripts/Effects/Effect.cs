using System;
using System.Collections.Generic;

namespace GlitchEscape.Effects {
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
}