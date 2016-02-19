﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host;
using System;
using System.Reflection;

namespace Microsoft.Azure.WebJobs.Script
{
    public static class INameResolverExtensions
    {
        // Apply the resolver to all string properties on obj that have an [AllowNameResolution] attribute.
        // Updates the object in-place. 
        public static void ResolveAllProperties(this INameResolver resolver, object obj)
        {
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType == typeof(string))
                {
                    if (prop.GetCustomAttribute<AllowNameResolutionAttribute>() != null)
                    {
                        string val = (string)prop.GetValue(obj);
                        string newVal = resolver.ResolveWholeString(val);
                        prop.SetValue(obj, newVal);
                    }
                }
            }
        }
    }

}