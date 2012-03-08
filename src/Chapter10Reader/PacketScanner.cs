using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chapter10Reader
{
    public sealed class PacketScanner : IDisposable
    {
        private readonly FileStream stream;

        private PacketScanner(FileStream stream)
        {
            this.stream = stream;
        }

        public static PacketScanner OpenFile(string filename)
        {
            return new PacketScanner(File.OpenRead(filename));
        }

        public void Dispose()
        {
            stream.Dispose();
            stream = null;
        }
    }
}
