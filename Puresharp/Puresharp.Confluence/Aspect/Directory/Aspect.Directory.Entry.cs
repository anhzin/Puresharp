using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Puresharp.Confluence
{
    abstract public partial class Aspect
    {
        static private partial class Directory
        {
            private sealed partial class Entry : IEnumerable<Aspect>
            {
                static private string Identity(Type type)
                {
                    var _index = type.Name.IndexOf('`');
                    var _name = _index < 0 ? type.Name : type.Name.Substring(0, _index);
                    if (type.GetGenericArguments().Length == 0) { return string.Concat("<", _name, ">"); }
                    _name = string.Concat(_name, "<", type.GetGenericArguments().Length.ToString(), ">");
                    return string.Concat("<", _name, string.Concat("<", string.Concat(type.GetGenericArguments().Select(_argument => string.Concat("<", _argument.Name, ">"))), ">"), ">");
                }

                static private string Identity(MethodBase method)
                {
                    return string.Concat("<", method.IsConstructor ? method.DeclaringType.Name : method.Name, method.GetGenericArguments().Length > 0 ? string.Concat("<", method.GetGenericArguments().Length, ">") : string.Empty, method.GetParameters().Length > 0 ? string.Concat("<", string.Concat(method.GetParameters().Select(_parameter => Identity(_parameter.ParameterType))), ">") : string.Empty, ">");
                }

                static private FieldInfo Pointer(MethodBase method)
                {
                    foreach (var _instruction in method.Body())
                    {
                        if (_instruction.Code == OpCodes.Ldsfld)
                        {
                            var _field = _instruction.Value as FieldInfo;
                            if (_field.Name == "<Pointer>") { return _field; }
                        }
                    }
                    throw new NotSupportedException(string.Format($"type '{ method.DeclaringType.AssemblyQualifiedName }' is not managed by CNeptune and cannot be supervised."));
                }

                public readonly Type Type;
                public readonly MethodBase Method;
                public readonly Activity Activity;
                private readonly LinkedList<Aspect> m_Aspectization;
                private readonly LinkedList<MethodInfo> m_Sequence;
                private readonly Dictionary<Aspect, Activity> m_Dictionary;
                private readonly IntPtr m_Pointer;
                private readonly FieldInfo m_Field;
                private readonly FieldInfo m_Boundary;

                internal Entry(Type type, MethodBase method, Activity activity)
                {
                    this.Type = type;
                    this.Method = method;
                    this.Activity = activity;
                    this.m_Aspectization = new LinkedList<Aspect>();
                    this.m_Dictionary = new Dictionary<Aspect, Activity>();
                    this.m_Field = Aspect.Directory.Entry.Pointer(method);
                    this.m_Pointer = (IntPtr)this.m_Field.GetValue(null);
                    this.m_Sequence = new LinkedList<MethodInfo>();
                    var _attribute = method.Attribute<AsyncStateMachineAttribute>();
                    if (_attribute == null) { return; }
                    this.m_Boundary = _attribute.StateMachineType.GetField("<Factory>", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                }

                private void Update()
                {
                    var _aspectization = this.m_Aspectization.SelectMany(_Aspect => _Aspect.Advise(this.Method)).ToArray();
                    if (this.m_Boundary == null)
                    {
                        var _pointer = this.m_Pointer;
                        this.m_Sequence.Clear();
                        foreach (var _advice in _aspectization)
                        {
                            if (_advice == null) { continue; }
                            var _method = _advice.Decorate(this.Method, _pointer);
                            this.m_Sequence.AddLast(_method);
                            if (_method != null) { _pointer = _method.GetFunctionPointer(); }
                        }
                        this.m_Field.SetValue(null, _pointer);
                    }
                    else
                    {
                        this.m_Sequence.Clear();
                        var _boundary = this.m_Boundary.GetValue(null) as Advice.Boundary.IFactory;
                        foreach (var _advice in _aspectization)
                        {
                            if (_advice == null) { continue; }
                            _boundary = _advice.Decorate(this.Method, _boundary);
                        }
                        this.m_Field.SetValue(null, _boundary);
                    }
                }

                public void Add(Aspect aspect)
                {
                    if (this.m_Dictionary.ContainsKey(aspect))
                    {
                        this.Update();
                        return;
                    }
                    this.m_Aspectization.AddFirst(aspect);
                    this.m_Dictionary.Add(aspect, null);
                    this.Update();
                }

                public void Remove(Aspect aspect)
                {
                    if (this.m_Dictionary.Remove(aspect))
                    {
                        this.m_Aspectization.Remove(aspect);
                        this.Update();
                    }
                }

                IEnumerator<Aspect> IEnumerable<Aspect>.GetEnumerator()
                {
                    return this.m_Aspectization.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.m_Aspectization.GetEnumerator();
                }
            }

            [DebuggerDisplay("{Debugger.Display(this) , nq}")]
            [DebuggerTypeProxy(typeof(Entry.Debugger))]
            private sealed partial class Entry
            {
                private class Debugger
                {
                    static public string Display(Aspect.Directory.Entry map)
                    {
                        return string.Concat(map.Type.Declaration(), ".", map.Method.Declaration(), " = ", map.m_Aspectization.Count.ToString());
                    }

                    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                    private Aspect.Directory.Entry m_Map;

                    public Debugger(Aspect.Directory.Entry map)
                    {
                        this.m_Map = map;
                    }

                    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                    public Aspect[] View
                    {
                        get { return this.m_Map.m_Aspectization.ToArray(); }
                    }
                }
            }
        }
    }
}