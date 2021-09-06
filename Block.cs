using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace XpsCompiler {
    internal class Field {
        public string Address;
        public Block Block;
        public string Comment;
        public Pattern Pattern;
    }

    internal class Block {
        public readonly List<Field> Fields = new List<Field>();
        public string Name;

        public Block(string name) {
            Name = name;
        }

        public void PushPattern(string offset, string bytes, string comment, char lastByte = '\0') {
            char[] symbols = {' ', '\t'};
            bytes = bytes.Trim(symbols);
            comment = comment.Trim(symbols);
            var field = new Field {
                Pattern = new Pattern(offset != null ? int.Parse(offset, NumberStyles.HexNumber) : 0, bytes, comment,
                    lastByte)
            };
            Fields.Add(field);
        }

        public void PushBlock(Block block) {
            var field = new Field {Block = block};
            Fields.Add(field);
        }

        public void PushField(string address, string comment) {
            Fields.Add(new Field {Address = address, Comment = comment});
        }

        public void StartScan(Memory mem, int modBase) {
            foreach (var field in Fields)
                if (field.Block != null) {
                    field.Block.StartScan(mem, modBase);
                } else if (field.Pattern != null) {
                    var search = mem.SearchPattern(field.Pattern.Bytes, field.Pattern.Mask);
                    if (search != 0x0)
                        field.Pattern.Address = search + modBase;
                }
        }

        public static string TabsConvert(int tabs) {
            var result = "";
            for (var i = 0; i < tabs; i++)
                result += '\t';
            return result;
        }

        public static string ToString(List<string> lines) {
            var result = "";
            foreach (var line in lines)
                result += line + "\n";
            return result;
        }

        public static void TrimEnd(List<string> build) {
            var count = 0;
            for (var i = build.Count - 1; i > 0; i--)
                if (build[i] == "")
                    count++;
                else
                    break;
            build.RemoveRange(build.Count - count, count);
        }

        public void Convert(List<string> build, int tabs = 0) {
            var tab = TabsConvert(tabs);
            if (string.IsNullOrEmpty(Name)) {
                foreach (var field in Fields) {
                    if (field.Pattern != null) {
                        field.Pattern.Convert(build, tabs);
                    } else if (field.Block != null) {
                        field.Block.Convert(build, tabs);
                    } else {
                        if (field.Comment != null)
                            build.Add(tab + $"{field.Address} - {field.Comment}");
                        else
                            build.Add(tab + $"{field.Address}");
                    }
                }
                return;
            }

            if (build.Count != 0 && build.Last() != "")
                for (var i = 0; i < Constants.Lines; i++)
                    build.Add("");
            build.Add(tab + $"[{Name}]: " + "{");
            foreach (var field in Fields)
                if (field.Pattern != null) {
                    field.Pattern.Convert(build, tabs + 1);
                } else if (field.Block != null) {
                    field.Block.Convert(build, tabs + 1);
                } else {
                    if (field.Comment != null)
                        build.Add(TabsConvert(tabs + 1) + $"{field.Address} - {field.Comment}");
                    else
                        build.Add(TabsConvert(tabs + 1) + $"{field.Address}");
                }

            if(build.Last() == "")
                TrimEnd(build);
            build.Add(tab + "}");
            for (var i = 0; i < Constants.Lines; i++)
                build.Add("");
        }
    }
}