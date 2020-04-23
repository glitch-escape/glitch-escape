using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GlitchEscape.Effects;

namespace Tests
{
    public class EffectSystemTest
    {
        public class EffectTest {
            public class Config {
                public int bar = 12;
            }
            public class State : EffectState<EffectTest, State> {
                public int testEffectCount = -1;
                public string foo = "asdf";
                public int bar = 10;
                public State(EffectTest owner) : base(owner) { }
                protected override void SetDefaults(EffectTest user) {
                    testEffectCount = 0;
                    foo = "";
                    bar = user.config.bar;
                }
            }
            public Config config;
            public State state;
            public void Init() {
                config = new Config();
                state = new State(this);
            }

            struct IncreaseTestCountEffect : IEffector<EffectTest, State> {
                public void Apply(State state) {
                    ++state.testEffectCount;
                    Debug.Log("Increased test count to "+state.testEffectCount);
                }
            }
            struct SetFooEffect : IEffector<EffectTest, State> {
                public string value;
                public void Apply(State state) { ++state.testEffectCount; state.foo = value; }
            }
            struct SetBarEffect : IEffector<EffectTest, State> {
                public int value;
                public void Apply(State state) { ++state.testEffectCount; state.bar = value; }
            }
            struct AddBarEffect : IEffector<EffectTest, State> {
                public int value;
                public void Apply(State state) { ++state.testEffectCount; state.bar += value; }
            }
            public IEffect IncreaseTestCount () {
                return state.CreateEffect(new IncreaseTestCountEffect());
            }
            public IEffect SetFoo(string value) {
                return state.CreateEffect(new SetFooEffect { value = value });
            }
            public IEffect SetBar(int value) {
                return state.CreateEffect(new SetBarEffect { value = value });
            }
            public IEffect AddBar(int value) {
                return state.CreateEffect(new AddBarEffect { value = value });
            }
        }
        
        class MockEffectBehavior : IEffectBehavior {
            public bool wasActiveSetTrue = false;
            public bool wasActiveSetFalse  = false;
            public bool wasActiveSet => wasActiveSetTrue || wasActiveSetFalse;
            public bool wasCancelled = false;
            public bool wasReset = false;
            public int updateCount = 0;
            public int operationCount = 0;
            public bool isActive   { get; set; }
            public bool isFinished { get; set; }
            public bool defaultValueOnReset { get; set; } = false;

            public MockEffectBehavior(bool active, bool finished = false) {
                isActive = active;
                isFinished = finished;
            }
            public bool active {
                get => isActive;
                set {
                    if (value) wasActiveSetTrue = true;
                    else wasActiveSetFalse = true;
                    ++operationCount;
                    isActive = value;
                }
            }
            public bool finished => isFinished;
            public void OnCancelled() {
                wasCancelled = true;
                ++operationCount;
            }
            public void Reset() {
                wasReset = true;
                isActive = defaultValueOnReset;
                ++operationCount;
            }
            public void Update() {
                ++updateCount;
                ++operationCount;
            }
        }
        
        [Test]
        public void TestEffectStateInitialization() {
            var component = new EffectTest();
            Assert.IsNull(component.config);
            Assert.IsNull(component.state);
            component.Init();
            
            // test that we've initialized config + state correctly
            Assert.NotNull(component.config);
            Assert.NotNull(component.state);
            Assert.AreEqual(12, (double)component.config.bar);
            
            // test that values match the results in SetDefaults(), NOT their class default values
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            Assert.AreEqual((double)component.config.bar, (double)component.state.bar);
        }

        [Test]
        public void TestNoEffectBehaviorFunctionsAreCalledNewEffect() {
            var test = new EffectTest();
            var behavior = new MockEffectBehavior(true, false);
            var effect = test.SetFoo("fubar").WithCustomBehavior(behavior);
            Assert.AreEqual(0, behavior.operationCount);
            Assert.IsFalse(behavior.wasActiveSet);
            Assert.IsFalse(behavior.wasCancelled);
            Assert.IsFalse(behavior.wasReset);
            Assert.AreEqual(0, behavior.updateCount);
        }
        
        [Test]
        public void TestEffectBehaviorThatStartsInactive() {
            TestEffectBehavior(false, false);
        }
        
        [Test]
        public void TestEffectBehaviorThatStartsActive() {
            TestEffectBehavior(true, false);
        }
        
        [Test]
        public void TestEffectBehaviorThatStartsFinishedAndInactive() {
            TestEffectBehavior(false, true);
        }
        
