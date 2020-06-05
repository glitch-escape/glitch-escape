using UnityEngine;

namespace GlitchEscape.Scripts.Utility {
    /// <summary>
    /// Stores floating point samples + calculates a running average of submitted samples
    /// used to implement <see cref="FramerateSampler"/>
    /// </summary>
    public class RingBufferAverageSampler {
        private float[] samples;
        private uint  numSamples = 0;
        private float currentSampleTotal = 0f;
        private uint  nextSampleIndex = 0;

        public RingBufferAverageSampler(uint numSamples = 64) {
            this.samples = new float[numSamples];
        }
        public void AddSample(float sample) {
            if (numSamples >= samples.Length) {
                currentSampleTotal -= samples[nextSampleIndex];
            } else {
                ++numSamples;
            }
            currentSampleTotal += samples[nextSampleIndex] = sample;
            nextSampleIndex = (nextSampleIndex + 1) % ((uint) samples.Length);
        }
        public float average => currentSampleTotal / numSamples;
    }
    
    public class FramerateSampler {
        private RingBufferAverageSampler dtSamples { get; } = new RingBufferAverageSampler(64);

        /// <summary>
        /// Should be called every frame
        /// </summary>
        public void Update() {
            dtSamples.AddSample(Time.unscaledDeltaTime);
        }
        public float framerate => 1f / dtSamples.average;
    }
}