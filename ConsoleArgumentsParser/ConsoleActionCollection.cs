using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleArgumentsParser
{
    internal interface IConsoleActionCollection
    {
        void Add(string name, Delegate action);
        Delegate GetAction(string name);
        Type[] GetTypes(string name);
    }

    internal class ConsoleActionCollection : IConsoleActionCollection
    {
        private readonly Dictionary<string, Delegate> _dic;

        public ConsoleActionCollection()
        {
            _dic = new Dictionary<string, Delegate>();
        }

        public void Add(string name, Delegate action)
        {
            _dic.SecureAdd(name, action);
        }

        public Delegate GetAction(string name)
        {
            return _dic[name];
        }

        public Type[] GetTypes(string name)
        {
            return (from parameter in _dic[name].Method.GetParameters() select parameter.ParameterType).ToArray();
        }
    }

    internal static class CollectionExtension
    {
        public static void SecureAdd(this IDictionary dic, string name, object obj)
        {
            if (dic.Contains(name)) throw new Exception("Action alredy exists");
            dic.Add(name, obj);
        }
    }
}