using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Nett;

namespace CodeRinseRepeat.Configuration.TomlConfigurationProvider
{
    internal class TomlConfigurationFileParser : ITomlObjectVisitor
    {
        readonly IDictionary<string, string> data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly Stack<string> context = new Stack<string>();
        string currentPath;

        public IDictionary<string, string> Parse(Stream stream)
        {
            data.Clear();
            context.Clear();

            Toml.ReadStream(stream).Visit(this);

            return data;
        }

        void PushContext(string context)
        {
            this.context.Push(context);
            currentPath = ConfigurationPath.Combine(this.context.Reverse());
        }

        void PopContext()
        {
            context.Pop();
            currentPath = ConfigurationPath.Combine(this.context.Reverse());
        }

        public void Visit(TomlTable table)
        {
            foreach (var row in table.Rows) {
                PushContext(row.Key);
                row.Value.Visit(this);
                PopContext();
            }
        }

        public void Visit(TomlTableArray tableArray)
        {
            for (int i = 0; i < tableArray.Count; i++) {
                PushContext(i.ToString());
                tableArray[i].Visit(this);
                PopContext();
            }
        }

        public void Visit(TomlInt i) => data[currentPath] = i.Value.ToString(CultureInfo.InvariantCulture);

        public void Visit(TomlFloat f) => data[currentPath] = f.Value.ToString(CultureInfo.InvariantCulture);

        public void Visit(TomlBool b) => data[currentPath] = b.Value.ToString(CultureInfo.InvariantCulture);

        public void Visit(TomlString s) => data[currentPath] = s.Value;

        // c is the default format provider, per MSDN.
        public void Visit(TomlTimeSpan ts) => data[currentPath] = ts.Value.ToString("c", CultureInfo.InvariantCulture);

        public void Visit(TomlDateTime dt) => data[currentPath] = dt.Value.ToString(CultureInfo.InvariantCulture);

        public void Visit(TomlArray a)
        {
            for (int i = 0; i < a.Length; i++) {
                PushContext(i.ToString());
                a[i].Visit(this);
                PopContext();
            }
        }
    }
}
