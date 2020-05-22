using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogConfig", menuName = "GameConfigs/DialogConfig", order = 1)]
public class DialogConfig : ScriptableObject {
    [Header("Yarn File for Dialog")] 
    public YarnProgram coreText;

    [Header("Text Speed Control")]
    public float sentenceDelay = 3;
    public float textSpeed = 0.025f;
    public bool isCutscene;

    [Header("Character Portraits")]
    public Portrait[] portraits;
    [System.Serializable]
    public class Portrait {
        public string name;
        public Sprite icon;
    }
}
