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
        
        
        
        // A Test behaves as an ordinary method
        [Test]
        public void EffectSystemTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator EffectSystemTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
