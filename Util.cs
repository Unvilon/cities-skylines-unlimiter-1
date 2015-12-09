﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EightyOne
{
    internal static class Util
    {
        private const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        public static void CopyArray(IList newArray, object em, string propertyName)
        {
            var oldArray = (IList)em.GetType().GetField(propertyName, AllFlags).GetValue(em);
            for (var i = 0; i < newArray.Count; i += 1)
            {
                newArray[i] = oldArray[i];
            }
        }

        public static void CopyArrayBack(IList newArray, object em, string propertyName)
        {
            var oldArray = (IList)em.GetType().GetField(propertyName, AllFlags).GetValue(em);
            for (var i = 0; i < newArray.Count; i += 1)
            {
                oldArray[i] = newArray[i];
            }
        }

        public static void CopyStructArray(IList newArray, object em, string propertyName)
        {
            var oldArray = (IList)em.GetType().GetField(propertyName, AllFlags).GetValue(em);
            var fields = GetFieldsFromStruct(newArray[0], oldArray[0]);
            for (var i = 0; i < newArray.Count; i += 1)
            {
                newArray[i] = CopyStruct((object)newArray[0], oldArray[i], fields);
            }
        }

        public static void CopyStructArrayBack(IList newArray, object em, string propertyName)
        {
            var oldArray = (IList)em.GetType().GetField(propertyName, AllFlags).GetValue(em);
            var fields = GetFieldsFromStruct(oldArray[0], newArray[0]);
            for (var i = 0; i < newArray.Count; i += 1)
            {
                oldArray[i] = CopyStruct((object)oldArray[i], newArray[i], fields);
            }
        }


        public static Dictionary<FieldInfo, FieldInfo> GetFieldsFromStruct(object newArray, object oldArray)
        {
            var fields = new Dictionary<FieldInfo, FieldInfo>();
            foreach (var f in oldArray.GetType().GetFields(AllFlags))
            {
                fields.Add(newArray.GetType().GetField(f.Name, AllFlags), f);
            }
            return fields;
        }

        public static void SetPropertyValue<T>(ref T result, object obj, string propName)
        {
            result = (T)obj.GetType().GetField(propName, AllFlags).GetValue(obj);
        }

        public static void SetPropertyValueBack(object result, object obj, string propName)
        {
            obj.GetType().GetField(propName, AllFlags).SetValue(obj, result);
        }

        public static object CopyStruct(object newObj, object original, Dictionary<FieldInfo, FieldInfo> fields)
        {
            foreach (var field in fields)
            {
                if (field.Key.FieldType != field.Value.FieldType)
                {
                    if (field.Key.FieldType == typeof(byte))
                    {
                        var oo = Mathf.Clamp((ushort)field.Value.GetValue(original), 0, 255);
                        field.Key.SetValue(newObj, (byte)oo);
                        continue;
                    }
                }
                field.Key.SetValue(newObj, field.Value.GetValue(original));
            }
            return newObj;
        }
    }
}