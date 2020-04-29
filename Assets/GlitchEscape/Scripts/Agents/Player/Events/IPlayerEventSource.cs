using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerEventSource {
    event PlayerEvent.Event OnEvent;
}

