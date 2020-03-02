using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enforcements {
    public static T GetSingleComponentInScene<T>(MonoBehaviour script, out T result) where T : Object {
        return result = GetSingleComponentInScene<T>(script);
    }
    public static T[] GetAllComponentsInScene<T>(MonoBehaviour script, out T[] result) where T : Object {
        return result = GetAllComponentsInScene<T>(script);
    }
    public static T GetComponent<T>(MonoBehaviour script, out T result) {
        return result = GetComponent<T>(script);
    }
    public static T[] GetComponents<T>(MonoBehaviour script, out T[] result) {
        return result = GetComponents<T>(script);
    }
    public static T GetComponentInParent<T>(MonoBehaviour script, out T result) {
        return result = GetComponentInParent<T>(script);
    }
    public static T[] GetComponentsInParent<T>(MonoBehaviour script, out T[] result) {
        return result = GetComponentsInParent<T>(script);
    }
    public static T GetComponentInChildren<T>(MonoBehaviour script, out T result) {
        return result = GetComponentInChildren<T>(script);
    }
    public static T[] GetComponentsInChildren<T>(MonoBehaviour script, out T[] result) {
        return result = GetComponentsInChildren<T>(script);
    }
    public static T GetSingleComponentInScene<T> (MonoBehaviour script) where T: Object {
        var component = GameObject.FindObjectOfType<T>();
        if (component == null) {
            Debug.LogError(
                script.name + " on " + script.gameObject.name
                + " no component in this scene! " + typeof(T) + "!");
        }
        return component;
    }
    public static T GetSingleComponentInScene<T> () where T: Object {
        var component = GameObject.FindObjectOfType<T>();
        if (component == null) {
            Debug.LogError(" no component in this scene! " + typeof(T) + "!");
        }
        return component;
    }
    public static T[] GetAllComponentsInScene<T>(MonoBehaviour script) where T: Object{
        var component = GameObject.FindObjectsOfType<T>();
        if (component == null  || component.Length == 0) {
            Debug.LogError(
                script.name + " on " + script.gameObject.name
                + " no component in this scene! " + typeof(T) + "!");
        }
        return component;
    }
    public static T GetComponent<T>(MonoBehaviour script) {
        var component = script.GetComponent<T>();
        if (component == null) {
            Debug.LogError(
                script.name + " on " + script.gameObject.name
                + " missing component: " + typeof(T) + "!");
        }
        return component;
    }
    public static T[] GetComponents<T>(MonoBehaviour script) {
        var component = script.GetComponents<T>();
        if (component == null || component.Length == 0) {
            Debug.LogError(
                script.name + " on " + script.gameObject.name
                + " needs at least one " + typeof(T) + " component!");
        }
        return component;
    }
    public static T GetComponentInParent<T>(MonoBehaviour script) {
        var component = script.GetComponentInParent<T>();
        if (component == null) {
            Debug.LogError(
                script.name + " on " + script.gameObject.name
                + " missing component: " + typeof(T) + "!");
        }
        return component;
    }
    public static T[] GetComponentsInParent<T>(MonoBehaviour script) {
        var component = script.GetComponentsInParent<T>();
        if (component == null || component.Length == 0) {
            Debug.LogError(
                script.name + " on " + script.gameObject.name
                + " missing component: " + typeof(T) + "!");
        }
        return component;
    }
    public static T GetComponentInChildren<T>(MonoBehaviour script) {
        var component = script.GetComponentInChildren<T>();
        if (component == null) {
            Debug.LogError(
                script.name + " on " + script.gameObject.name
                + " missing component: " + typeof(T) + "!");
        }
        return component;
    }
    public static T[] GetComponentsInChildren<T>(MonoBehaviour script) {
        var component = script.GetComponentsInChildren<T>();
        if (component == null || component.Length == 0) {
            Debug.LogError(
                script.name + " on " + script.gameObject.name
                + " missing component: " + typeof(T) + "!");
        }

        return component;
    }
}