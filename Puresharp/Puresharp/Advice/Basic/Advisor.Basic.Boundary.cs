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
            // create a method to wrap entire call!
            // create a structure to store arguments, exception & return and implements IBefore/IAfter
            // (1) store as local the new structure
            // (2) on start instantiate Boundary with newobj if it is a class.
            // (3) call before first and give structure as argument
            // (4) try catch finally => call after in finally and check if exception != null => throw it or return value from structure!
            // voilà!



            // for async method : find AsyncMachineStructure
            // find field where IBoundary.Factory is stored.
            // replace factory! by Factory<T>

            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.ReturnType();
                var _signature = _Method.Signature();
                var _routine = new Closure.Routine(_Pointer, _signature, _type);
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                var _boundary = _body.DeclareLocal(Runtime<T>.Type);
                _body.Emit(OpCodes.Newobj, Runtime.Constructor(() => new T()));
                _body.Emit(OpCodes.Stloc_0);
                _body.Emit(OpCodes.Ldloc_0);
                //
                _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Method(Runtime<MethodBase>.Value, Runtime<ParameterInfo[]>.Value)));
                if (_Method.IsStatic)
                {
                    for (var _index = 0; _index < _signature.Length; _index++)
                    {
                        _body.Emit(OpCodes.Ldloc_0);
                        switch (_index)
                        {
                            case 0: _body.Emit(OpCodes.Ldc_I4_0); break;
                            case 1: _body.Emit(OpCodes.Ldc_I4_1); break;
                            case 2: _body.Emit(OpCodes.Ldc_I4_2); break;
                            case 3: _body.Emit(OpCodes.Ldc_I4_3); break;
                            case 4: _body.Emit(OpCodes.Ldc_I4_4); break;
                            case 5: _body.Emit(OpCodes.Ldc_I4_5); break;
                            case 6: _body.Emit(OpCodes.Ldc_I4_6); break;
                            case 7: _body.Emit(OpCodes.Ldc_I4_7); break;
                            case 8: _body.Emit(OpCodes.Ldc_I4_8); break;
                            default: _body.Emit(OpCodes.Ldc_I4_S, _index); break;
                        }
                        _body.Emit(OpCodes.Ldarga_S, _index);
                        _body.Emit(OpCodes.Callvirt, Runtime<Advice.IBoundary>.Method(_Boundary => _Boundary.Argument(Runtime<int>.Value, Runtime<ParameterInfo>.Value, ref Runtime<object>.Value)));
                    }
                }
                else
                {
                }

                if (_type == Runtime.Void)
                {
                    _body.Emit(OpCodes.Newobj
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Newobj, _routine.Constructor);
                    _body.Emit(OpCodes.Ldftn, _routine.Method);
                    _body.Emit(OpCodes.Newobj, Runtime<Action>.Type.GetConstructors().Single());
                    _body.Emit(OpCodes.Call, advice.Method);
                }
                else
                {
                    _body.DeclareLocal(_routine.Type);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Newobj, _routine.Constructor);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Ldftn, _routine.Method);
                    _body.Emit(OpCodes.Newobj, Runtime<Action>.Type.GetConstructors().Single());
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Ldfld, _routine.Value);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}