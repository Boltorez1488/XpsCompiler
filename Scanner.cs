using System;
using System.Collections.Generic;
using System.IO;

namespace XpsCompiler {
    internal class Scanner : Block {
        public string Exe;
        public int ModuleBase;
        public string OutName;

        public Scanner(string name = null) : base(name) {
        }

        public void StartScan() {
            var mem = new Memory(Exe);
            StartScan(mem, ModuleBase);
        }

        public void Dump() {
            var pos = OutName.IndexOf("{exe}", StringComparison.OrdinalIgnoreCase);
            if (pos != -1) {
                var exeName = Path.GetFileNameWithoutExtension(Exe);
                OutName = OutName.Remove(pos, 5).Insert(pos, exeName ?? throw new InvalidOperationException());
            }

            var build = new List<string>();
            Convert(build);
            TrimEnd(build);

            using (var stream = new StreamWriter(new FileStream(OutName, FileMode.Create))) {
                for (var i = 0; i < build.Count; i++)
                    if (i == build.Count - 1)
                        stream.Write(build[i]);
                    else
                        stream.WriteLine(build[i]);
            }
        }
    }
}