        [Test]
        public void TestEffectBehaviorThatStartsFinishedAndActive() {
            TestEffectBehavior(false, true);
        }
        private void TestEffectBehavior(bool startsActive, bool startsFinished) {
            var test = new EffectTest();
            test.Init();
            var behavior = new MockEffectBehavior(startsActive, startsFinished);
            var effect = test.SetFoo("fubar").WithCustomBehavior(behavior);

            var expectActive = startsActive;
            var expectFinished = startsFinished;

            // check initial values
            Assert.AreEqual(0, behavior.operationCount);
            Assert.AreEqual(false, behavior.wasActiveSet);
            Assert.AreEqual(false, behavior.wasCancelled);
            Assert.AreEqual(false, behavior.wasReset);
            Assert.AreEqual(0, behavior.updateCount);
            Assert.AreEqual(expectFinished, effect.finished);
            Assert.AreEqual(expectActive, effect.active);

            // test set active = true
            effect.active = true;
            expectActive = true && !expectFinished;
            Assert.AreEqual(expectActive, effect.active);
            Assert.AreEqual(expectFinished, effect.finished);
            Assert.AreEqual(1, behavior.operationCount);
            Assert.AreEqual(true, behavior.wasActiveSetTrue);
            Assert.AreEqual(false, behavior.wasActiveSetFalse);
            behavior.wasActiveSetTrue = false;
            
            // test set active = false
            effect.active = false;
            expectActive = false;
            Assert.AreEqual(expectActive, effect.active);
            Assert.AreEqual(expectFinished, effect.finished);
            Assert.AreEqual(2, behavior.operationCount);
            Assert.AreEqual(false, behavior.wasActiveSetTrue);
            Assert.AreEqual(true, behavior.wasActiveSetFalse);
            behavior.wasActiveSetFalse = false;
            Assert.AreEqual(false, behavior.wasActiveSet);
            
            // test Cancel()
            effect.Cancel();
            Assert.AreEqual(3, behavior.operationCount);
            Assert.AreEqual(true, behavior.wasCancelled);
            Assert.AreEqual(false, behavior.wasActiveSet);
            Assert.AreEqual(false, effect.active);
            Assert.AreEqual(true, effect.finished);
            
            // test Cancel() again
            effect.Cancel();
            Assert.AreEqual(3, behavior.operationCount);
            Assert.AreEqual(false, behavior.wasCancelled);
            Assert.AreEqual(false, behavior.wasActiveSet);
            Assert.AreEqual(false, effect.active);
            Assert.AreEqual(true, effect.finished);
            
            // test Start() (should do nothing since finished)
            effect.Start();
            Assert.AreEqual(3, behavior.operationCount);
            Assert.AreEqual(false, behavior.wasActiveSet);
            
            // test Reset() w/ reset-to-active true
            effect.Reset();
            behavior.defaultValueOnReset = true;
            Assert.AreEqual(4, behavior.operationCount);
            Assert.AreEqual(true, behavior.wasReset);
            Assert.AreEqual(behavior.defaultValueOnReset, effect.active);
            Assert.AreEqual(false, effect.finished);
            behavior.wasReset = false;
            
            // test Reset() w/ reset-to-active false
            effect.Reset();
            behavior.defaultValueOnReset = false;
            Assert.AreEqual(5, behavior.operationCount);
            Assert.AreEqual(true, behavior.wasReset);
            Assert.AreEqual(behavior.defaultValueOnReset, effect.active);
            Assert.AreEqual(false, effect.finished);
            behavior.wasReset = false;

            // test Start() (when not finished; should start effect)
            behavior.wasActiveSetTrue = behavior.wasActiveSetFalse = false;
            Assert.AreEqual(false, behavior.wasActiveSet);
            effect.Start();
            Assert.AreEqual(6, behavior.operationCount);
            Assert.AreEqual(true, behavior.wasActiveSet);
            Assert.AreEqual(true, behavior.wasActiveSetTrue);
            Assert.AreEqual(false, behavior.wasActiveSetFalse);
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(false, effect.finished);
            
            // test Start() again (should have no effect)
            behavior.wasActiveSetTrue = behavior.wasActiveSetFalse = false;
            Assert.AreEqual(false, behavior.wasActiveSet);
            effect.Start();
            Assert.AreEqual(7, behavior.operationCount);
            Assert.AreEqual(false, behavior.wasActiveSet);
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(false, effect.finished);

            // test Restart() w/ reset-to-active true (while started)
            behavior.defaultValueOnReset = true;
            behavior.wasReset = false;
            behavior.wasActiveSetTrue = behavior.wasActiveSetFalse = false;
            effect.Restart();
            Assert.AreEqual(8, behavior.operationCount);
            Assert.AreEqual(true, behavior.wasReset);
            Assert.AreEqual(false, behavior.wasActiveSet);
            Assert.AreEqual(behavior.defaultValueOnReset, effect.active);
            Assert.AreEqual(false, effect.finished);
            
            // test Restart() w/ reset-to-active false (while started)
            behavior.defaultValueOnReset = true;
            behavior.wasReset = false;
            behavior.wasActiveSetTrue = behavior.wasActiveSetFalse = false;
            effect.Restart();
            Assert.AreEqual(8, behavior.operationCount);
            Assert.AreEqual(true, behavior.wasReset);
            Assert.AreEqual(false, behavior.wasActiveSet);
            Assert.AreEqual(behavior.defaultValueOnReset, effect.active);
            Assert.AreEqual(false, effect.finished);
        }
        
        

