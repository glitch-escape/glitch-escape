using GlitchEscape.Effects.Types;

namespace GlitchEscape.Effects {
    /// <summary>
    /// Represents a temporary in-game effect that can be activated / deactivated,
    /// cancelled, or reset for further reuse.
    ///
    /// Internally wraps an <see cref="IEffector{TOwner,TState}"/>, which is basically just something that applies
    /// an effect / change to a TState state class (ie. SomeObject.State), belonging to a TOwner class (ie. SomeObject)
    ///
    /// This TState type must derive from <see cref="EffectState{TOwner,TState}"/>, which, through CRTP, is the class
    /// that actually manages states + tracking / applying active effects to the TState object itself, and it provides
    /// a method to create <see cref="IEffect"/>s from <see cref="IEffector{TOwner,TState}"/>s:
    ///     <see cref="EffectState{TOwner,TState}.CreateEffect{TEffector}(TEffector)"/>
    ///
    /// Note that an <see cref="IEffector{TOwner,TState}"/> is just a wrapper for a class or function that takes a TState
    /// and applies some modification to it, and TState is "calculated", or rather recalculated, by taking some "default"
    /// TState state + applying every modification linked to every currently active effect on that state, whenever any
    /// effect is created, cancelled, set active / inactive, or reset.
    ///
    /// This may seem like an odd way to manage state, but this concept is rooted in functional programming and
    /// programming language theory, and is the heart of the js react library, and functional reactive programming in
    /// general. In our case it has a few key advantages:
    /// - state changes only need to be described once (ie. a "do" action; don't need to have "do" + "undo" actions for
    /// to model every state modification), and are *much* simpler to track + implement (which means less bugs)
    /// (ie. it has advantages over other alternative implementations)
    /// - it's basically a way to solve the very specific problem of:
    ///     - game system X applies some status effects to object A by writing to some variables
    ///     - game system Y also applies some status effects to object A by also writing to some variables
    ///     - X + Y may start / terminate at different times; how to restore the correct state
    ///         when X made a change, Y overwrote that change, X ends before Y ended,
    ///         ergo A could have the wrong state (eg. when Y ends, restores to X's state but X ended, etc)
    /// - since this system, with <see cref="IDurationEffect"/>, is basically a system + framework to manage
    ///     potentially complex state changes over time, we can use this to replace a lot of *other* complex
    ///     game code
    ///
    /// It also has the following practical advantages:
    /// - the effect system + state pattern can stringify + print any state or effect using c# reflection and
    /// <see cref="EffectData{TOwner,TState}.ToString()"/> and <see cref="EffectState{TOwner,TState}.ToString()"/>
    /// (ie. we can trivially add in-game debug GUIs to list all game states + active effects to pin down any game
    /// mechanic bugs in QA, if those game mechanics use the state pattern + effect system)
    /// - 
    ///
    /// The IEffect interface exists to wrap an <see cref="EffectData{TOwner,TState}"/>, which is the actual effect
    /// implementation, and all of the above templatized machinery describing states + the classes / objects that those
    /// states belong to, with a simple and straightforward interface that omits the above type information.
    /// </summary>
    /// <example>
    ///    class MyGameSystem : MonobehaviourUsingConfig<MyConfig>, IPlayerDebug, IResettable {
    ///        class State : EffectState<MyGameSystem, MyGameSystem.State> {
    ///            float speedMultiplier;
    ///            float extraSpeed;
    ///            protected override void SetDefaults(MyGameSystem owner) {
    ///                this.speedMultiplier = 1f;
    ///                this.extraSpeed = 0f;
    ///                // note: owner provided so you can eg. initialize state properties using owner.config
    ///                // eg. this.speed = owner.config.speed;
    ///            }
    ///        }
    ///        // readonly internal state, modified using effects created using .CreateEffect()
    ///        private State state { get; } = new State();
    ///
    ///        // a state effector
    ///        // structs used (as opposed to lambdas) so we can print all of the parameters + values
    ///        // using c# reflection (you CANNOT do this w/ c# lambdas), and b/c structs are lightweight
    ///        // value types and don't require a constructor (can use new T { ... } syntax).
    ///        struct ApplySpeedEffect : IEffector<State> {
    ///             float multiplier;
    ///             float extraSpeed;
    ///             void Apply (State state) {
    ///                 state.speedMultiplier *= multiplier;
    ///                 state.speed += addSpeed;
    ///             }
    ///        }
    ///
    ///        // an effect created by linking a new state effector to the effect object
    ///        public IEffect ApplySpeedMultiplier (float multiplier) {
    ///             return state.CreateEffect(new ApplySpeedMultiplierEffect {
    ///                 multiplier = multiplier, addSpeed = 0f,
    ///             });
    ///        }
    ///        public IEffect IncreaseSpeed (float speed) {
    ///             return state.CreateEffect(new ApplySpeedMultiplierEffect {
    ///                 multiplier = 0f, addSpeed = speed
    ///             });
    ///        }
    ///
    ///        // system parameters are readonly; all state within game state
    ///        public float speed => state.speed * state.speedMultiplier;
    ///
    ///        // show all state + effect info using the IPlayerDebug system
    ///        public string debugName => typeof(MyGameSystem).Name;
    ///        public void DrawDebugUI () {
    ///            // can print out other stuff but this is all we need to print state + all active effects
    ///            GUILayout.Label(state.ToString());
    ///        }
    ///
    ///        // make state reset when our system resets (note: yes we need to do this explicitely)
    ///        // note that this has the effect of removing (and resetting) _every_ active effect
    ///        // we have created
    ///        // note that the effects are removed but can be restarted (and will re-register themselves)
    ///        // by just setting effect.active = true on an IEffect or IDurationEffect reference.
    ///        public void Reset () {
    ///            state.Reset();
    ///        }
    ///    }
    ///    class MyGameMechanic : Monobehaviour, IResettable {
    ///        [InjectComponent] MyGameSystem someGameSystem;
    ///        IDurationEffect activeEffect;
    ///
    ///        // fire off an effect that lasts for some duration;
    ///        // replaces any previously active effects
    ///        void StartSomeEffect (float duration) {
    ///            // if we have an effect cancel it
    ///            activeEffect?.Cancel();
    ///
    ///            // start a new effect w/ some given duration
    ///            activeEffect = someGameSystem.ApplySpeedMultiplier(1.5f).WithDuration(duration);
    ///        }
    ///
    ///        // expose some ability info we can use to eg. draw / update UI elements or something
    ///        public bool active => activeEffect?.active ?? false;
    ///        public bool remainingTime => activeEffect?.remainingTime ?? 0f;
    ///
    ///        // clear + cancel active effects on reset
    ///        // (note that if you're just doing this to reset active effects when a system gets reset,
    ///        //  that is done automatically for you for all active effects if the system calls state.Reset().
    ///        //  however if eg. just this game mechanic / object needs to be reset (for some reason), then
    ///        //  we should make sure to cancel any effects we/re not using)
    ///        void Reset() {
    /// 
    ///            // cancel effect if active
    ///            activeEffect?.Cancel();
    ///
    ///            // clearing refs to unused effects is advised so that unused effects get GC-ed
    ///            activeEffect = null;
    ///        }
    ///    }
    ///    class MyOtherGameMechanic : Monobehavior, IResettable {
    ///        [InjectComponent] MyGameSystem someGameSystem;
    ///        IEffect reusedEffect;
    ///        IEffect effect => reusedEffect ?? (reusedEffect = someGameSystem.ApplySpeedMultiplier(1.2f));
    ///
    ///        void OnDestroy() { reusedEffect.Cancel(); reusedEffect = null; }
    ///        void SetActive (bool active) {
    ///            if (active == effect.active) return;
    ///            if (active) effect.Restart();
    ///            else        effect.Cancel();
    ///        }
    ///        void Reset() { effect.Reset(); }
    ///    }
    /// </example>
    public interface IEffect : IResettable {
        IEffectBehavior effectBehavior { get; set; }
        bool active { get; set; }
        bool finished { get; }
        
