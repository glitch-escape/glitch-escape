using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class Autowire : MonoBehaviour {
    public string info;
    public string log;
    public string warnings;
    public string errors;
    
    private static bool HasInjectableAttrib(FieldInfo field) {
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
    private bool IsInjectable(MonoBehaviour script, FieldInfo field) {
        if (!HasInjectableAttrib(field)) return false;
        if (!IsUnityComponent(field.FieldType)) {
            warnings += "[Injectable] field " + field + " on " + script + " is not a unity component!\n";
            return false;
        }
        return true;
    }
    private void InjectEmptyComponentFields(MonoBehaviour target) {
        var fields = target.GetType().GetFields();
        foreach (var field in fields) {
            if (IsInjectable(target, field)) {
                var value = field.GetValue(target);
                info += "found injectable field " + field + ", value " + value + ", is null? " + (value == null) + "\n";
                if (value != null) continue;
                info += "field is currently null\n";
                var type = field.FieldType;
                var addedComponent = false;
                var componentReference = 
                    type == typeof(Camera) ? Camera.main : 
                        target.GetComponentInChildren(type);
                if (componentReference == null) {
                    log += "Adding " + type + " component to " + target.gameObject + "\n";
                    componentReference = target.gameObject.AddComponent(type);
                    addedComponent = true;
                }
                log += "Setting missing " + type + " component reference for " + field + " in " + target + "\n";
                field.SetValue(target, componentReference);
                if (addedComponent && componentReference is MonoBehaviour) {
                    InjectEmptyComponentFields((MonoBehaviour) componentReference);
                }
            }
        }
    }
    
    public void Run() {
        info = "";
        log = "";
        warnings = "";
        errors = "";
        var components = GetComponents<MonoBehaviour>();
        foreach (var component in components) {
            info += "injecting fields on " + component + "\n";
            InjectEmptyComponentFields(component);
        }
    }
}
