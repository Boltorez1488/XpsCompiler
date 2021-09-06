using System;
using System.Collections.Generic;
using System.Globalization;

namespace XpsCompiler {
    internal class Pattern {
        public int Address;
        public byte[] Bytes;
        public string Comment;
        public char LastByte;
        public string Mask;
        public int Offset;

        public Pattern(int offset, string bytes, string comment, char lastByte) {
            Offset = offset;
            Mask = "";
            var split = bytes.Split(' ');
            var list = new List<byte>();
            foreach (var s in split)
                if (s != "?") {
                    list.Add(byte.Parse(s, NumberStyles.HexNumber));
                    Mask += "x";
                } else {
                    list.Add(0x0);
                    Mask += "?";
                }

            Bytes = list.ToArray();
            Comment = comment;
            LastByte = lastByte;
            Address = 0x0;
        }

        public void Convert(List<string> build, int tabs = 0) {
            if (Address != 0x0 && LastByte != '\0') {
                var strA = (Address + Offset).ToString("X8");
                var check = strA[strA.Length - 1];
                var bt = LastByte;
                if (check != bt)
                    Console.WriteLine(Comment != null
                        ? $"[Warning]: Last byte check fail - {bt} != {strA}, '{Comment}'"
                        : $"[Warning]: Last byte check fail - {bt} != {strA}");
            }

            var tab = Block.TabsConvert(tabs);
            var adr = Address != 0x0 ? (Address + Offset).ToString("X8") : "???";
            if (Comment != null)
                build.Add(tab + $"{adr} - {Comment}");
            else
                build.Add(tab + $"{adr}");
        }
    }
}