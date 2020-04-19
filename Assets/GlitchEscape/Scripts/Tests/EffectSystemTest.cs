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
            public Effect<EffectTest, State> IncreaseTestCount () {
                return state.CreateEffect(newState => ++newState.testEffectCount);
            }
            public Effect<EffectTest, State> SetFoo(string value) {
                return state.CreateEffect(newState => {
                    ++newState.testEffectCount;
                    newState.foo = value;
                });
            }
            public Effect<EffectTest, State> SetBar(int value) {
                return state.CreateEffect(newState => {
                    ++newState.testEffectCount;
                    newState.bar = value;
                });
            }
            public Effect<EffectTest, State> AddBar(int value) {
                return state.CreateEffect(newState => {
                    ++newState.testEffectCount;
                    newState.bar += value;
                });
            }
            public Effect<EffectTest, State> MulBar(int value) {
                return state.CreateEffect(newState => {
                    ++newState.testEffectCount;
                    newState.bar *= value;
                });
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
            Assert.AreEqual(false, effect.cancelled);
            Assert.AreEqual(true, effect.active);

            effect.Cancel();
            Assert.AreEqual(effect.cancelled, true);
        }

        [Test]
        public void TestEffectCancelUpdatesState() {
            var component = new EffectTest();
            component.Init();
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            var effect = component.SetFoo("foo");
            Assert.AreEqual(false, effect.cancelled);
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(1, (double)component.state.testEffectCount);
            Assert.AreEqual("foo", component.state.foo);

            effect.Cancel();
            Assert.AreEqual(effect.cancelled, true);
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
            Assert.AreEqual(false, effect.cancelled);
            Assert.AreEqual(true, effect.active);

            effect.active = false;
            Assert.AreEqual(false, effect.cancelled);
            Assert.AreEqual(false, effect.active);
        }
        
        [Test]
        public void TestReactivateEffectSetsEffectFlags() {
            var component = new EffectTest();
            component.Init();
            var effect = component.SetFoo("foo");
            effect.active = false;
            Assert.AreEqual(false, effect.cancelled);
            Assert.AreEqual(false, effect.active);

            effect.active = true;
            Assert.AreEqual(false, effect.cancelled);
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
            Assert.AreEqual(false, effect.cancelled);
            Assert.AreEqual(1, (double)component.state.testEffectCount);

            effect.Cancel();
            Assert.AreEqual(false, effect.active);
            Assert.AreEqual(true, effect.cancelled);
            Assert.AreEqual(0, (double)component.state.testEffectCount);
        }

        [Test]
        public void TestEffectsAreNotCancelledWhenSetInactive() {
            var component = new EffectTest();
            component.Init();
            var effect = component.SetFoo("fubar");
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(false, effect.cancelled);
            Assert.AreEqual(1, (double)component.state.testEffectCount);

            effect.active = false;
            Assert.AreEqual(false, effect.active);
            Assert.AreEqual(false, effect.cancelled);
            Assert.AreEqual(0, (double)component.state.testEffectCount);

            effect.active = true;
            Assert.AreEqual(true, effect.active);
            Assert.AreEqual(false, effect.cancelled);
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
            Assert.AreEqual(false, inactiveEffect.cancelled);
            Assert.AreEqual(1, (double) component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);
            
            inactiveEffect.Cancel();
            Assert.AreEqual(false, inactiveEffect.active);
            Assert.AreEqual(true, inactiveEffect.cancelled);
            Assert.AreEqual(1, (double) component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);

            inactiveEffect.active = true;
            Assert.AreEqual(false, inactiveEffect.active);
            Assert.AreEqual(true, inactiveEffect.cancelled);
            Assert.AreEqual(1, (double) component.state.testEffectCount);
            Assert.AreEqual("fubar", component.state.foo);
            
            activeEffect.Cancel();
            Assert.AreEqual(false, activeEffect.active);
            Assert.AreEqual(true, activeEffect.cancelled);
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
            
            Assert.AreEqual(false, test1.cancelled);
            Assert.AreEqual(false, test2.cancelled);
            Assert.AreEqual(false, setFubar.cancelled);
            Assert.AreEqual(false, set10.cancelled);
            Assert.AreEqual(false, setBar.cancelled);
            
            Assert.AreEqual(5, (double)component.state.testEffectCount);
            Assert.AreEqual(10, (double)component.state.bar);
            Assert.AreEqual("bar", component.state.foo);
            
            component.state.Reset();
            Assert.AreEqual(true, test1.cancelled);
            Assert.AreEqual(true, test2.cancelled);
            Assert.AreEqual(true, setFubar.cancelled);
            Assert.AreEqual(true, set10.cancelled);
            Assert.AreEqual(true, setBar.cancelled);
            
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
            Assert.AreEqual(false, eff3.cancelled);
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
            
            component.state.Reset();
            Assert.AreEqual(true, eff3.cancelled);
            Assert.AreEqual(0, (double)component.state.testEffectCount);
            Assert.AreEqual("", component.state.foo);
        }
    }
}
