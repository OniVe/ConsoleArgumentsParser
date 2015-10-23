using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleArgumentsParser
{
    public class ArgumentParser
    {
        private readonly ConsoleActionCollection _consoleActionCollection;
        private string _token;

        public ArgumentParser(char token)
        {
            ValidateToken(token);
            _consoleActionCollection = new ConsoleActionCollection();
        }

        private void ValidateToken(char @char)
        {
            if (char.IsLetterOrDigit(@char)) throw new Exception("Token must be only symbol");
            _token = Convert.ToString(@char);
        }

        public void AddAction(string name, Delegate action) => _consoleActionCollection.Add(name, action);

        private string[] SanitizeArgs(string[] args)
        {
            var commands = new List<string>();
            var enumerator = args.GetEnumerator();
            StringBuilder builder = null;
            while (enumerator.MoveNext())
            {
                var arg = enumerator.Current as string;
                if (arg.StartsWith(_token))
                {
                    if (builder != null) commands.Add(builder.ToString());
                    builder = new StringBuilder();
                    builder.Append(arg);
                }
                else
                {
                    builder.Append($" {arg}");
                }
            }


            commands.Add(builder.ToString());

            return commands.ToArray();
        }

        public void Parse(string[] args)
        {
            var inArgs = args;
            var list = new List<Argument>();
            var pattern = $@"^(?<key>(?<token>\{_token})\w+)(\s|)(?<args>.*?$)";
            inArgs = SanitizeArgs(inArgs);
            foreach (var arg in inArgs)
            {
                var match = Regex.Match(arg, pattern);
                if (!match.Success) continue;
                var key = match.Groups["key"].Value.Replace(match.Groups["token"].Value, "");
                var arguments = match.Groups["args"].Value;
                var argument = new Argument {Key = key, Arguments = arguments};
                list.Add(argument);
            }

            foreach (var argument in list)
            {
                var method = _consoleActionCollection.GetAction(argument.Key);
                method.DynamicInvoke(argument.HaveArgument()
                    ? argument.ConvertArguments(_consoleActionCollection.GetTypes(argument.Key))
                    : null);
            }
        }
    }

    internal struct Argument
    {
        public string Key { get; set; }
        public string Arguments { get; set; }
        // Thanks FailedDev(http://stackoverflow.com/users/880096/faileddev) for regex (http://stackoverflow.com/questions/7804851/regex-to-match-a-path-in-c-sharp)
        private static readonly Regex PathRegex =
            new Regex(@"(([a-zA-Z]:|\\\\[a-z0-9_.$]+\\[a-z0-9_.$]+)?(\\?(?:[^\\/:*?""<>|\r\n]+\\)+)[^\\/:*?""<>|\r\n]+)");

        public bool HaveArgument()
        {
            return Arguments != null;
        }

        public object[] ConvertArguments(Type[] types)
        {
            if (types.Length == 0) return null;

            var list = new List<object>();

            var thereIsPath = PathRegex.IsMatch(Arguments);
            var argument = PathRegex.Replace(Arguments, "").Replace("\"", "").Trim();
            var args = argument.Split(" ".ToCharArray());

            var enumerator = args.GetEnumerator();
            var typeIndex = 0;

            while (enumerator.MoveNext())
            {
                var type = types[typeIndex];
                var obj = enumerator.Current as string;
                if (CanConvert(obj, type))
                {
                    if (type == typeof (string) && thereIsPath)
                    {
                        list.Add(Convert.ChangeType(PathRegex.Match(Arguments).Value, type));
                    }
                    else
                    {
                        list.Add(Convert.ChangeType(obj, type));
                    }
                    typeIndex++;
                }
            }
            return list.ToArray();
        }

        private bool CanConvert(string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            return converter.IsValid(value);
        }
    }
}