using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGSTCollisionEditor
{
    internal class PACFile
    {
        char[] MAGIC = { 'F', 'P', 'A', 'C' };
        public uint data_start;
        public uint total_size;
        public bool is_bb = true;
        uint file_count;
        uint string_size;
        private byte[] unk1;
        private byte[] unk2;
        public List<PACEntry> pacentries = new List<PACEntry>();
        public PACFile(Stream stream)
        {
            BinaryReader pacbr = new BinaryReader(stream);
            char[] maybemagic = pacbr.ReadChars(4);
            if (!MAGIC.SequenceEqual(maybemagic))
            {
                pacbr.BaseStream.Seek(0x38, SeekOrigin.Begin);
                maybemagic = pacbr.ReadChars(4);
                if (!MAGIC.SequenceEqual(maybemagic))
                {
                    return;
                }
                else
                {
                    is_bb = false;
                }
            }
            data_start = pacbr.ReadUInt32();
            total_size = pacbr.ReadUInt32();
            file_count = pacbr.ReadUInt32();
            unk1 = pacbr.ReadBytes(4);
            string_size = pacbr.ReadUInt32();
            unk2 = pacbr.ReadBytes(8);
            var entry_size = ((int)data_start - 32) / file_count;
            for (var i = 0; i < file_count; i++)
            {
                PACEntry entry = new PACEntry();
                long start = pacbr.BaseStream.Position;
                byte[] namebytes = pacbr.ReadBytes((int)string_size).TakeWhile(b => (b != 0)).ToArray();
                entry.name = Encoding.UTF8.GetString(namebytes, 0, namebytes.Length);
                pacbr.BaseStream.Seek(4, SeekOrigin.Current);
                entry.offset = pacbr.ReadUInt32();
                entry.size = pacbr.ReadUInt32();
                while (pacbr.BaseStream.Position < start + entry_size)
                {
                    pacbr.BaseStream.Seek(4, SeekOrigin.Current);
                }
                pacentries.Add(entry);
            }
            pacbr.Close();
        }
        public int getOffsetByName(string name)
        {
            var results = pacentries.Where(pe => pe.name.Equals(name));
            if (results.Count() != 0)
            {
                return (int)(results.First().offset + data_start);
            }
            return -1;
        }
        public PACEntry getEntryByName(string name)
        {
            var results = pacentries.Where(pe => pe.name.Equals(name));
            if (results.Count() != 0)
            {
                return results.First();
            }
            return null;
        }
        public class PACEntry : IComparable<PACEntry>
        {
            public uint offset;
            public string name;
            public uint size;
            public override string ToString() => name;

            int IComparable<PACEntry>.CompareTo(PACEntry other)
            {
                if (other != null)
                {
                    return this.offset.CompareTo(other.offset);
                }
                else
                {
                    throw new ArgumentException("Object is not a PACEntry");
                }
            }
        }
    }
}
