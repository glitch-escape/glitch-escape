using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlitchEscape.Effects {
    public class EffectList<TOwner, TState> where TState : EffectState<TOwner, TState> {
        // use list to preserve ordering
        private TState state { get; }
        private List<Effect<TOwner, TState>> activeEffects { get; } = new List<Effect<TOwner, TState>>();
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
            Debug.Log(""+Time.time+" Added effect: "+effect);
            activeEffects.Add(effect);
            if (effect.active) {
                dirty = true;
                ReapplyEffects();
            }
        }

        public void RemoveEffect(Effect<TOwner, TState> effect) {
            activeEffects.Remove(effect);
            Debug.Log(""+Time.time+" Removing effect: "+effect);
            dirty = true;
            ReapplyEffects();
        }

        private void ReapplyEffects() {
            if (dirty) {
                Debug.Log(""+Time.time+" dirty, reapplying "+activeEffects.Count+" effect(s)");
                dirty = false;
                state.SetDefaults();
                activeEffects.RemoveAll(effect => effect.cancelled);
                Debug.Log("" + Time.time + " applying " + activeEffects.Count + " remaining effect(s)");
                activeEffects.ForEach(effect => {
                    if (effect.active) {
                        effect.Apply(state);
                    }
                });
            }
        }

        public void ClearEffects() {
            Debug.Log(""+Time.time+" Clearing all effects");
            activeEffects.ForEach(effect => effect.Cancel());
            activeEffects.Clear();
            dirty = true;
            ReapplyEffects();
        }
    }
}