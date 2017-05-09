using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static public partial class __Advice
    {
        static public Advice Boundary<T>(this Advice.Style.IBasic basic)
            where T : Advice.IBoundary, new()
        {
            //TODO optimize by introspecting T to disable boundary call if not requiered
            //TODO missing dispose call!
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.ReturnType();
                var _parameters = _Method.GetParameters();
                var _signature = _Method.Signature();
                var _routine = new Closure.Routine(_Pointer, _signature, _type);
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                var _boundary = _body.DeclareLocal(Runtime<T>.Type);
                var _backup = _body.DeclareLocal(Runtime<Exception>.Type);
                var _exception = _body.DeclareLocal(Runtime<Exception>.Type);
                _body.Emit(OpCodes.Newobj, Runtime.Constructor(() => new T()));
                _body.Emit(OpCodes.Stloc_0);
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Ldsfld, _Method is MethodInfo ? Runtime.Inventory.Method(_Method as MethodInfo) : Runtime.Inventory.Constructor(_Method as ConstructorInfo));
                _body.Emit(OpCodes.Ldsfld, Runtime.Inventory.Signature(_Method));
                _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Method(Runtime<MethodBase>.Value, Runtime<ParameterInfo[]>.Value)));
                if (_Method.IsStatic)
                {
                    for (var _index = 0; _index < _signature.Length; _index++)
                    {
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldsfld, Runtime.Inventory.Parameter(_parameters[_index]));
                        _body.Emit(OpCodes.Ldarga_S, _index);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Argument(Runtime<ParameterInfo>.Value, ref Runtime<object>.Value)));
                    }
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Begin()));
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    if (_type == Runtime.Void)
                    {
                        var _return = _body.DefineLabel();
                        var _null = _body.DefineLabel();
                        var _rethrow = _body.DefineLabel();
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Return()));
                        _body.Emit(OpCodes.Leave, _return);
                        _body.BeginCatchBlock(Runtime<Exception>.Type);
                        _body.Emit(OpCodes.Stloc_1);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Stloc_2);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldloca_S, 2);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Throw(ref Runtime<Exception>.Value)));
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Brfalse, _null);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Beq, _rethrow);
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Throw);
                        _body.MarkLabel(_rethrow);
                        _body.Emit(OpCodes.Rethrow);
                        _body.MarkLabel(_null);
                        _body.Emit(OpCodes.Leave, _return);
                        _body.EndExceptionBlock();
                        _body.MarkLabel(_return);
                        _body.Emit(OpCodes.Ret);
                    }
                    else
                    {
                        var _return = _body.DefineLabel();
                        var _null = _body.DefineLabel();
                        var _rethrow = _body.DefineLabel();
                        _body.DeclareLocal(_type);
                        _body.Emit(OpCodes.Stloc_3);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldloca_S, 3);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Return(ref Runtime<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_type));
                        _body.Emit(OpCodes.Leave, _return);
                        _body.BeginCatchBlock(Runtime<Exception>.Type);
                        _body.Emit(OpCodes.Stloc_1);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Stloc_2);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldloca_S, 2);
                        _body.Emit(OpCodes.Ldloca_S, 3);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Throw(ref Runtime<Exception>.Value, ref Runtime<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod());
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Brfalse, _null);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Beq, _rethrow);
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Throw);
                        _body.MarkLabel(_rethrow);
                        _body.Emit(OpCodes.Rethrow);
                        _body.MarkLabel(_null);
                        _body.Emit(OpCodes.Leave, _return);
                        _body.EndExceptionBlock();
                        _body.MarkLabel(_return);
                        _body.Emit(OpCodes.Ldloc_3);
                        _body.Emit(OpCodes.Ret);
                    }
                }
                else
                {
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Instance(Runtime<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_Method.DeclaringType));
                    for (var _index = 0; _index < _parameters.Length; _index++)
                    {
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldsfld, Runtime.Inventory.Parameter(_parameters[_index]));
                        _body.Emit(OpCodes.Ldarga_S, _index + 1);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Argument(Runtime<ParameterInfo>.Value, ref Runtime<object>.Value)));
                    }
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Begin()));
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    if (_type == Runtime.Void)
                    {
                        var _return = _body.DefineLabel();
                        var _null = _body.DefineLabel();
                        var _rethrow = _body.DefineLabel();
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Return()));
                        _body.Emit(OpCodes.Leave, _return);
                        _body.BeginCatchBlock(Runtime<Exception>.Type);
                        _body.Emit(OpCodes.Stloc_1);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Stloc_2);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldloca_S, 2);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Throw(ref Runtime<Exception>.Value)));
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Brfalse, _null);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Beq, _rethrow);
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Throw);
                        _body.MarkLabel(_rethrow);
                        _body.Emit(OpCodes.Rethrow);
                        _body.MarkLabel(_null);
                        _body.Emit(OpCodes.Leave, _return);
                        _body.EndExceptionBlock();
                        _body.MarkLabel(_return);
                        _body.Emit(OpCodes.Ret);
                    }
                    else
                    {
                        var _return = _body.DefineLabel();
                        var _null = _body.DefineLabel();
                        var _rethrow = _body.DefineLabel();
                        _body.DeclareLocal(_type);
                        _body.Emit(OpCodes.Stloc_3);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldloca_S, 3);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Return(ref Runtime<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_type));
                        _body.Emit(OpCodes.Leave, _return);
                        _body.BeginCatchBlock(Runtime<Exception>.Type);
                        _body.Emit(OpCodes.Stloc_1);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Stloc_2);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldloca_S, 2);
                        _body.Emit(OpCodes.Ldloca_S, 3);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Throw(ref Runtime<Exception>.Value, ref Runtime<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod());
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Brfalse, _null);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Beq, _rethrow);
                        _body.Emit(OpCodes.Ldloc_2);
                        _body.Emit(OpCodes.Throw);
                        _body.MarkLabel(_rethrow);
                        _body.Emit(OpCodes.Rethrow);
                        _body.MarkLabel(_null);
                        _body.Emit(OpCodes.Leave, _return);
                        _body.EndExceptionBlock();
                        _body.MarkLabel(_return);
                        _body.Emit(OpCodes.Ldloc_3);
                        _body.Emit(OpCodes.Ret);
                    }
                }
                _method.Prepare();
                return _method;
            });
        }
    }
}