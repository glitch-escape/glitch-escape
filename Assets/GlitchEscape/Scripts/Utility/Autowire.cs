using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Autowire : MonoBehaviour {

    public string info;
    public void Run() {
        info = "";
        var components = GetComponents<MonoBehaviour>();
        foreach (var component in components) {
            var type = component.GetType();
            // var attribs = System.Attribute.GetCustomAttributes(type.Module.Assembly, type);
            info += "" + component + " (type " + type + "):\n";

            var fields = type.GetFields();
            foreach (var field in fields) {
                info += "field " + field + "\n";
                var attribs = field.GetCustomAttributes(true);
                foreach (var attrib in attribs) {
                    info += "field attrib: " + attrib + "\n";
                }
                info += "field value " + field.GetValue(component);
            }
            // foreach (var attrib in attribs) {
            //     info += "" + attrib + "\n";
            // }
        }
    }
}
