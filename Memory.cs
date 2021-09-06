using System.IO;

namespace XpsCompiler {
    internal class Memory {
        private readonly byte[] _bytes;

        public Memory(string exeName) {
            _bytes = File.ReadAllBytes(exeName);
        }

        public int SearchPattern(byte[] bytes, string mask) {
            for (var i = 0; i < _bytes.Length; i++) {
                var match = 0;
                for (int j = i, k = 0; k < bytes.Length && j < _bytes.Length; j++, k++)
                    if (mask[k] == '?')
                        match++;
                    else if (bytes[k] == _bytes[j])
                        match++;
                    else
                        break;

                if (match == bytes.Length)
                    return i;
            }

            return 0x0;
        }
    }
}