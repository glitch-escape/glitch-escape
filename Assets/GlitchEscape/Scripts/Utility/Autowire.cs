using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class Autowire : MonoBehaviour {

    private static bool IsValidAutowireTarget(FieldInfo field) {
        var attribs = field.GetCustomAttributes(true);
        foreach (var attrib in attribs) {
            if (attrib is InjectComponent) return true;
        }
        return false;
    }
    private static bool IsUnityComponent(Type type) {
        var componentType = typeof(Component);
        while (type != null) {
            if (type == componentType) return true;
            type = type.BaseType;
        }
        return false;
    }

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
                info += "field type: " + field.FieldType + "\n";
                info += "is unity component? " + IsUnityComponent(field.FieldType) + "\n";
                info += "is injectable? " + IsValidAutowireTarget(field) + "\n";
                var attribs = field.GetCustomAttributes(true);
                foreach (var attrib in attribs) {
                    info += "field attrib: " + attrib + "\n";
                }
                info += "field value " + field.GetValue(component);
            }
        }
    }
}
