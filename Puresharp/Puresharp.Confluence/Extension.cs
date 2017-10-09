﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp.Confluence
{
    abstract public class Extension
    {
        abstract public IEnumerable<Advice> Advise(MethodBase method);
    }
}