        /// <summary>
        /// Starts an effect (equivalent to setting active = true)
        /// No effect if the effect was finished (see <see cref="IEffect.Restart()"/> instead)
        /// </summary>
        /// <returns>
        ///    A reference to this object so an effect can be started and assigned in one line
        /// </returns>
        /// <example>
        ///    var effect = someSystem.DoSomeEffect().Start();
        /// </example>
        IEffect Start();

        /// <summary>
        /// Equivalent to calling Reset() followed by Start().
        /// </summary>
        void Restart();

        /// <summary>
        /// Cancels an effect.
        /// Has no effect if finished = true.
        /// finished = true will always be true after this call
        /// </summary>
        void Cancel();
        
        /// <summary>
        /// Sets this effect to have its 'active' state determined by a function or other
        /// delegate reference (ie. object method, etc), and returns a reference to self.
        /// </summary>
        IEffect WithActive(FunctionWithNoParametersReturning<bool> active);
        
        /// <summary>
        /// Sets this effect to have a fixed duration and returns a reference to an IEffect wrapper
        /// that gives you a similar interface but w/ a bunch of additional time properties and no
        /// further "With" constructors
        /// </summary>
        IDurationEffect WithDuration(float duration);
        
        /// <summary>
        /// Sets this effect to have a duration specified by a function / lambda / object method
        /// and returns a reference to an IEffect wrapper that exposes additional time properties
        /// </summary>
        IDurationEffect WithDuration(FunctionWithNoParametersReturning<float> duration);
        
        /// <summary>
        /// Sets this effect to have a duration specified by a custom IDuration class
        /// and returns a reference to an IEffect wrapper that exposes additional time properties
        /// Note that internally this is used to implement the above WithDuration() methods.
        /// </summary>
        IDurationEffect WithDuration(IDuration duration);
        
        /// <summary>
        /// Sets this effect to use an entirely custom IEffect behavior (controller)
        /// and returns a reference to self.
        /// Internally used to implement all of the above With() methods.
        /// </summary>
        IEffect WithCustomBehavior(IEffectBehavior behavior);
    }
    public interface IDurationEffect : IResettable {
        IDurationEffect Start();
        void Restart();
        void Cancel();
        bool active { get; set; }
        bool finished { get; }
        float duration { get; set; }
        float elapsedTime { get; }
        float remainingTime { get; }
    }
}