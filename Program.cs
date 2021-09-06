using System;
using System.IO;

namespace XpsCompiler {
    internal class Program {
        private static void Main(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("use: <xps_path> <lines>");
                return;
            }

            var file = Path.HasExtension(args[0]) ? args[0] : args[0] + ".xps";

            if (!File.Exists(file)) {
                Console.WriteLine($"[Error]: Input file '{file}' not found");
                return;
            }

            if (args.Length > 1) {
                Constants.Lines = int.Parse(file);
            }

            var reader = new Reader(file);
            var scan = reader.Load();
            scan.StartScan();
            scan.Dump();
        }
    }
}