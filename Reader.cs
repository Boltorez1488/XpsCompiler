using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace XpsCompiler {
    internal class Reader {
        public string FName;

        public Reader(string fname) {
            FName = fname;
        }

        public Scanner Load() {
            var xDoc = XDocument.Load(FName);
            var root = xDoc.Root;
            if (root == null)
                return null;
            return root.Name == "scanner" ? ReadBody(root) : null;
        }

        public Scanner ReadBody(XElement elem) {
            var modBase = elem.Attribute("base")?.Value;
            var scan = new Scanner(elem.Attribute("name")?.Value) {
                ModuleBase = modBase != null ? int.Parse(modBase, NumberStyles.HexNumber) : 0x400000,
                Exe = elem.Attribute("exe")?.Value,
                OutName = elem.Attribute("out")?.Value
            };
            foreach (var xNode in elem.Nodes().Where(x => x.NodeType != XmlNodeType.Comment)) {
                var e = (XElement) xNode;
                if (e.Name == "search")
                    ReadPattern(e, scan);
                else if (e.Name == "block")
                    ReadBlock(e, scan);
                else if (e.Name == "field")
                    ReadField(e, scan);
            }

            return scan;
        }

        public void ReadBlock(XElement elem, Block root) {
            var block = new Block(elem.Attribute("name")?.Value);
            foreach (var xNode in elem.Nodes().Where(x => x.NodeType != XmlNodeType.Comment)) {
                var e = (XElement) xNode;
                if (e.Name == "search")
                    ReadPattern(e, block);
                else if (e.Name == "block")
                    ReadBlock(e, block);
                else if (e.Name == "field")
                    ReadField(e, block);
            }

            root.PushBlock(block);
        }

        public void ReadField(XElement elem, Block block) {
            var address = elem.Attribute("address")?.Value;
            var comment = elem.Attribute("comment")?.Value;
            block.PushField(address, comment);
        }

        public void ReadPattern(XElement elem, Block block) {
            var offset = elem.Attribute("offset")?.Value;
            var last = elem.Attribute("last")?.Value;
            string bytes = elem.Attribute("bytes")?.Value;
            string comment = null;
            foreach (var xNode in elem.Nodes()) {
                if (xNode.NodeType == XmlNodeType.Comment) {
                    comment = ((XComment)xNode).Value;
                    continue;
                }
                var e = (XElement) xNode;
                if (e.Name == "bytes")
                    bytes = e.Value;
                else if (e.Name == "comment") comment = e.Value;
            }

            var lastByte = last?[0] ?? '\0';
            block.PushPattern(offset, bytes, comment, lastByte);
        }
    }
}