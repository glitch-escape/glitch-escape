using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlitchEscape.Effects {
    public delegate void StateEffector<TOwner, in TState>(TState state) where TState : EffectState<TOwner, TState>;

    public class Effect<TOwner, TState> where TState : EffectState<TOwner, TState> {
        private StateEffector<TOwner, TState> applyEffects { get; }
        private EffectList<TOwner, TState> target;

        public Effect(StateEffector<TOwner, TState> applyEffects, EffectList<TOwner, TState> target) {
            this.applyEffects = applyEffects;
            this.target = target;
            Debug.Log(""+Time.time+" Created effect "+this);
            this.target?.AddEffect(this);
        }

        public void Apply(TState state) {
            Debug.Log(""+Time.time+" Applying effect "+this);
            applyEffects(state);
        }

        private bool _active = true;

        public bool active {
            get => _active && !cancelled;
            set {
                var prevActive = _active;
                _active = value;
                if (value != prevActive) {
                    target?.ReapplyEffects();
                }
            }
        }

        public void Cancel() {
            Debug.Log(""+Time.time+" Cancelling effect effect "+this);
            var t = target;
            target = null;
            t?.RemoveEffect(this);
        }

        public bool cancelled => target == null;
    }
}