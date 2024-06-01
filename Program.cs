using System.IO;

namespace Chaos_Heroes_Online_Extractor
{
    class Program
    {
        static BinaryReader br;
        static void Main(string[] args)
        {
            br = new(File.OpenRead(args[0]));
            br.BaseStream.Position = 6;
            long tableStart = br.ReadInt64();
            br.BaseStream.Position = tableStart;

            br.BaseStream.Position += 22;//Todo: figure out what this is. Maybe i can find a name table.
            int fileCount = br.ReadInt32();
            System.Collections.Generic.List<Subfile> subfiles = new();
            for (int i = 0; i < fileCount; i++)
            {
                subfiles.Add(new());
                br.ReadInt64();//padding
            }

            //subfiles.Sort();

            string path = Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//";
            Directory.CreateDirectory(path);
            int n = 0;
            foreach (Subfile file in subfiles)
            {
                br.BaseStream.Position = file.start;
                BinaryWriter bw = new(File.Create(path + n));
                bw.Write(br.ReadBytes(file.size));
                bw.Close();

                br = new(File.OpenRead(path + "//" + n));
                string magic = new string(System.Text.Encoding.UTF7.GetString(br.ReadBytes(4)));
                if (magic == "Game")
                    magic += new string(System.Text.Encoding.UTF7.GetString(br.ReadBytes(9)));
                else if (magic == ";Gam")
                    magic += new string(System.Text.Encoding.UTF7.GetString(br.ReadBytes(9)));
                br.Close();
                br = new(File.OpenRead(args[0]));

                switch (magic)
                {
                    case "\u0089PNG":
                        File.Move(path + "//" + n, path + "//" + n + ".png");
                        break;
                    case "DDS ":
                        File.Move(path + "//" + n, path + "//" + n + ".dds");
                        break;
                    case "Gamebryo File":
                        File.Move(path + "//" + n, path + "//" + n + ".nif");
                        break;
                    case "Gamebryo KFM ":
                    case ";Gamebryo KFM":
                        File.Move(path + "//" + n, path + "//" + n + ".kfm");
                        break;
                    case "GFX\u0008":
                    case "GFX\u0009":
                    case "GFX\u000A":
                        File.Move(path + "//" + n, path + "//" + n + ".gfx");
                        break;
                    case "OggS":
                        File.Move(path + "//" + n, path + "//" + n + ".ogg");
                        break;
                    case "ID3":
                    case "ÿû°@":
                    case "ÿû°`":
                        File.Move(path + "//" + n, path + "//" + n + ".mp3");
                        break;
                    case "ÿØÿá":
                        File.Move(path + "//" + n, path + "//" + n + ".jpeg");
                        break;
                    case "LJ":
                        File.Move(path + "//" + n, path + "//" + n + ".lj");
                        break;
                    case "GIF8":
                        File.Move(path + "//" + n, path + "//" + n + ".gif");
                        break;
                    case "wOFF":
                        File.Move(path + "//" + n, path + "//" + n + ".woff");
                        break;
                    default: break;
                }
                n++;
            }
        }

        class Subfile// : System.IComparable
        {
            byte unknown = br.ReadByte();//usually 0x4000. Sometimes 0x2000.
            public long start = br.ReadInt64();
            public int size = br.ReadInt32();
            /*public int CompareTo(object obj)
            {
                if (obj == null) return 1;

                Subfile otherFile = obj as Subfile;
                return this.start.CompareTo(otherFile.start);
            }*/
        }
    }
}
