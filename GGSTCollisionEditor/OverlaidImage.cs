using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GGSTCollisionEditor
{
    internal class OverlaidImage
    {
        const string JONBIN_HEADER = "JONB";
        uint hitboxcount;
        uint hurtboxcount;
        public List<JonbinBox> hurtboxes;
        public List<JonbinBox> hitboxes;
        public List<JonbinChunk> chunks;

        public OverlaidImage(StorageFile file, int coloffset)
        {
            parseJONB(file, coloffset);
        }

        public void AddHurtbox(StorageFile file, int coloffset)
        {
            BinaryReader jonbr = new BinaryReader(new FileStream(file.Path, FileMode.Open));
            BinaryWriter jonbw = new BinaryWriter(new FileStream(file.Path + ".tmp", FileMode.Create));
            jonbr.BaseStream.Seek((long)coloffset, SeekOrigin.Begin);
            if (!jonbr.ReadChars(4).SequenceEqual(JONBIN_HEADER.ToCharArray()))
            {
                return;
            }
            short imcount = jonbr.ReadInt16();
            for (var i = 0; i < imcount; i++)
            {
                var strbytes = jonbr.ReadBytes(0x20).TakeWhile(b => (b != 0)).ToArray();
            }
            jonbr.BaseStream.Seek(3, SeekOrigin.Current);
            var chunkcount = jonbr.ReadUInt32();

            int firstPos = (int)jonbr.BaseStream.Position;
            hurtboxcount = (uint)jonbr.ReadInt16();
            hitboxcount = (uint)jonbr.ReadInt16();

            chunks = new List<JonbinChunk>();
            hurtboxes = new List<JonbinBox>();
            hitboxes = new List<JonbinBox>();

            jonbr.BaseStream.Seek(41 * 2 + 2, SeekOrigin.Current);
            if (imcount > 0)
            {
                for (var c = 0; c < chunkcount; c++)
                {
                    var chunk = new JonbinChunk();
                    chunk.SrcX = jonbr.ReadSingle();
                    chunk.SrcY = jonbr.ReadSingle();
                    chunk.SrcWidth = jonbr.ReadSingle();
                    chunk.SrcHeight = jonbr.ReadSingle();
                    chunk.DestX = jonbr.ReadSingle();
                    chunk.DestY = jonbr.ReadSingle();
                    chunk.DestWidth = jonbr.ReadSingle();
                    chunk.DestHeight = jonbr.ReadSingle();
                    jonbr.BaseStream.Seek(0x20, SeekOrigin.Current);
                    chunk.Layer = jonbr.ReadInt32();
                    jonbr.BaseStream.Seek(0xC, SeekOrigin.Current);
                    chunks.Add(chunk);
                }
            }
            for (var h = 0; h < hurtboxcount; h++)
            {
                var hurt = new JonbinBox();
                hurt.id = jonbr.ReadInt32();
                hurt.x = jonbr.ReadSingle();
                hurt.y = jonbr.ReadSingle();
                hurt.width = jonbr.ReadSingle();
                hurt.height = jonbr.ReadSingle();
                hurtboxes.Add(hurt);
            }

            int secondPos = (int)jonbr.BaseStream.Position;
            jonbr.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] firstData = jonbr.ReadBytes(secondPos);

            jonbw.Write(firstData, 0, firstData.Length);

            jonbw.Seek(firstPos, SeekOrigin.Begin);
            hurtboxcount++;
            jonbw.Write(hurtboxcount);

            jonbw.Seek(0, SeekOrigin.End);

            jonbw.Write((Int32)0);
            jonbw.Write((Single)0);
            jonbw.Write((Single)0);
            jonbw.Write((Single)0);
            jonbw.Write((Single)0);

            byte[] secondData = jonbr.ReadBytes((int)jonbr.BaseStream.Length - secondPos);

            jonbw.Write(secondData, 0, secondData.Length);

            for (var h = 0; h < hitboxcount; h++)
            {
                var hit = new JonbinBox();
                hit.id = jonbr.ReadInt32();
                hit.x = jonbr.ReadSingle();
                hit.y = jonbr.ReadSingle();
                hit.width = jonbr.ReadSingle();
                hit.height = jonbr.ReadSingle();
                hitboxes.Add(hit);
            }

            jonbr.Close();
            jonbw.Close();

        }
        public void AddHitbox(StorageFile file, int coloffset)
        {
            BinaryReader jonbr = new BinaryReader(new FileStream(file.Path, FileMode.Open));
            BinaryWriter jonbw = new BinaryWriter(new FileStream(file.Path + ".tmp", FileMode.Create));
            jonbr.BaseStream.Seek((long)coloffset, SeekOrigin.Begin);
            if (!jonbr.ReadChars(4).SequenceEqual(JONBIN_HEADER.ToCharArray()))
            {
                return;
            }
            short imcount = jonbr.ReadInt16();
            for (var i = 0; i < imcount; i++)
            {
                var strbytes = jonbr.ReadBytes(0x20).TakeWhile(b => (b != 0)).ToArray();
            }
            jonbr.BaseStream.Seek(3, SeekOrigin.Current);
            var chunkcount = jonbr.ReadUInt32();

            hurtboxcount = (uint)jonbr.ReadInt16();
            int firstPos = (int)jonbr.BaseStream.Position;
            hitboxcount = (uint)jonbr.ReadInt16();

            chunks = new List<JonbinChunk>();
            hurtboxes = new List<JonbinBox>();
            hitboxes = new List<JonbinBox>();

            jonbr.BaseStream.Seek(41 * 2 + 2, SeekOrigin.Current);
            if (imcount > 0)
            {
                for (var c = 0; c < chunkcount; c++)
                {
                    var chunk = new JonbinChunk();
                    chunk.SrcX = jonbr.ReadSingle();
                    chunk.SrcY = jonbr.ReadSingle();
                    chunk.SrcWidth = jonbr.ReadSingle();
                    chunk.SrcHeight = jonbr.ReadSingle();
                    chunk.DestX = jonbr.ReadSingle();
                    chunk.DestY = jonbr.ReadSingle();
                    chunk.DestWidth = jonbr.ReadSingle();
                    chunk.DestHeight = jonbr.ReadSingle();
                    jonbr.BaseStream.Seek(0x20, SeekOrigin.Current);
                    chunk.Layer = jonbr.ReadInt32();
                    jonbr.BaseStream.Seek(0xC, SeekOrigin.Current);
                    chunks.Add(chunk);
                }
            }
            for (var h = 0; h < hurtboxcount; h++)
            {
                var hurt = new JonbinBox();
                hurt.id = jonbr.ReadInt32();
                hurt.x = jonbr.ReadSingle();
                hurt.y = jonbr.ReadSingle();
                hurt.width = jonbr.ReadSingle();
                hurt.height = jonbr.ReadSingle();
                hurtboxes.Add(hurt);
            }
            for (var h = 0; h < hitboxcount; h++)
            {
                var hit = new JonbinBox();
                hit.id = jonbr.ReadInt32();
                hit.x = jonbr.ReadSingle();
                hit.y = jonbr.ReadSingle();
                hit.width = jonbr.ReadSingle();
                hit.height = jonbr.ReadSingle();
                hitboxes.Add(hit);
            }

            int secondPos = (int)jonbr.BaseStream.Position;
            jonbr.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] firstData = jonbr.ReadBytes(secondPos);

            jonbw.Write(firstData, 0, firstData.Length);

            jonbw.Seek(firstPos, SeekOrigin.Begin);
            hitboxcount++;
            jonbw.Write(hitboxcount);

            jonbw.Seek(0, SeekOrigin.End);

            jonbw.Write((Int32)1);
            jonbw.Write((Single)0);
            jonbw.Write((Single)0);
            jonbw.Write((Single)0);
            jonbw.Write((Single)0);

            byte[] secondData = jonbr.ReadBytes((int)jonbr.BaseStream.Length - secondPos);

            jonbw.Write(secondData, 0, secondData.Length);
            jonbr.Close();
            jonbw.Close();
        }
        public bool RemoveHurtbox(StorageFile file, int coloffset)
        {
            BinaryReader jonbr = new BinaryReader(new FileStream(file.Path, FileMode.Open));
            BinaryWriter jonbw = new BinaryWriter(new FileStream(file.Path + ".tmp", FileMode.Create));
            jonbr.BaseStream.Seek((long)coloffset, SeekOrigin.Begin);
            if (!jonbr.ReadChars(4).SequenceEqual(JONBIN_HEADER.ToCharArray()))
            {
                return false;
            }
            short imcount = jonbr.ReadInt16();
            for (var i = 0; i < imcount; i++)
            {
                var strbytes = jonbr.ReadBytes(0x20).TakeWhile(b => (b != 0)).ToArray();
            }
            jonbr.BaseStream.Seek(3, SeekOrigin.Current);
            var chunkcount = jonbr.ReadUInt32();

            int firstPos = (int)jonbr.BaseStream.Position;
            hurtboxcount = (uint)jonbr.ReadInt16();
            hitboxcount = (uint)jonbr.ReadInt16();

            if (hurtboxcount == 0)
            {
                jonbr.Close();
                jonbw.Close();
                return false;
            }
            hurtboxcount--;

            chunks = new List<JonbinChunk>();
            hurtboxes = new List<JonbinBox>();
            hitboxes = new List<JonbinBox>();

            jonbr.BaseStream.Seek(41 * 2 + 2, SeekOrigin.Current);
            if (imcount > 0)
            {
                for (var c = 0; c < chunkcount; c++)
                {
                    var chunk = new JonbinChunk();
                    chunk.SrcX = jonbr.ReadSingle();
                    chunk.SrcY = jonbr.ReadSingle();
                    chunk.SrcWidth = jonbr.ReadSingle();
                    chunk.SrcHeight = jonbr.ReadSingle();
                    chunk.DestX = jonbr.ReadSingle();
                    chunk.DestY = jonbr.ReadSingle();
                    chunk.DestWidth = jonbr.ReadSingle();
                    chunk.DestHeight = jonbr.ReadSingle();
                    jonbr.BaseStream.Seek(0x20, SeekOrigin.Current);
                    chunk.Layer = jonbr.ReadInt32();
                    jonbr.BaseStream.Seek(0xC, SeekOrigin.Current);
                    chunks.Add(chunk);
                }
            }
            for (var h = 0; h < hurtboxcount; h++)
            {
                var hurt = new JonbinBox();
                hurt.id = jonbr.ReadInt32();
                hurt.x = jonbr.ReadSingle();
                hurt.y = jonbr.ReadSingle();
                hurt.width = jonbr.ReadSingle();
                hurt.height = jonbr.ReadSingle();
                hurtboxes.Add(hurt);
            }

            int secondPos = (int)jonbr.BaseStream.Position;
            jonbr.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] firstData = jonbr.ReadBytes(secondPos);

            jonbw.Write(firstData, 0, firstData.Length);

            jonbw.Seek(firstPos, SeekOrigin.Begin);
            jonbw.Write(hurtboxcount);

            jonbw.Seek(0, SeekOrigin.End);

            jonbr.BaseStream.Seek(0x14, SeekOrigin.Current);

            byte[] secondData = jonbr.ReadBytes((int)jonbr.BaseStream.Length - secondPos + 0x14);
            jonbw.Write(secondData, 0, secondData.Length);

            jonbr.BaseStream.Seek(0x14, SeekOrigin.Begin);

            for (var h = 0; h < hitboxcount; h++)
            {
                var hit = new JonbinBox();
                hit.id = jonbr.ReadInt32();
                hit.x = jonbr.ReadSingle();
                hit.y = jonbr.ReadSingle();
                hit.width = jonbr.ReadSingle();
                hit.height = jonbr.ReadSingle();
                hitboxes.Add(hit);
            }

            jonbr.Close();
            jonbw.Close();
            return true;
        }
        public bool RemoveHitbox(StorageFile file, int coloffset)
        {
            BinaryReader jonbr = new BinaryReader(new FileStream(file.Path, FileMode.Open));
            BinaryWriter jonbw = new BinaryWriter(new FileStream(file.Path + ".tmp", FileMode.Create));
            jonbr.BaseStream.Seek((long)coloffset, SeekOrigin.Begin);
            if (!jonbr.ReadChars(4).SequenceEqual(JONBIN_HEADER.ToCharArray()))
            {
                return false;
            }
            short imcount = jonbr.ReadInt16();
            for (var i = 0; i < imcount; i++)
            {
                var strbytes = jonbr.ReadBytes(0x20).TakeWhile(b => (b != 0)).ToArray();
            }
            jonbr.BaseStream.Seek(3, SeekOrigin.Current);
            var chunkcount = jonbr.ReadUInt32();

            hurtboxcount = (uint)jonbr.ReadInt16();
            int firstPos = (int)jonbr.BaseStream.Position;
            hitboxcount = (uint)jonbr.ReadInt16();

            if (hitboxcount == 0)
            {
                jonbr.Close();
                jonbw.Close();
                return false;
            }
            hitboxcount--;

            chunks = new List<JonbinChunk>();
            hurtboxes = new List<JonbinBox>();
            hitboxes = new List<JonbinBox>();

            jonbr.BaseStream.Seek(41 * 2 + 2, SeekOrigin.Current);
            if (imcount > 0)
            {
                for (var c = 0; c < chunkcount; c++)
                {
                    var chunk = new JonbinChunk();
                    chunk.SrcX = jonbr.ReadSingle();
                    chunk.SrcY = jonbr.ReadSingle();
                    chunk.SrcWidth = jonbr.ReadSingle();
                    chunk.SrcHeight = jonbr.ReadSingle();
                    chunk.DestX = jonbr.ReadSingle();
                    chunk.DestY = jonbr.ReadSingle();
                    chunk.DestWidth = jonbr.ReadSingle();
                    chunk.DestHeight = jonbr.ReadSingle();
                    jonbr.BaseStream.Seek(0x20, SeekOrigin.Current);
                    chunk.Layer = jonbr.ReadInt32();
                    jonbr.BaseStream.Seek(0xC, SeekOrigin.Current);
                    chunks.Add(chunk);
                }
            }
            for (var h = 0; h < hurtboxcount; h++)
            {
                var hurt = new JonbinBox();
                hurt.id = jonbr.ReadInt32();
                hurt.x = jonbr.ReadSingle();
                hurt.y = jonbr.ReadSingle();
                hurt.width = jonbr.ReadSingle();
                hurt.height = jonbr.ReadSingle();
                hurtboxes.Add(hurt);
            }
            for (var h = 0; h < hitboxcount - 1; h++)
            {
                var hit = new JonbinBox();
                hit.id = jonbr.ReadInt32();
                hit.x = jonbr.ReadSingle();
                hit.y = jonbr.ReadSingle();
                hit.width = jonbr.ReadSingle();
                hit.height = jonbr.ReadSingle();
                hitboxes.Add(hit);
            }

            int secondPos = (int)jonbr.BaseStream.Position;
            jonbr.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] firstData = jonbr.ReadBytes(secondPos);

            jonbw.Write(firstData, 0, firstData.Length);

            jonbw.Seek(firstPos, SeekOrigin.Begin);

            jonbw.Write(hitboxcount);

            jonbw.Seek(0, SeekOrigin.End);

            jonbr.BaseStream.Seek(0x14, SeekOrigin.Current);

            byte[] secondData = jonbr.ReadBytes((int)jonbr.BaseStream.Length - secondPos);
            jonbw.Write(secondData, 0, secondData.Length);

            jonbr.Close();
            jonbw.Close();
            return true;
        }

        private void parseJONB(StorageFile file, int coloffset)
        {
            BinaryReader jonbr = new BinaryReader(new FileStream(file.Path, FileMode.Open));
            jonbr.BaseStream.Seek((long)coloffset, SeekOrigin.Begin);
            if (!jonbr.ReadChars(4).SequenceEqual(JONBIN_HEADER.ToCharArray()))
            {
                return;
            }
            Console.WriteLine("Image Names: ");
            short imcount = jonbr.ReadInt16();
            for (var i = 0; i < imcount; i++)
            {
                var strbytes = jonbr.ReadBytes(0x20).TakeWhile(b => (b != 0)).ToArray();
                Console.WriteLine(Encoding.UTF8.GetString(strbytes, 0, strbytes.Length));
            }
            jonbr.BaseStream.Seek(3, SeekOrigin.Current);
            var chunkcount = jonbr.ReadUInt32();
            hurtboxcount = (uint)jonbr.ReadInt16();
            hitboxcount = (uint)jonbr.ReadInt16();
            chunks = new List<JonbinChunk>();
            hurtboxes = new List<JonbinBox>();
            hitboxes = new List<JonbinBox>();
            jonbr.BaseStream.Seek(41 * 2 + 2, SeekOrigin.Current);
            if (imcount > 0)
            {
                for (var c = 0; c < chunkcount; c++)
                {
                    var chunk = new JonbinChunk();
                    chunk.SrcX = jonbr.ReadSingle();
                    chunk.SrcY = jonbr.ReadSingle();
                    chunk.SrcWidth = jonbr.ReadSingle();
                    chunk.SrcHeight = jonbr.ReadSingle();
                    chunk.DestX = jonbr.ReadSingle();
                    chunk.DestY = jonbr.ReadSingle();
                    chunk.DestWidth = jonbr.ReadSingle();
                    chunk.DestHeight = jonbr.ReadSingle();
                    jonbr.BaseStream.Seek(0x20, SeekOrigin.Current);
                    chunk.Layer = jonbr.ReadInt32();
                    jonbr.BaseStream.Seek(0xC, SeekOrigin.Current);
                    chunks.Add(chunk);
                }
            }
            for (var h = 0; h < hurtboxcount; h++)
            {
                var hurt = new JonbinBox();
                hurt.id = jonbr.ReadInt32();
                hurt.x = jonbr.ReadSingle();
                hurt.y = jonbr.ReadSingle();
                hurt.width = jonbr.ReadSingle();
                hurt.height = jonbr.ReadSingle();
                hurtboxes.Add(hurt);
            }
            for (var h = 0; h < hitboxcount; h++)
            {
                var hit = new JonbinBox();
                hit.id = jonbr.ReadInt32();
                hit.x = jonbr.ReadSingle();
                hit.y = jonbr.ReadSingle();
                hit.width = jonbr.ReadSingle();
                hit.height = jonbr.ReadSingle();
                hitboxes.Add(hit);
            }
            jonbr.Close();
        }
    }

    public class JonbinBox
    {
        public int id { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public override string ToString()
        {
            return width.ToString() + " by " + height.ToString() + " box at (" + x.ToString() + "," + y.ToString() + ")";
        }
    }
    public class JonbinChunk
    {
        public float SrcX { get; set; }
        public float SrcY { get; set; }
        public float SrcWidth { get; set; }
        public float SrcHeight { get; set; }
        public float DestX { get; set; }
        public float DestY { get; set; }
        public float DestWidth { get; set; }
        public float DestHeight { get; set; }
        public int Layer { get; set; }

    }
}
