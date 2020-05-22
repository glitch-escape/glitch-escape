using UnityEngine;

[CreateAssetMenu(fileName = "Fragment Particle Config", menuName = "Config/Particles/FragmentParticleConfig", order = 0)]
public class FragmentParticleVisualConfig : ScriptableObject {
    public Mesh[] meshes;
    public Material[] materials;
    public Vector3 scale = Vector3.one;
}