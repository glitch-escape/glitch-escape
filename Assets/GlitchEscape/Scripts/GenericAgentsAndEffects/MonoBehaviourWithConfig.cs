using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviourWithConfig<Config> : MonoBehaviour, IConfigurable<Config> 
    where Config: ScriptableObject 
{
    [SerializeField]
    public Config _config;
    public Config config {
        get { return _config; }
        set { _config = value; }
    }
}
