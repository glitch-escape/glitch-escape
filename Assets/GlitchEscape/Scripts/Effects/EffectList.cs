using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlitchEscape.Effects {
    class EffectList<TOwner, TState> : IEnumerable<EffectData<TOwner, TState>> where TState : EffectState<TOwner, TState> {
        private EffectState<TOwner, TState> owner { get; }
        private SortedSet<EffectData<TOwner, TState>> effects { get; } = new SortedSet<EffectData<TOwner, TState>>();
        private int nextId = 0;

        public EffectList(EffectState<TOwner, TState> owner) {
            this.owner = owner;
        }
        public IEffectHandle AddEffect<TEffector>(TEffector effector, IEffectController effectController = null) 
            where TEffector : struct, IEffector<TOwner, TState>
        {
            var effect = new EffectData<TOwner, TState>(++nextId, owner, effectController);
            effect.SetEffector(effector);
            effects.Add(effect);
            Debug.Log("Added effect: "+effect);
            return effect;
        }
        public void ApplyStateEffects(TState state) {
            effects.RemoveWhere(effect => effect.finished);
            foreach (var effect in effects) {
                if (effect.active) {
                    effect.Apply(state);
                }
            }   
        }
        public void Clear() {
            foreach (var effect in effects) {
                if (!effect.finished) {
                    effect.Cancel();
                }
            }
            effects.Clear();
            nextId = 0;
        }
        public void UpdateControllers() {
            var hasFinishedEffects = false;
            foreach (var effect in effects) {
                if (effect.active) {
                    effect.UpdateController();
                } else if (effect.finished) {
                    hasFinishedEffects = true;
                }
            }
            if (hasFinishedEffects) {
                effects.RemoveWhere(effect => effect.finished);
            }
        }
        public IEnumerator<EffectData<TOwner, TState>> GetEnumerator() {
            foreach (var effect in effects) {
                yield return effect;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}