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
        public IEffect AddEffect<TEffector>(TEffector effector, IEffectBehavior effectBehavior = null) 
            where TEffector : struct, IEffector<TOwner, TState>
        {
            var effect = new EffectData<TOwner, TState>(++nextId, owner, effectBehavior);
            effect.SetEffector(effector);
            effects.Add(effect);
            Debug.Log("Added effect: "+effect);
            if (effect.active)
                owner.RebuildState();
            return effect;
        }
        public void ApplyStateEffects(TState state) {
            Debug.Log("Applying effects (have "+effects.Count+" effect(s))");
            foreach (var effect in effects) {
                if (effect.finished)
                    Debug.Log("  removing "+effect);
            }
            effects.RemoveWhere(effect => effect.finished);
            foreach (var effect in effects) {
                if (effect.active) {
                    Debug.Log("  applying "+effect);
                    effect.Apply(state);
                }
                else {
                    Debug.Log("  skipping "+effect);
                }
            }
            Debug.Log("  done");
        }
        public void Clear() {
            // Debug.Log("EffectList: Clear()");
            foreach (var effect in effects) {
                if (!effect.finished) {
                    effect.Cancel();
                }
            }
            effects.Clear();
            // Debug.Log("  after clear have "+effects.Count+" item(s):");
            foreach (var effect in effects) {
                Debug.Log("  - "+effect);
            }            
            nextId = 0;
            owner.RebuildState();
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
        public int Count => effects.Count;
    }
}