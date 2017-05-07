﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Puresharp
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __ParameterInfo
    {
        static public string Declaration(this ParameterInfo parameter)
        {
            return string.Concat(parameter.ParameterType.Declaration(), " ", parameter.Name);
        }

        static public bool Attributed<T>(this ParameterInfo parameter)
            where T : Attribute, new()
        {
            return System.Attribute.IsDefined(parameter, Runtime<T>.Type);
        }

        static public T Attribute<T>(this ParameterInfo parameter)
            where T : Attribute
        {
            return System.Attribute.GetCustomAttribute(parameter, Runtime<T>.Type) as T;
        }

        static public IEnumerable<T> Attributes<T>(this ParameterInfo parameter)
            where T : Attribute
        {
            return System.Attribute.GetCustomAttributes(parameter, Runtime<T>.Type).Cast<T>();
        }
    }
}