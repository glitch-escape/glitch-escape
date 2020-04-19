using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlitchEscape.Effects {
    class EffectBag<TOwner, TState> : IEnumerable<EffectData<TOwner, TState>> where TState : EffectState<TOwner, TState> {
        private SortedSet<EffectData<TOwner, TState>> effects { get; } = new SortedSet<EffectData<TOwner, TState>>();
        private int nextId = 0;
        public IEffectHandle AddEffect<TEffector>(TEffector effector, IEffectController effectController = null) 
            where TEffector : struct, IEffector<TOwner, TState>
        {
            var effect = new EffectData<TOwner, TState>(++nextId, effectController);
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
    
    public class EffectList<TOwner, TState> where TState : EffectState<TOwner, TState> {
        // use list to preserve ordering
        private TState state { get; }
        private List<Effect<TOwner, TState>> activeEffects { get; } = new List<Effect<TOwner, TState>>();
        private bool dirty { get; set; } = true;
        private bool reapplyingEffects = false;
        
        public EffectList(TState state) {
            this.state = state;
        }

        public void AddEffect(Effect<TOwner, TState> effect) {
            // inefficient but not called frequently; preserves order and is and cheap for small N
            foreach (var activeEffect in activeEffects) {
                if (effect == activeEffect)
                    return;
            }
            Debug.Log(""+Time.time+" Added effect: "+effect);
            activeEffects.Add(effect);
            if (effect.active) {
                dirty = true;
                if (!reapplyingEffects) ReapplyEffects();
            }
        }

        public void RemoveEffect(Effect<TOwner, TState> effect) {
            Debug.Log(""+Time.time+" Removing effect: "+effect);
            dirty = true;
            if (!reapplyingEffects) ReapplyEffects();
        }

        public void ReapplyEffects() {
            reapplyingEffects = true;
            dirty = false;
            state.SetDefaults();
            activeEffects.RemoveAll(effect => effect.cancelled);
            Debug.Log("" + Time.time + " applying " + activeEffects.Count + " remaining effect(s)");
            activeEffects.ForEach(effect => {
                if (effect.active) {
                    effect.Apply(state);
                }
            });
            reapplyingEffects = false;
        }

        public void ClearEffects() {
            reapplyingEffects = true;
            Debug.Log(""+Time.time+" Clearing all effects");
            activeEffects.ForEach(effect => effect.Cancel());
            activeEffects.Clear();
            dirty = true;
            reapplyingEffects = false;
            ReapplyEffects();
        }
    }
}