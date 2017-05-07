using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __LambdaExpression
    {
        static private class Compiler
        {
            static private readonly Type m_Type = Runtime<Expression>.Type.Assembly.GetType("System.Linq.Expressions.Compiler.LambdaCompiler");
            static private readonly FieldInfo m_Method = Compiler.m_Type.GetField("_method", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            static private readonly FieldInfo m_Body = Compiler.m_Type.GetField("_ilg", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            static private readonly FieldInfo m_Closure = Compiler.m_Type.GetField("_hasClosureArgument", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            static private readonly MethodInfo m_Analyze = Compiler.m_Type.GetMethod("AnalyzeLambda", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            static private readonly Type m_Tree = Runtime<Expression>.Type.Assembly.GetType("System.Linq.Expressions.Compiler.AnalyzedTree");
            static private readonly MethodInfo m_Signature = Compiler.m_Type.GetMethod("GetParameterTypes", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            static private readonly MethodInfo m_Compile = Compiler.m_Type.GetMethod("EmitLambdaBody", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);

            static public Func<LambdaExpression, DynamicMethod> Compile()
            {
                var _method = new DynamicMethod(string.Empty, Runtime<DynamicMethod>.Type, new Type[] { Runtime<object>.Type, Runtime<LambdaExpression>.Type }, true);
                var _body = _method.GetILGenerator();
                _body.DeclareLocal(Compiler.m_Type);
                _body.Emit(OpCodes.Ldarga_S, 1);
                _body.Emit(OpCodes.Call, Compiler.m_Analyze);
                _body.Emit(OpCodes.Ldarg_1);
                _body.Emit(OpCodes.Newobj, Compiler.m_Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { Compiler.m_Tree, Runtime<LambdaExpression>.Type }, null));
                _body.Emit(OpCodes.Stloc_0);
                _body.Emit(OpCodes.Ldsfld, Runtime.Field(() => Compiler.m_Method));
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Castclass, Runtime<object>.Type);
                _body.Emit(OpCodes.Ldsfld, Runtime.Field(() => string.Empty));
                _body.Emit(OpCodes.Ldarg_1);
                _body.Emit(OpCodes.Call, Runtime<LambdaExpression>.Property(_Lambda => _Lambda.ReturnType).GetGetMethod(true));
                _body.Emit(OpCodes.Ldarg_1);
                _body.Emit(OpCodes.Call, Compiler.m_Signature);
                _body.Emit(OpCodes.Ldc_I4_1);
                _body.Emit(OpCodes.Newobj, Runtime.Constructor(() => new DynamicMethod(Runtime<string>.Value, Runtime<Type>.Value, Runtime<Type[]>.Value, Runtime<bool>.Value)));
                _body.Emit(OpCodes.Castclass, Runtime<object>.Type);
                _body.Emit(OpCodes.Call, Runtime<FieldInfo>.Method(_Field => _Field.SetValue(Runtime<object>.Value, Runtime<object>.Value)));
                _body.Emit(OpCodes.Ldsfld, Runtime.Field(() => Compiler.m_Body));
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Castclass, Runtime<object>.Type);
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Ldfld, Compiler.m_Method);
                _body.Emit(OpCodes.Castclass, Runtime<DynamicMethod>.Type);
                _body.Emit(OpCodes.Call, Runtime<DynamicMethod>.Method(_Method => _Method.GetILGenerator()));
                _body.Emit(OpCodes.Castclass, Runtime<object>.Type);
                _body.Emit(OpCodes.Call, Runtime<FieldInfo>.Method(_Field => _Field.SetValue(Runtime<object>.Value, Runtime<object>.Value)));
                _body.Emit(OpCodes.Ldsfld, Runtime.Field(() => Compiler.m_Closure));
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Castclass, Runtime<object>.Type);
                _body.Emit(OpCodes.Ldc_I4_0);
                _body.Emit(OpCodes.Box, Runtime<bool>.Type);
                _body.Emit(OpCodes.Call, Runtime<FieldInfo>.Method(_Field => _Field.SetValue(Runtime<object>.Value, Runtime<object>.Value)));
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Call, Compiler.m_Compile);
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Ldfld, Compiler.m_Method);
                _body.Emit(OpCodes.Castclass, Runtime<DynamicMethod>.Type);
                _body.Emit(OpCodes.Ret);
                return _method.CreateDelegate(Runtime<Func<LambdaExpression, DynamicMethod>>.Type, null) as Func<LambdaExpression, DynamicMethod>;
            }
        }

        static private readonly Func<DynamicMethod, RuntimeMethodHandle> m_Handle = Delegate.CreateDelegate(Runtime<Func<DynamicMethod, RuntimeMethodHandle>>.Type, Runtime<DynamicMethod>.Type.GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic)) as Func<DynamicMethod, RuntimeMethodHandle>;
        static internal readonly Func<LambdaExpression, DynamicMethod> m_Compile = Compiler.Compile();

        static public DynamicMethod CompileToMethod(this LambdaExpression lambda)
        {
            return __LambdaExpression.m_Compile(lambda);
        }
    }
}
