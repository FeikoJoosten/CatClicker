using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Method)]
public class EditorButton : PropertyAttribute
{
	public object[] parameters;

	public EditorButton(object[] parameters = null)
	{
		this.parameters = parameters;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
public class EditorButtonEditor : Editor {
    public override void OnInspectorGUI() {
        if (!target)
            return;

        base.OnInspectorGUI();
        
        MonoBehaviour mono = target as MonoBehaviour;

	    if (mono == null) return;

        IEnumerable<MemberInfo> methods = mono.GetType()
            .GetMembers(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                        BindingFlags.NonPublic)
            .Where(o => Attribute.IsDefined(o, typeof(EditorButton)));

        foreach (MemberInfo memberInfo in methods) {
	        if (!GUILayout.Button(memberInfo.Name)) continue;

			MethodInfo method = memberInfo as MethodInfo;
	        EditorButton[] editorButton = memberInfo.GetCustomAttributes(typeof(EditorButton), true) as EditorButton[];
	        if (method == null) continue;
	        if (editorButton == null) continue;
	        if (editorButton.Length == 0) continue;

	        method.Invoke(mono, editorButton[0].parameters);
        }
    }
}
#endif