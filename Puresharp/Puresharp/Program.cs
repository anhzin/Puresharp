﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Puresharp;
using Puresharp.Confluence;
using Puresharp.Discovery;

using Assembly = System.Reflection.Assembly;
using MethodBase = System.Reflection.MethodBase;
using MethodInfo = System.Reflection.MethodInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace Puresharp
{
    static public class Program
    {
        private const string Puresharp = "<Puresharp>";
        public const string Module = "<Module>";

        static private readonly MethodInfo GetMethodHandle = Metadata<MethodBase>.Property(_Method => _Method.MethodHandle).GetGetMethod();
        static private readonly MethodInfo GetFunctionPointer = Metadata<RuntimeMethodHandle>.Method(_Method => _Method.GetFunctionPointer());
        static private readonly MethodInfo CreateDelegate = Discovery.Metadata.Method(() => Delegate.CreateDelegate(Argument<Type>.Value, Argument<MethodInfo>.Value));
        static private readonly List<AssemblyDefinition> m_Inclusion = new List<AssemblyDefinition>();

        static public void Main(string[] arguments)
        {
            if (arguments == null) { throw new ArgumentNullException(); }
            switch (arguments.Length)
            {
                case 1:
                    Program.Manage(arguments[0]);
                    break;
                case 2:
                    var _directory = string.Concat(Path.GetDirectoryName(arguments[0]), @"\");
                    var _document = XDocument.Load(arguments[0]);
                    var _namespace = _document.Root.Name.Namespace;
                    var _name = _document.Descendants(_namespace.GetName("AssemblyName")).Single().Value;
                    var _type = _document.Descendants(_namespace.GetName("OutputType")).SingleOrDefault();
                    foreach (var _element in _document.Descendants(_namespace.GetName("OutputPath")))
                    {
                        foreach (var _attribute in _element.Parent.Attributes())
                        {
                            if (_attribute.Value.Contains(arguments[1]))
                            {
                                switch (_type == null ? "Library" : _type.Value)
                                {
                                    case "Library": Program.Manage(string.Concat(_directory, _element.Value, _name, ".dll")); return;
                                    case "WinExe":
                                    case "Exe": Program.Manage(string.Concat(_directory, _element.Value, _name, ".exe")); return;
                                    default: throw new NotSupportedException($"Unknown OutputType: {_type.Value}");
                                }
                            }
                        }
                    }
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        static private void Manage(string assembly)
        {
            var _resolver = new DefaultAssemblyResolver();
            _resolver.AddSearchDirectory(Path.GetDirectoryName(assembly));
            var _assembly = AssemblyDefinition.ReadAssembly(assembly, new ReaderParameters() { AssemblyResolver = _resolver, ReadSymbols = true, ReadingMode = ReadingMode.Immediate });
            var _name = typeof(Aspect).Assembly.GetName().Name;
            var _version = typeof(Aspect).Assembly.GetName().Version.Major.ToString();
            _assembly.Attribute(() => new System.Reflection.AssemblyMetadataAttribute(_name, _version));
            var _module = _assembly.MainModule;
            foreach (var _type in _module.GetTypes().ToArray()) { Program.Manage(_type); }
            _assembly.Write(assembly, new WriterParameters { WriteSymbols = true });
        }

        static private bool Bypass(TypeDefinition type)
        {
            return type.IsInterface || type.IsValueType || type.Name == Program.Module || (type.BaseType != null && type.BaseType.Resolve() == type.Module.Import(typeof(MulticastDelegate)).Resolve() || type.Interfaces.Any(_Interface => _Interface.Resolve() == type.Module.Import(typeof(IAsyncStateMachine)).Resolve()));
        }

        static private bool Bypass(MethodDefinition method)
        {
            return method.Body == null || (method.IsConstructor && method.IsStatic);
        }

        static private void Manage(TypeDefinition type)
        {
            if (Program.Bypass(type)) { return; }
            foreach (var _method in type.Methods.ToArray())
            {
                if (Program.Bypass(_method)) { continue; }
                Program.Manage(_method);
            }
        }

        //static private void Confluence(this AssemblyDefinition assembly)
        //{
        //    if (Program.m_Lifecycles.Contains(assembly)) { return; }
        //    var _module = assembly.MainModule.Types.First(_Type => _Type.Name == Program.Module);
        //    var _lazy = _module.Field<Lazy<Assembly>>("<Puresharp.Confluence>", FieldAttributes.Static | FieldAttributes.Private);
        //    Program.m_Lifecycles.Add(assembly);
        //    var _make = _module.Method<Assembly>("<Puresharp.Confluence<Fake.Make>>", MethodAttributes.Static | MethodAttributes.Private);
        //    _make.Body.Emit(OpCodes.Ldstr, Convert.ToBase64String(File.ReadAllBytes(typeof(Advice.IBoundary).Assembly.Location)));
        //    _make.Body.Emit(OpCodes.Call, Reflection.Metadata.Method(() => Convert.FromBase64String(Argument<string>.Value)));
        //    _make.Body.Emit(OpCodes.Call, Reflection.Metadata.Method(() => Assembly.Load(Argument<byte[]>.Value)));
        //    _make.Body.Emit(OpCodes.Ret);
        //    var _fake = _module.Method<Assembly>("<Puresharp.Confluence<Fake>>", MethodAttributes.Static | MethodAttributes.Private);
        //    _fake.Parameter<object>("sender");
        //    _fake.Parameter<ResolveEventArgs>("arguments");
        //    _fake.Body.Emit(OpCodes.Ldarg_1);
        //    _fake.Body.Emit(OpCodes.Call, Metadata<ResolveEventArgs>.Property(_ResolveEventArgs => _ResolveEventArgs.Name).GetGetMethod());
        //    _fake.Body.Emit(OpCodes.Ldstr, "Puresharp.Confluence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a040f522acb22b09");
        //    var _null = Instruction.Create(OpCodes.Ldnull);
        //    _fake.Body.Emit(OpCodes.Call, Reflection.Metadata.Method(() => string.Equals(Argument<string>.Value, Argument<string>.Value)));
        //    _fake.Body.Emit(OpCodes.Brfalse, _null);
        //    _fake.Body.Emit(OpCodes.Ldsfld, _lazy);
        //    _fake.Body.Emit(OpCodes.Call, Metadata<Lazy<Assembly>>.Property(_Lazy => _Lazy.Value).GetGetMethod());
        //    _fake.Body.Emit(OpCodes.Ret);
        //    _fake.Body.Add(_null);
        //    _fake.Body.Emit(OpCodes.Ret);
        //    var _initializer = _module.Initializer();
        //    _initializer.Body.Emit(OpCodes.Ldnull);
        //    _initializer.Body.Emit(OpCodes.Ldftn, _make);
        //    _initializer.Body.Emit(OpCodes.Newobj, Metadata<Func<Assembly>>.Type.GetConstructors().Single());
        //    _initializer.Body.Emit(OpCodes.Ldc_I4, (int)System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        //    _initializer.Body.Emit(OpCodes.Newobj, Reflection.Metadata.Constructor(() => new Lazy<Assembly>(Argument<Func<Assembly>>.Value, Argument<System.Threading.LazyThreadSafetyMode>.Value)));
        //    _initializer.Body.Emit(OpCodes.Stsfld, _lazy);
        //    _initializer.Body.Emit(OpCodes.Call, Reflection.Metadata.Property(() => AppDomain.CurrentDomain).GetGetMethod());
        //    _initializer.Body.Emit(OpCodes.Ldnull);
        //    _initializer.Body.Emit(OpCodes.Ldftn, _fake);
        //    _initializer.Body.Emit(OpCodes.Newobj, typeof(ResolveEventHandler).GetConstructors().Single());
        //    _initializer.Body.Emit(OpCodes.Callvirt, typeof(AppDomain).GetMethod("add_AssemblyResolve", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public));
        //    _initializer.Body.Emit(OpCodes.Ret);
        //}

        static private void Include(this AssemblyDefinition assembly, params Assembly[] dependencies)
        {
            if (Program.m_Inclusion.Contains(assembly)) { return; }
            var _module = assembly.MainModule.Types.First(_Type => _Type.Name == Program.Module);
            var _initializer = _module.Initializer();

            var _fake = _module.Method<Assembly>("<Puresharp<Fake>>", MethodAttributes.Static | MethodAttributes.Private);
            _fake.Parameter<object>("sender");
            _fake.Parameter<ResolveEventArgs>("arguments");

            foreach (var _dependency in dependencies)
            {
                var _lazy = _module.Field<Lazy<Assembly>>($"<{ _dependency.GetName().Name }>", FieldAttributes.Static | FieldAttributes.Private);
                Program.m_Inclusion.Add(assembly);
                var _make = _module.Method<Assembly>($"<{ _dependency.GetName().Name }<Make>>", MethodAttributes.Static | MethodAttributes.Private);
                _make.Body.Emit(OpCodes.Ldstr, Convert.ToBase64String(File.ReadAllBytes(_dependency.Location)));
                _make.Body.Emit(OpCodes.Call, Discovery.Metadata.Method(() => Convert.FromBase64String(Argument<string>.Value)));
                _make.Body.Emit(OpCodes.Call, Discovery.Metadata.Method(() => Assembly.Load(Argument<byte[]>.Value)));
                _make.Body.Emit(OpCodes.Ret);
               
                _fake.Body.Emit(OpCodes.Ldarg_1);
                _fake.Body.Emit(OpCodes.Call, Metadata<ResolveEventArgs>.Property(_ResolveEventArgs => _ResolveEventArgs.Name).GetGetMethod());
                _fake.Body.Emit(OpCodes.Ldstr, _dependency.FullName);
                _fake.Body.Emit(OpCodes.Call, Discovery.Metadata.Method(() => string.Equals(Argument<string>.Value, Argument<string>.Value)));
                using (_fake.Body.True())
                {
                    _fake.Body.Emit(OpCodes.Ldsfld, _lazy);
                    _fake.Body.Emit(OpCodes.Call, Metadata<Lazy<Assembly>>.Property(_Lazy => _Lazy.Value).GetGetMethod());
                    _fake.Body.Emit(OpCodes.Ret);
                }
                _initializer.Body.Emit(OpCodes.Ldnull);
                _initializer.Body.Emit(OpCodes.Ldftn, _make);
                _initializer.Body.Emit(OpCodes.Newobj, Metadata<Func<Assembly>>.Type.GetConstructors().Single());
                _initializer.Body.Emit(OpCodes.Ldc_I4, (int)LazyThreadSafetyMode.ExecutionAndPublication);
                _initializer.Body.Emit(OpCodes.Newobj, Discovery.Metadata.Constructor(() => new Lazy<Assembly>(Argument<Func<Assembly>>.Value, Argument<LazyThreadSafetyMode>.Value)));
                _initializer.Body.Emit(OpCodes.Stsfld, _lazy);
            }
            _fake.Body.Emit(OpCodes.Ldnull);
            _fake.Body.Emit(OpCodes.Ret);

            _initializer.Body.Emit(OpCodes.Call, Discovery.Metadata.Property(() => AppDomain.CurrentDomain).GetGetMethod());
            _initializer.Body.Emit(OpCodes.Ldnull);
            _initializer.Body.Emit(OpCodes.Ldftn, _fake);
            _initializer.Body.Emit(OpCodes.Newobj, typeof(ResolveEventHandler).GetConstructors().Single());
            _initializer.Body.Emit(OpCodes.Callvirt, typeof(AppDomain).GetMethod("add_AssemblyResolve", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public));
            _initializer.Body.Emit(OpCodes.Ret);
        }

        //static private Boundary Boundary(this ModuleDefinition module)
        //{
        //    return new Boundary
        //    (
        //        module.Import(typeof(Advice.IBoundary)),
        //        module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Instance(Argument<object>.Value)).GetGenericMethodDefinition()),
        //        module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Argument(Argument<ParameterInfo>.Value, ref Argument<object>.Value)).GetGenericMethodDefinition()),
        //        module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Begin())),
        //        module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Await(Argument<Task>.Value))),
        //        module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Continue())),
        //        new Boundary.Feedback
        //        (
        //            module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Return())),
        //            module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Throw(ref Argument<Exception>.Value)))
        //        ),
        //        new Boundary.Feedback
        //        (
        //            module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Return(ref Argument<object>.Value)).GetGenericMethodDefinition()),
        //            module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Throw(ref Argument<Exception>.Value, ref Argument<object>.Value)).GetGenericMethodDefinition())
        //        )
        //    );
        //}

        static private TypeDefinition Authority(this TypeDefinition type)
        {
            foreach (var _type in type.NestedTypes) { if (_type.Name == Program.Puresharp) { return _type; } }
            return type.Type(Program.Puresharp, TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.NestedAssembly | TypeAttributes.BeforeFieldInit | TypeAttributes.SpecialName);
        }

        static private TypeDefinition Authority(this TypeDefinition type, string name)
        {
            var _authority = type.Authority();
            foreach (var _type in _authority.NestedTypes) { if (_type.Name == name) { return _type; } }
            return _authority.Type(name, TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.NestedPublic | TypeAttributes.BeforeFieldInit | TypeAttributes.SpecialName);
        }

        static private MethodDefinition Authentic(this MethodDefinition method)
        {
            var _type = method.DeclaringType.Authority("<Authentic>");
            var _copy = new Copy(method);
            var _method = _type.Method(method.IsConstructor ? "<Constructor>" : method.Name, MethodAttributes.Static | MethodAttributes.Public);
            foreach (var _attribute in method.CustomAttributes) { _method.CustomAttributes.Add(_attribute); }
            foreach (var _parameter in method.GenericParameters) { _method.GenericParameters.Add(new GenericParameter(_parameter.Name, _method)); }
            _copy.Genericity = _method.GenericParameters.ToArray();
            _method.ReturnType = _copy[method.ReturnType];
            if (!method.IsStatic) { _method.Parameters.Add(new ParameterDefinition("this", ParameterAttributes.None, method.DeclaringType)); }
            foreach (var _parameter in method.Parameters) { _method.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, _copy[_parameter.ParameterType])); }
            _copy.Signature = _method.Parameters.ToArray();
            var _body = _method.Body;
            _body.InitLocals = method.Body.InitLocals;
            foreach (var _variable in method.Body.Variables) { _body.Add(new VariableDefinition(_variable.Name, _copy[_variable.VariableType])); }
            _copy.Variation = _body.Variables.ToArray();
            foreach (var _instruction in method.Body.Instructions) { _body.Instructions.Add(_copy[_instruction, method.Module]); }

            //TODO : for virtual method => replace base call to "pure path"!
            if (method.IsVirtual)
            {
                //lookup base call to same method definition and swap it to direct base authentic call!
                //it will allow to wrap the entire virtual call.
            }

            foreach (var _exception in method.Body.ExceptionHandlers)
            {
                _body.ExceptionHandlers.Add(new ExceptionHandler(_exception.HandlerType)
                {
                    CatchType = _exception.CatchType,
                    TryStart = _copy[_exception.TryStart, method.Module],
                    TryEnd = _copy[_exception.TryEnd, method.Module],
                    HandlerType = _exception.HandlerType,
                    HandlerStart = _copy[_exception.HandlerStart, method.Module],
                    HandlerEnd = _copy[_exception.HandlerEnd, method.Module]
                });
            }
            method.Body.OptimizeMacros();
            _body.OptimizeMacros();
            return _method;
        }

        static private FieldDefinition Intermediate(this MethodDefinition method, MethodDefinition authentic)
        {
            var _intermediate = method.DeclaringType.Authority("<Intermediate>").Type(method.IsConstructor ? $"<<Constructor>>" : $"<{method.Name}>", TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
            foreach (var _parameter in method.GenericParameters) { _intermediate.GenericParameters.Add(new GenericParameter(_parameter.Name, _intermediate)); }
            var _handle = _intermediate.Field<object>("<Handle>", FieldAttributes.Static | FieldAttributes.Public);
            var _authentic = _intermediate.Field<IntPtr>("<Authentic>", FieldAttributes.Static | FieldAttributes.Public);
            var _auxiliary = _intermediate.Field<IntPtr>("<Auxiliary>", FieldAttributes.Static | FieldAttributes.Public);
            var _pointer = _intermediate.Field<IntPtr>("<Pointer>", FieldAttributes.Static | FieldAttributes.Public);

            var _update = _intermediate.Method("<Update>", MethodAttributes.Static | MethodAttributes.Private);
            _update.Parameter<IntPtr>("<Pointer>");
            var _return = Instruction.Create(OpCodes.Ret);
            using (_update.Body.Lock(_handle))
            {
                _update.Body.Emit(OpCodes.Ldsfld, _auxiliary);
                using (_update.Body.True())
                {
                    _update.Body.Emit(OpCodes.Ldarg_0);
                    _update.Body.Emit(OpCodes.Stsfld, _auxiliary);
                    _update.Body.Emit(OpCodes.Leave, _return);
                }
                _update.Body.Emit(OpCodes.Ldarg_0);
                _update.Body.Emit(OpCodes.Stsfld, _pointer);
            }
            _update.Body.Emit(_return);

            var _copy = new Copy(method);
            var _primary = _intermediate.Method("<Primary>", MethodAttributes.Static | MethodAttributes.Public);
            foreach (var _attribute in method.CustomAttributes) { _primary.CustomAttributes.Add(_attribute); }
            foreach (var _parameter in method.GenericParameters) { _primary.GenericParameters.Add(new GenericParameter(_parameter.Name, _primary)); }
            _copy.Genericity = _primary.GenericParameters.ToArray();
            _primary.ReturnType = _copy[method.ReturnType];
            if (!method.IsStatic) { _primary.Parameters.Add(new ParameterDefinition("this", ParameterAttributes.None, method.DeclaringType)); }
            foreach (var _parameter in method.Parameters) { _primary.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, _copy[_parameter.ParameterType])); }
            _primary.Body.Variable<IntPtr>("<Pointer>");
            _primary.Body.Emit(OpCodes.Ldsfld, _handle);
            _primary.Body.Emit(OpCodes.Call, Discovery.Metadata.Method(() => Monitor.Enter(Argument<object>.Value)));

            _primary.Body.Emit(Metadata<Action<MethodBase>>.Type);
            _primary.Body.Emit(typeof(Metadata));
            _primary.Body.Emit(OpCodes.Ldstr, "Broadcast");
            _primary.Body.Emit(OpCodes.Ldc_I4, (int)(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly));
            _primary.Body.Emit(OpCodes.Call, Metadata<Type>.Method(_Type => _Type.GetMethod(Argument<string>.Value, Argument<System.Reflection.BindingFlags>.Value)));
            _primary.Body.Emit(OpCodes.Call, Discovery.Metadata.Method(() => Delegate.CreateDelegate(Argument<Type>.Value, Argument<MethodInfo>.Value)));
            _primary.Body.Emit(method);
            _primary.Body.Emit(OpCodes.Callvirt, Metadata<Action<MethodBase>>.Method(_Action => _Action.Invoke(Argument<MethodBase>.Value)));

            _primary.Body.Emit(OpCodes.Ldsfld, _pointer);
            _primary.Body.Emit(OpCodes.Stloc_0);
            _primary.Body.Emit(OpCodes.Ldsfld, _auxiliary);
            using (_primary.Body.True())
            {
                _primary.Body.Emit(OpCodes.Ldsfld, _auxiliary);
                _primary.Body.Emit(OpCodes.Stloc_0);
                _primary.Body.Emit(OpCodes.Ldloc_0);
                _primary.Body.Emit(OpCodes.Stsfld, _pointer);
                _primary.Body.Emit(OpCodes.Ldsfld, Discovery.Metadata.Field(() => IntPtr.Zero));
                _primary.Body.Emit(OpCodes.Stsfld, _auxiliary);
            }

            _primary.Body.Emit(OpCodes.Ldsfld, _handle);
            _primary.Body.Emit(OpCodes.Call, Discovery.Metadata.Method(() => Monitor.Exit(Argument<object>.Value)));

            for (var _index = 0; _index < _primary.Parameters.Count; _index++)
            {
                switch (_index)
                {
                    case 0: _primary.Body.Emit(OpCodes.Ldarg_0); break;
                    case 1: _primary.Body.Emit(OpCodes.Ldarg_1); break;
                    case 2: _primary.Body.Emit(OpCodes.Ldarg_2); break;
                    case 3: _primary.Body.Emit(OpCodes.Ldarg_3); break;
                    default: _primary.Body.Emit(OpCodes.Ldarg_S, _primary.Parameters[ _index]); break;
                }
            }
            //call!
            if (method.GenericParameters.Count == 0)
            {
                _primary.Body.Emit(OpCodes.Ldloc_0);
                _primary.Body.Emit(OpCodes.Calli, method.ReturnType, authentic.Parameters);
            }
            else
            {
                var _type = new GenericInstanceType(_intermediate.DeclaringType);
                foreach (var _parameter in method.GenericParameters) { _type.GenericArguments.Add(_parameter); }
                _primary.Body.Emit(OpCodes.Ldloc_0);
                var _method1 = new GenericInstanceMethod(authentic);
                foreach (var _parameter in method.GenericParameters) { _method1.GenericArguments.Add(_parameter); }
                _primary.Body.Emit(OpCodes.Calli, _method1.ReturnType, _method1.Parameters);
            }


            _primary.Body.Emit(OpCodes.Ret);
            _primary.Body.OptimizeMacros();


            var _initializer = _intermediate.Initializer();
            var _variable = _initializer.Body.Variable<RuntimeMethodHandle>();
            _initializer.Body.Variable<Func<IntPtr>>();
            _initializer.Body.Emit(OpCodes.Newobj, Discovery.Metadata.Constructor(() => new object()));
            _initializer.Body.Emit(OpCodes.Stsfld, _handle);
            if (_intermediate.GenericParameters.Count == 0) //TODO Virtuoze : Replace direct init by authentic by checking appdomain.CurrentDocumain.GetData('Resolver'). => if(resolver == null) => authentic else call resolver to get pointer!
            {
                _initializer.Body.Emit(authentic);
                _initializer.Body.Emit(OpCodes.Callvirt, Program.GetMethodHandle);
                _initializer.Body.Emit(OpCodes.Stloc_0);
                _initializer.Body.Emit(OpCodes.Ldloca_S, _variable);
                _initializer.Body.Emit(OpCodes.Callvirt, Program.GetFunctionPointer);
                _initializer.Body.Emit(OpCodes.Stsfld, _authentic);
                _initializer.Body.Emit(OpCodes.Ldsfld, _authentic);
                _initializer.Body.Emit(OpCodes.Stsfld, _auxiliary);

                _initializer.Body.Emit(_primary);
                _initializer.Body.Emit(OpCodes.Callvirt, Program.GetMethodHandle);
                _initializer.Body.Emit(OpCodes.Stloc_0);
                _initializer.Body.Emit(OpCodes.Ldloca_S, _variable);
                _initializer.Body.Emit(OpCodes.Callvirt, Program.GetFunctionPointer);
                _initializer.Body.Emit(OpCodes.Stsfld, _pointer);
            }
            else
            {
                _initializer.Body.Emit(authentic.MakeGenericMethod(_intermediate.GenericParameters));
                _initializer.Body.Emit(OpCodes.Callvirt, Program.GetMethodHandle);
                _initializer.Body.Emit(OpCodes.Stloc_0);
                _initializer.Body.Emit(OpCodes.Ldloca_S, _variable);
                _initializer.Body.Emit(OpCodes.Callvirt, Program.GetFunctionPointer);
                _initializer.Body.Emit(OpCodes.Stsfld, new FieldReference(_authentic.Name, _authentic.FieldType, _intermediate.MakeGenericType(_intermediate.GenericParameters)));
                _initializer.Body.Emit(OpCodes.Ldsfld, new FieldReference(_authentic.Name, _authentic.FieldType, _intermediate.MakeGenericType(_intermediate.GenericParameters)));
                _initializer.Body.Emit(OpCodes.Stsfld, new FieldReference(_auxiliary.Name, _auxiliary.FieldType, _intermediate.MakeGenericType(_intermediate.GenericParameters)));

                _initializer.Body.Emit(_primary.Relative());
                _initializer.Body.Emit(OpCodes.Callvirt, Program.GetMethodHandle);
                _initializer.Body.Emit(OpCodes.Stloc_0);
                _initializer.Body.Emit(OpCodes.Ldloca_S, _variable);
                _initializer.Body.Emit(OpCodes.Callvirt, Program.GetFunctionPointer);
                _initializer.Body.Emit(OpCodes.Stsfld, new FieldReference(_pointer.Name, _pointer.FieldType, _intermediate.MakeGenericType(_intermediate.GenericParameters)));

                //TODO : IOC of AOP !? What the? in fact it will be used to be able to inject on method on demand but a late as possible.
                //Action<MethodBase> _update;
                //lock (AppDomain.CurrentDomain.Evidence.SyncRoot) { _update = AppDomain.CurrentDomain.GetData("<Neptune<Update>>") as Action<MethodBase>; }
                //if (_update != null) { _update(...); }
            }
            _initializer.Body.Emit(OpCodes.Ret);
            _initializer.Body.OptimizeMacros();
            return _pointer;
        }

        static private FieldDefinition Metadata(this MethodDefinition method)
        {
            var _metadata = method.DeclaringType.Authority("<Metadata>").Type(method.IsConstructor ? $"<<Constructor>>" : $"<{method.Name}>", TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
            foreach (var _parameter in method.GenericParameters) { _metadata.GenericParameters.Add(new GenericParameter(_parameter.Name, _metadata)); }
            var _method = _metadata.Field<MethodBase>("<Method>", FieldAttributes.Static | FieldAttributes.Public);
            var _signature = _metadata.Field<ParameterInfo[]>("<Signature>", FieldAttributes.Static | FieldAttributes.Public);
            var _initializer = _metadata.Initializer();
            if (_metadata.HasGenericParameters)
            {
                _initializer.Body.Emit(method.MakeGenericMethod(_metadata.GenericParameters));
                _initializer.Body.Emit(OpCodes.Stsfld, new FieldReference(_method.Name, _method.FieldType, _metadata.MakeGenericType(_metadata.GenericParameters)));
            }
            else
            {
                _initializer.Body.Emit(method);
                _initializer.Body.Emit(OpCodes.Stsfld, _method);
            }
            _initializer.Body.Emit(OpCodes.Ldsfld, _method);
            _initializer.Body.Emit(OpCodes.Callvirt, Metadata<MethodBase>.Method(_Method => _Method.GetParameters()));
            _initializer.Body.Emit(OpCodes.Stsfld, _signature);
            for (var _index = 0; _index < method.Parameters.Count; _index++)
            {
                _initializer.Body.Emit(OpCodes.Ldsfld, _signature);
                _initializer.Body.Emit(OpCodes.Ldc_I4, _index);
                _initializer.Body.Emit(OpCodes.Ldelem_Ref);
                _initializer.Body.Emit(OpCodes.Stsfld, _metadata.Field<ParameterInfo>(method.Parameters[_index].Name, FieldAttributes.Static | FieldAttributes.Public));
            }
            _initializer.Body.Emit(OpCodes.Ret);
            _initializer.Body.OptimizeMacros();
            return _method;
        }

        static private void Manage(this MethodDefinition method)
        {
            Program.Include(method.Module.Assembly, typeof(Metadata).Assembly, typeof(Advice.IBoundary).Assembly);
            var _metadata = method.Metadata();
            var _machine = method.CustomAttributes.SingleOrDefault(_Attribute => _Attribute.AttributeType.Resolve() == method.Module.Import(typeof(AsyncStateMachineAttribute)).Resolve());
            var _authentic = method.Authentic();
            var _intermediate = method.Intermediate(_authentic);
            method.Body = new MethodBody(method);
            for (var _index = 0; _index < _authentic.Parameters.Count; _index++)
            {
                switch (_index)
                {
                    case 0: method.Body.Emit(OpCodes.Ldarg_0); break;
                    case 1: method.Body.Emit(OpCodes.Ldarg_1); break;
                    case 2: method.Body.Emit(OpCodes.Ldarg_2); break;
                    case 3: method.Body.Emit(OpCodes.Ldarg_3); break;
                    default: method.Body.Emit(OpCodes.Ldarg_S, method.Parameters[method.IsStatic ? _index : _index - 1]); break;
                }
            }
            if (method.GenericParameters.Count == 0)
            {
                method.Body.Emit(OpCodes.Ldsfld, _intermediate);
                method.Body.Emit(OpCodes.Calli, method.ReturnType, _authentic.Parameters);
            }
            else
            {
                var _type = new GenericInstanceType(_intermediate.DeclaringType);
                foreach (var _parameter in method.GenericParameters) { _type.GenericArguments.Add(_parameter); }
                method.Body.Emit(OpCodes.Ldsfld, new FieldReference(_intermediate.Name, _intermediate.FieldType, _type));
                var _method = new GenericInstanceMethod(_authentic);
                foreach (var _parameter in method.GenericParameters) { _method.GenericArguments.Add(_parameter); }
                method.Body.Emit(OpCodes.Calli, _method.ReturnType, _method.Parameters);
            }
            method.Body.Emit(OpCodes.Ret);
            method.Body.OptimizeMacros();
            if (_machine != null)
            {
                var _type = _machine.ConstructorArguments[0].Value as TypeDefinition;

                //get from appdomain => factory of factory to define initial factory!
                //Generate custom delegate to ref delegate :-(
                //TODO Virtuoze => replace boundary/factory by delegate/delegate factory for each method.
                //var _lifecycle = _type.Module.Boundary();
                var _factory = _type.Field<Advice.Boundary.IFactory>("<Factory>", FieldAttributes.Public | FieldAttributes.Static);
                var _boundary = _type.Field<Advice.IBoundary>("<Boundary>", FieldAttributes.Public);
                _type.IsBeforeFieldInit = true;
                var _intializer = _type.Initializer();
                _intializer.Body.Emit(OpCodes.Newobj, Discovery.Metadata.Constructor(() => new Advice.Boundary.Factory())); //TODO Virtuoze => get data from appdomain to detect default boundary for method! if null => instantiate ABF
                _intializer.Body.Emit(OpCodes.Stsfld, _factory.Relative());
                _intializer.Body.Emit(OpCodes.Ret);
                var _constructor = _type.Methods.Single(m => m.IsConstructor && !m.IsStatic);
                _constructor.Body = new MethodBody(_constructor);
                _constructor.Body.Emit(OpCodes.Ldarg_0);
                _constructor.Body.Emit(OpCodes.Call, Discovery.Metadata.Constructor(() => new object()));
                _constructor.Body.Emit(OpCodes.Ldarg_0);
                _constructor.Body.Emit(OpCodes.Ldsfld, _constructor.Module.Import(_factory.Relative()));
                _constructor.Body.Emit(OpCodes.Callvirt, Metadata<Advice.Boundary.IFactory>.Method(_Factory => _Factory.Create()));
                _constructor.Body.Emit(OpCodes.Stfld, _boundary.Relative());
                _constructor.Body.Emit(OpCodes.Ret);
                var _move = _type.Methods.Single(_Method => _Method.Name == "MoveNext");
                var _task = _move.Body.Variable<Task>("<Task>");
                var _instance = method.IsStatic ? null : _type.Fields.Single(_Field => _Field.Name == "<>4__this").Relative();
                var _state = _type.Fields.Single(_Field => _Field.Name == "<>1__state").Relative();
                var _builder = _type.Fields.Single(_Field => _Field.Name == "<>t__builder").Relative();
                var _offset = 0;
                var _begin = _move.Body.Instructions[_offset];
                var _resume = Instruction.Create(OpCodes.Ldarg_0);
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _state));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldc_I4_0));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Bge, _resume));
                //_move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                //_move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _boundary.Relative()));
                //_move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldsfld, _metadata));
                //_move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldsfld, _metadata.DeclaringType.Fields.Single(_Field => _Field.Name == "<Signature>")));
                //_move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(_lifecycle.Method))));
                if (_instance != null)
                {
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _boundary.Relative()));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _instance));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Instance<object>(Argument<object>.Value)).GetGenericMethodDefinition()).MakeGenericMethod(method.DeclaringType))));
                }
                foreach (var _parameter in method.Parameters)
                {
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _boundary.Relative()));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldsfld, _metadata.DeclaringType.Fields.Single(_Field => _Field.Name == _parameter.Name)));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldflda, _type.Fields.First(_Field => _Field.Name == _parameter.Name).Relative()));
                    _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Argument<object>(Argument<ParameterInfo>.Value, ref Argument<object>.Value)).GetGenericMethodDefinition()).MakeGenericMethod(_parameter.ParameterType.IsGenericParameter ? _type.GenericParameters.First(_Type => _Type.Name == _parameter.ParameterType.Name) : _parameter.ParameterType))));
                }
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _boundary.Relative()));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Begin()))));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Br_S, _begin));
                _move.Body.Instructions.Insert(_offset++, _resume);
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _boundary.Relative()));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Continue()))));
                while (_offset < _move.Body.Instructions.Count)
                {
                    var _instruction = _move.Body.Instructions[_offset];
                    if (_instruction.OpCode == OpCodes.Callvirt)
                    {
                        if (_instruction.Operand is MethodReference)
                        {
                            var _operand = _instruction.Operand as MethodReference;
                            if (_operand.Name == "GetAwaiter")
                            {
                                var _action = _move.Body.Instructions[_offset - 1].Operand as MethodReference;
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Stloc, _task));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _boundary.Relative()));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldtoken, _action));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldtoken, _action.DeclaringType));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Call, _move.Module.Import(Discovery.Metadata.Method(() => MethodInfo.GetMethodFromHandle(Argument<RuntimeMethodHandle>.Value, Argument<RuntimeTypeHandle>.Value))))); //TODO Virtuoze => cache it!
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldloca, _task));
                                if (_action.ReturnType.Resolve() == _move.Module.Import(typeof(Task)).Resolve()) { _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Await(Argument<MethodInfo>.Value, ref Argument<Task>.Value))))); }
                                else { _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Await(Argument<MethodInfo>.Value, ref Argument<Task<object>>.Value)).GetGenericMethodDefinition()).MakeGenericMethod((_action.ReturnType as GenericInstanceType).GenericArguments[0]))); }
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldloc, _task));
                            }
                        }
                    }
                    else if (_instruction.OpCode == OpCodes.Call)
                    {
                        if (_instruction.Operand is MethodReference)
                        {
                            var _operand = _instruction.Operand as MethodReference;
                            if (_operand.Name == "get_IsCompleted")
                            {
                                var _continue = _move.Body.Instructions[++_offset];
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Dup));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Brfalse_S, _continue));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _boundary.Relative()));
                                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Continue()))));
                            }
                            else if (_operand.Name == "SetResult")
                            {
                                var _return = _type.Method("<Return>", MethodAttributes.Public);
                                if (_operand.HasParameters)
                                {
                                    var _parameter = new ParameterDefinition("<Value>", ParameterAttributes.None, (_builder.FieldType as GenericInstanceType).GenericArguments[0]);
                                    _return.Parameters.Add(_parameter);
                                    var _exception = _return.Body.Variable<Exception>("<Exception>");
                                    var _disposed = _return.Body.Variable<bool>("<Invoked>");
                                    var _end = Instruction.Create(OpCodes.Ret);
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                    _return.Body.Emit(OpCodes.Ldarga_S, _parameter);
                                    _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Return(ref Argument<object>.Value)).GetGenericMethodDefinition()).MakeGenericMethod(_parameter.ParameterType)));
                                    _return.Body.Emit(OpCodes.Ldc_I4_1);
                                    _return.Body.Emit(OpCodes.Stloc_1);
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                    _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldflda, _builder);
                                    _return.Body.Emit(OpCodes.Ldarg_1);
                                    _return.Body.Emit(OpCodes.Call, _operand);
                                    _return.Body.Emit(OpCodes.Ret);
                                    var _catch = _return.Body.Emit(OpCodes.Stloc_0);
                                    _return.Body.Emit(OpCodes.Ldloc_1);
                                    using (_return.Body.False())
                                    {
                                        _return.Body.Emit(OpCodes.Ldarg_0);
                                        _return.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                        _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                    }
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldflda, _builder);
                                    _return.Body.Emit(OpCodes.Ldloc_0);
                                    var _method = _move.Module.Import(_builder.FieldType.Resolve().Methods.Single(_Method => _Method.Name == "SetException"));
                                    _method.DeclaringType = _builder.FieldType;
                                    _return.Body.Emit(OpCodes.Call, _method);
                                    _return.Body.Emit(OpCodes.Ret);
                                    _return.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                    {
                                        TryStart = _return.Body.Instructions[0],
                                        TryEnd = _return.Body.Instructions[_catch],
                                        HandlerStart = _return.Body.Instructions[_catch],
                                        HandlerEnd = _return.Body.Instructions[_return.Body.Instructions.Count - 1],
                                        CatchType = _exception.VariableType
                                    });
                                    _return.Body.OptimizeMacros();
                                    _instruction.Operand = _type.HasGenericParameters ? _return.MakeHostInstanceGeneric(_type.GenericParameters.ToArray()) : _return;
                                    _move.Body.Instructions[_offset - 2].OpCode = OpCodes.Nop;
                                }
                                else
                                {
                                    var _exception = _return.Body.Variable<Exception>("<Exception>");
                                    var _disposed = _return.Body.Variable<bool>("<Invoked>");
                                    var _end = Instruction.Create(OpCodes.Ret);
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                    _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Return())));
                                    _return.Body.Emit(OpCodes.Ldc_I4_1);
                                    _return.Body.Emit(OpCodes.Stloc_1);
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                    _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldflda, _builder);
                                    _return.Body.Emit(OpCodes.Call, _operand);
                                    _return.Body.Emit(OpCodes.Ret);
                                    var _catch = _return.Body.Emit(OpCodes.Stloc_0);
                                    _return.Body.Emit(OpCodes.Ldloc_1);
                                    using (_return.Body.False())
                                    {
                                        _return.Body.Emit(OpCodes.Ldarg_0);
                                        _return.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                        _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                    }
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldflda, _builder);
                                    _return.Body.Emit(OpCodes.Ldloc_0);
                                    var _method = _move.Module.Import(_builder.FieldType.Resolve().Methods.Single(_Method => _Method.Name == "SetException"));
                                    _method.DeclaringType = _builder.FieldType;
                                    _return.Body.Emit(OpCodes.Call, _method);
                                    _return.Body.Emit(OpCodes.Ret);
                                    _return.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                    {
                                        TryStart = _return.Body.Instructions[0],
                                        TryEnd = _return.Body.Instructions[_catch],
                                        HandlerStart = _return.Body.Instructions[_catch],
                                        HandlerEnd = _return.Body.Instructions[_return.Body.Instructions.Count - 1],
                                        CatchType = _exception.VariableType,
                                    });
                                    _return.Body.OptimizeMacros();
                                    _instruction.Operand = _type.HasGenericParameters ? _return.MakeHostInstanceGeneric(_type.GenericParameters.ToArray()) : _return;
                                    _move.Body.Instructions[_offset - 1].OpCode = OpCodes.Nop;
                                }
                            }
                            else if (_operand.Name == "SetException")
                            {
                                var _throw = _type.Method("<Throw>", MethodAttributes.Public);
                                var _parameter = new ParameterDefinition("<Exception>", ParameterAttributes.None, _throw.Module.Import(typeof(Exception)));
                                _throw.Parameters.Add(_parameter);
                                if (_builder.FieldType.IsGenericInstance)
                                {
                                    var _value = new VariableDefinition("<Value>", (_builder.FieldType as GenericInstanceType).GenericArguments[0]);
                                    _throw.Body.Variables.Add(_value);
                                    var _disposed = _throw.Body.Variable<bool>("<Invoked>");
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                    _throw.Body.Emit(OpCodes.Ldarg_S, _parameter);
                                    _throw.Body.Emit(OpCodes.Ldloca_S, _value);
                                    _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Throw(ref Argument<Exception>.Value, ref Argument<object>.Value)).GetGenericMethodDefinition()).MakeGenericMethod(_value.VariableType));
                                    _throw.Body.Emit(OpCodes.Ldc_I4_1);
                                    _throw.Body.Emit(OpCodes.Stloc_1);
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                    _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                    _throw.Body.Emit(OpCodes.Ldarg_1);
                                    using (_throw.Body.True())
                                    {
                                        _throw.Body.Emit(OpCodes.Ldarg_0);
                                        _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                        _throw.Body.Emit(OpCodes.Ldarg_1);
                                        _throw.Body.Emit(OpCodes.Call, _operand);
                                        _throw.Body.Emit(OpCodes.Ret);
                                    }
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                    _throw.Body.Emit(OpCodes.Ldloc_0);
                                    var _method = _move.Module.Import(_move.Module.Import(_builder.FieldType.Resolve().Methods.Single(_Method => _Method.Name == "SetResult" && _Method.Parameters[0].ParameterType.IsGenericParameter)));
                                    _method.DeclaringType = _builder.FieldType;
                                    _throw.Body.Emit(OpCodes.Call, _method);
                                    _throw.Body.Emit(OpCodes.Ret);
                                    var _catch = _throw.Body.Emit(OpCodes.Starg, _parameter);
                                    _throw.Body.Emit(OpCodes.Ldloc_1);
                                    using (_throw.Body.False())
                                    {
                                        _throw.Body.Emit(OpCodes.Ldarg_0);
                                        _throw.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                        _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                    }
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                    _throw.Body.Emit(OpCodes.Ldarg_1);
                                    _throw.Body.Emit(OpCodes.Call, _operand);
                                    _throw.Body.Emit(OpCodes.Ret);
                                    _throw.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                    {
                                        TryStart = _throw.Body.Instructions[0],
                                        TryEnd = _throw.Body.Instructions[_catch],
                                        HandlerStart = _throw.Body.Instructions[_catch],
                                        HandlerEnd = _throw.Body.Instructions[_throw.Body.Instructions.Count - 1],
                                        CatchType = _parameter.ParameterType,
                                    });
                                }
                                else
                                {
                                    var _disposed = _throw.Body.Variable<bool>("<Invoked>");
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                    _throw.Body.Emit(OpCodes.Ldarga_S, _parameter);
                                    _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(Metadata<Advice.IBoundary>.Method(_Boundary => _Boundary.Throw(ref Argument<Exception>.Value))));
                                    _throw.Body.Emit(OpCodes.Ldc_I4_1);
                                    _throw.Body.Emit(OpCodes.Stloc_0);
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                    _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                    _throw.Body.Emit(OpCodes.Ldarg_1);
                                    using (_throw.Body.True())
                                    {
                                        _throw.Body.Emit(OpCodes.Ldarg_0);
                                        _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                        _throw.Body.Emit(OpCodes.Ldarg_1);
                                        _throw.Body.Emit(OpCodes.Call, _operand);
                                        _throw.Body.Emit(OpCodes.Ret);
                                    }
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                    var _method = _move.Module.Import(_builder.FieldType.Resolve().Methods.Single(_Method => _Method.Name == "SetResult" && _Method.Parameters.Count == 0));
                                    _method.DeclaringType = _builder.FieldType;
                                    _throw.Body.Emit(OpCodes.Call, _method);
                                    _throw.Body.Emit(OpCodes.Ret);
                                    var _catch = _throw.Body.Emit(OpCodes.Starg, _parameter);
                                    _throw.Body.Emit(OpCodes.Ldloc_0);
                                    using (_throw.Body.False())
                                    {
                                        _throw.Body.Emit(OpCodes.Ldarg_0);
                                        _throw.Body.Emit(OpCodes.Ldfld, _boundary.Relative());
                                        _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                    }
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                    _throw.Body.Emit(OpCodes.Ldarg_1);
                                    _throw.Body.Emit(OpCodes.Call, _operand);
                                    _throw.Body.Emit(OpCodes.Ret);
                                    _throw.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                    {
                                        TryStart = _throw.Body.Instructions[0],
                                        TryEnd = _throw.Body.Instructions[_catch],
                                        HandlerStart = _throw.Body.Instructions[_catch],
                                        HandlerEnd = _throw.Body.Instructions[_throw.Body.Instructions.Count - 1],
                                        CatchType = _parameter.ParameterType,
                                    });
                                }
                                _throw.Body.OptimizeMacros();
                                _instruction.Operand = _type.HasGenericParameters ? _throw.MakeHostInstanceGeneric(_type.GenericParameters.ToArray()) : _throw;
                                _move.Body.Instructions[_offset - 2].OpCode = OpCodes.Nop;
                            }
                        }
                    }
                    _offset++;
                }
                _move.Body.OptimizeMacros();
            }
        }
    }
}