        [Test]
        public void TestEffectApplication() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);

            component.IncreaseTestCount();
            Assert.AreEqual(1, (double)component.state.testEffectCount);

            component.IncreaseTestCount();
            Assert.AreEqual(2, (double)component.state.testEffectCount);

            component.SetFoo("fubar");
            Assert.AreEqual(3, (double)component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);
            
            component.SetBar(10);
            Assert.AreEqual(4, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);

            component.SetFoo("bar");
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual("bar", component.state.foo);
        }

        [Test]
        public void TestEffectResetMultiple() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);

            component.IncreaseTestCount();
            component.IncreaseTestCount();
            component.SetFoo("fubar");
            component.SetBar(10);
            component.SetFoo("bar");
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);

            component.state.Reset();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual((double)component.config.bar, (double)component.state.bar);
            Assert.AreEqual("", component.state.foo);
            
            component.SetFoo("fubar");
            component.SetBar(10);
            Assert.AreEqual(2, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("fubar", component.state.foo);
            
            component.state.Reset();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual((double)component.config.bar, (double)component.state.bar);
            Assert.AreEqual("", component.state.foo);
        }

        [Test]
        public void TestEffectsAreAutomaticallySetActive() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            
            var test1 = component.IncreaseTestCount();
            var test2 = component.IncreaseTestCount();
            var setFubar = component.SetFoo("fubar");
            var set10 = component.SetBar(10);
            var setBar = component.SetFoo("bar");
            Assert.AreEqual(true, test1.active);
            Assert.AreEqual(true, test2.active);
            Assert.AreEqual(true, setFubar.active);
            Assert.AreEqual(true, set10.active);
            Assert.AreEqual(true, setBar.active);
        }
        
        [Test]
        public void TestEffectCancelSetsEffectFlags() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            var effect = component.SetFoo("foo");
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(true, effect.active);

            effect.Cancel();
            Assert.AreEqual(effect.finished, true);
        }

        [Test]
        public void TestEffectCancelUpdatesState() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            var effect = component.SetFoo("foo");
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual("foo", component.state.foo);

            effect.Cancel();
            Assert.AreEqual(true, effect.finished);
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
        }

        [Test]
        public void TestInactivateEffectSetsEffectFlags() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            var effect = component.SetFoo("foo");
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(true, effect.active);

            effect.active = false;
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(false, effect.active);
        }
        
        [Test]
        public void TestReactivateEffectSetsEffectFlags() {
            var component = new EffectTest();
            component.Init();
            var effect = component.SetFoo("foo");
            effect.active = false;
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(false, effect.active);

            effect.active = true;
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(true, effect.active);
        }
        
        [Test]
        public void TestReactivateEffectUpdatesState() {
            var component = new EffectTest();
            component.Init();
            var effect = component.SetFoo("foo");
            effect.active = false;
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            effect.active = true;
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual("foo", component.state.foo);
        }
        
        [Test]
        public void TestInactivateEffectUpdatesState() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            var effect = component.SetFoo("foo");
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual("foo", component.state.foo);

            effect.active = false;
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
        }
        

        [Test]
        public void TestEffectsAreSetInactiveWhenCancelled() {
            var component = new EffectTest();
            component.Init();
            var effect = component.SetFoo("fubar");
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(1, (double)component.state.testEffectCount);

            effect.Cancel();
            Assert.AreEqual(false, effect.active);
            Assert.AreEqual(true, effect.finished);
            Assert.AreEqual(0, (double)component.state.testEffectCount);
        }
        
        [Test]
        public void TestEffectsAreNotCancelledWhenSetInactive() {
            var component = new EffectTest();
            component.Init();
            var effect = component.SetFoo("fubar");
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(1, (double)component.state.testEffectCount);

            effect.active = false;
            Assert.AreEqual(false, effect.active);
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(0, (double)component.state.testEffectCount);

            effect.active = true;
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(false, effect.finished);
            Assert.AreEqual(1, (double)component.state.testEffectCount);
        }
        
        [Test]
        public void TestEffectsCanBeCancelledWhenActiveOrInactive() {
            var component = new EffectTest();
            component.Init();
            var activeEffect = component.SetFoo("fubar");
            var inactiveEffect = component.SetFoo("bar");
            Assert.AreEqual(2, (double) component.state.testEffectCount);
            Assert.AreEqual("bar", component.state.foo);

            inactiveEffect.active = false;
            Assert.AreEqual(false, inactiveEffect.active);
            Assert.AreEqual(false, inactiveEffect.finished);
            Assert.AreEqual(1, (double) component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);
            
            inactiveEffect.Cancel();
            Assert.AreEqual(false, inactiveEffect.active);
            Assert.AreEqual(true, inactiveEffect.finished);
            Assert.AreEqual(1, (double) component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);

            inactiveEffect.active = true;
            Assert.AreEqual(false, inactiveEffect.active);
            Assert.AreEqual(true, inactiveEffect.finished);
            Assert.AreEqual(1, (double) component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);
            
            activeEffect.Cancel();
            Assert.AreEqual(false, activeEffect.active);
            Assert.AreEqual(true, activeEffect.finished);
            Assert.AreEqual(0, (double) component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
        }

        [Test]
        public void TestMultipleEffectsAreCancelledByReset() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            var test1 = component.IncreaseTestCount();
            var test2 = component.IncreaseTestCount();
            var setFubar = component.SetFoo("fubar");
            var set10 = component.SetBar(10);
            var setBar = component.SetFoo("bar");
            
            Assert.AreEqual(true, test1.active);
            Assert.AreEqual(true, test2.active);
            Assert.AreEqual(true, setFubar.active);
            Assert.AreEqual(true, set10.active);
            Assert.AreEqual(true, setBar.active);
            
            Assert.AreEqual(false, test1.finished);
            Assert.AreEqual(false, test2.finished);
            Assert.AreEqual(false, setFubar.finished);
            Assert.AreEqual(false, set10.finished);
            Assert.AreEqual(false, setBar.finished);
            
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);
            
            component.state.Reset();
            Assert.AreEqual(true, test1.finished);
            Assert.AreEqual(true, test2.finished);
            Assert.AreEqual(true, setFubar.finished);
            Assert.AreEqual(true, set10.finished);
            Assert.AreEqual(true, setBar.finished);
            
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
        }
        
        [Test]
        public void TestInterleavedEffectCancel() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            
            var test1 = component.IncreaseTestCount();
            var test2 = component.IncreaseTestCount();
            var setFubar = component.SetFoo("fubar");
            var set10 = component.SetBar(10);
            var setBar = component.SetFoo("bar");
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);

            test1.Cancel();
            Assert.AreEqual(4, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);
            
            test1.Cancel();
            Assert.AreEqual(4, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);

            var setBaz = component.SetFoo("baz");
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("baz", component.state.foo);
            
            set10.Cancel();
            Assert.AreEqual(4, (double)component.state.testEffectCount);
            Assert.AreEqual((double)component.config.bar, (double)component.state.bar);
            Assert.AreEqual("baz", component.state.foo);
            
            setBaz.Cancel();
            Assert.AreEqual(3, (double)component.state.testEffectCount);
            Assert.AreEqual((double)component.config.bar, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);
            
            setFubar.Cancel();
            Assert.AreEqual(2, (double)component.state.testEffectCount);
            Assert.AreEqual((double)component.config.bar, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);
            
            setBar.Cancel();
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual((double)component.config.bar, (double)component.state.bar);
            Assert.AreEqual("", component.state.foo);

            component.state.Reset();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual((double)component.config.bar, (double)component.state.bar);
            Assert.AreEqual("", component.state.foo);
        }

        [Test]
        public void TestinterleavedEffectSetActive() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            
            var test1 = component.IncreaseTestCount();
            var test2 = component.IncreaseTestCount();
            var setFubar = component.SetFoo("fubar");
            var set10 = component.SetBar(10);
            var setBar = component.SetFoo("bar");
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);
            
            var setBaz = component.SetFoo("baz");
            Assert.AreEqual(6, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("baz", component.state.foo);

            setBaz.active = false;
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);
            
            setBar.active = false;
            Assert.AreEqual(4, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("fubar", component.state.foo);
            
            setBaz.active = true;
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("baz", component.state.foo);

            setFubar.active = false;
            Assert.AreEqual(4, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("baz", component.state.foo);
            
            setBaz.active = false;
            Assert.AreEqual(3, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("", component.state.foo);
            
            component.state.Reset();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);

            var eff1 = component.SetFoo("fubar");
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);

            var eff2 = component.SetFoo("bar");
            Assert.AreEqual(2, (double)component.state.testEffectCount);
            Assert.AreEqual("bar", component.state.foo);

            eff2.active = false;
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);

            eff1.Cancel();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);

            eff1.active = true;
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);

            eff2.active = true;
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual("bar", component.state.foo);

            eff2.active = false;
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            eff2.Cancel();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);

            var eff3 = component.SetFoo("foo");
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual("foo", component.state.foo);

            eff3.active = false;
            Assert.AreEqual(false, eff3.finished);
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            component.state.Reset();
            Assert.AreEqual(true, eff3.finished);
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
        }
    }
}
