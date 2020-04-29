
using UnityEditor.UIElements;
using UnityEngine;

[System.Serializable]
public struct FloatRange {
    public float minimum;
    public float maximum;
    public float range => maximum - minimum;
    public float Lerp(float t) {
        return Mathf.Lerp(minimum, maximum, t);
    }
    public float InverseLerp(float sample) {
        return Mathf.Clamp01((sample - minimum) / range);
    }
    public float SampleCurve(float sample, AnimationCurve curve) {
        return curve.Evaluate(InverseLerp(sample));
    }
}
