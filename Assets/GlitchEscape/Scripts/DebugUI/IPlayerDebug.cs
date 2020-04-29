using UnityEngine;
namespace GlitchEscape.Scripts.DebugUI {
    public interface IPlayerDebug {
        void DrawDebugUI();
        string debugName { get; }
    }
}