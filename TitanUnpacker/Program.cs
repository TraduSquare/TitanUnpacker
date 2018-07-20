using System;
using System.IO;
using System.Collections.Generic;
using Ionic.Zlib;

namespace TitanUnpacker
{
    class Program
    {
        //From here https://github.com/eropple/dotnetzip/blob/master/Examples/C%23/ZLIB/ZlibStreamExample.cs#L67
        static void CopyStream(System.IO.Stream src, System.IO.Stream dest)
        {
            byte[] buffer = new byte[1024];
            int len;
            while ((len = src.Read(buffer, 0, buffer.Length)) > 0)
                dest.Write(buffer, 0, len);
            dest.Flush();
        }
        //End
        static void Main(string[] args)
        {
            Console.WriteLine("TitanUnpacker 1.0 - Koei Tecmo games unpacker/repacker by Darkmet98.");
            Console.WriteLine("Thanks to Leeg and IlDucci for some help.");
            if (args.Length != 3 && args.Length != 2)
            {
                Console.WriteLine("USAGE: TitanUnpacker.exe -unpack/-repack/-zlib 'file' 'folder' ");
                Console.WriteLine("Unpack the file: TitanUnpacker.exe -unpack LINKDATA_EU_A.bin Unpacked ");
                Console.WriteLine("Pack the folder: TitanUnpacker.exe -pack Unpacked Generated.bin ");
                Console.WriteLine("Unpack the compressed file (WIP): TitanUnpacker.exe -zlib file.bin ");
                Environment.Exit(-1);
            }
            int magic;
            //int magic2;
            int files;
            int type;
            int blank;
            Int64 position;
            int size;
            int compression;
            Int16 Unknown1;
            Int16 Unknown2;
            Int16 Unknown3;
            int i = 0;
            int a = 0;
            string extension = "";
            List<Int64> pos = new List<Int64>();
            List<Int64> poschunk = new List<Int64>();
            List<int> filesize = new List<int>();
            List<int> comp = new List<int>();
            List<int> comp2 = new List<int>();
            List<int> headersize = new List<int>();
            List<string> names = new List<string>();
            ZlibStream zOut;
            string namelist = "";
            string ruta = "";
            switch (args[0])
            {
                case "-unpack":
                    Console.WriteLine("Unpacking...");
                    BinaryReader reader = new BinaryReader(File.Open(args[1], FileMode.Open));
                    magic = reader.ReadInt32();
                    files = reader.ReadInt32();
                    type = reader.ReadInt32();
                    blank = reader.ReadInt32();
                    for (i = 0; i < files; i++)
                    {
                        position = reader.ReadInt64();
                        size = reader.ReadInt32();
                        compression = reader.ReadInt32();
                        pos.Add(position * 2048); //IlDucci's magic.
                        filesize.Add(size);
                        comp.Add(compression);
                    }
                    if (Directory.Exists(args[2])) { return; }
                    else { Directory.CreateDirectory(args[2]); }
                    using (BinaryWriter writer = new BinaryWriter(File.Open(args[2] + "\\" + "info.bin", FileMode.Create)))
                    {
                        writer.Write(magic);
                        writer.Write(files);
                        writer.Write(type);
                    }
                    using (BinaryWriter writer = new BinaryWriter(File.Open(args[2] + "\\" + "header.bin", FileMode.Create)))
                    {
                        writer.Write(magic);
                        writer.Write(files);
                        writer.Write(type);
                        writer.Write(blank);
                        for (i = 0; i < files; i++)
                        {
                            writer.Write(0x00000000);
                            writer.Write(0x00000000);
                            writer.Write(0x00000000);
                            writer.Write(comp[i]);
                        }
                    }
                    if (File.Exists(args[1] + ".txt"))
                    {
                        // From here, https://github.com/TraduSquare/Parcheador_Universal_3DS/blob/LordOfMagna/Parcheador3DS/Form1.cs#L377
                        // Thanks Shiryu
                        string[] lista = File.ReadAllLines(args[1] + ".txt");
                        // End
                        Directory.CreateDirectory(args[2] + "\\" + "temp");
                        for (a = 0; a < files; a++)
                        {
                            using (BinaryWriter writer = new BinaryWriter(File.Open(args[2] + "\\" + "temp" + "\\" + a + ".bin", FileMode.Create)))
                            {
                                reader.BaseStream.Position = pos[a];
                                byte[] array = reader.ReadBytes(filesize[a]);
                                writer.Write(array);
                            }
                            ruta = Path.GetDirectoryName(lista[a]);
                            Directory.CreateDirectory(args[2] + "\\" + ruta);
                        }
                        for (a = 0; a < files; a++)
                        {
                            ruta = args[2] + "\\" + lista[a];
                            File.Move(args[2] + "\\" + "temp" + "\\" + a + ".bin", ruta);
                        }
                        File.Copy(args[1] + ".txt", args[2] + "\\" + "directory.txt");
                        System.IO.Directory.Delete(args[2] +  "\\" + "temp", true);
                    }
                    else
                    {
                        for (a = 0; a < files; a++)
                        {
                            reader.BaseStream.Position = pos[a];
                            magic = reader.ReadInt32();
                            if (magic == 0x47315447) { extension = ".g1t"; }
                            else if (magic == 0x00100038) { extension = ".ZL_"; }
                            else if (magic == 0x00040038) { extension = ".ZL_"; }
                            else if (magic == 0x0001CE10) { extension = ".ZL_"; }
                            else if (magic == 0x0000EB00) { extension = ".ZL_"; }
                            else if (magic == 0x000401A8) { extension = ".ZL_"; }
                            else if (magic == 0x000002C4) { extension = ".ZL_"; }
                            else if (magic == 0x000381A8) { extension = ".ZL_"; }
                            else if (magic == 0x0002D6B4) { extension = ".ZL_"; }
                            else if (magic == 0x0002D66C) { extension = ".ZL_"; }
                            else if (magic == 0x5253544B) { extension = ".ktsr"; }
                            else if (magic == 0x51475753) { extension = ".swg"; }
                            else if (magic == 0x4B53484C) { extension = ".gls"; }
                            else if (magic == 0x4731454D) { extension = ".g1em"; }
                            else { extension = ".bin"; }
                            using (BinaryWriter writer = new BinaryWriter(File.Open(args[2] + "\\" + a + extension, FileMode.Create)))
                            {
                                reader.BaseStream.Position = pos[a];
                                byte[] array = reader.ReadBytes(filesize[a]);
                                writer.Write(array);
                            }
                            names.Add(a + extension);
                            
                        }
                        System.IO.File.WriteAllLines(args[2] + "\\" + "extensionlist.txt", names.ConvertAll(Convert.ToString));
                    }
                    Console.WriteLine("The file is unpacked.");
                    break;
                case "-pack":
                    Console.WriteLine("Packing...");
                    reader = new BinaryReader(File.Open(args[1] + "\\" + "info.bin", FileMode.Open));
                    magic = reader.ReadInt32();
                    files = reader.ReadInt32();
                    type = reader.ReadInt32();
                    using (BinaryWriter writer = new BinaryWriter(File.Open(args[2], FileMode.Create)))
                    {

                        byte[] headerfile = File.ReadAllBytes(args[1] + "\\" + "header.bin");
                        if (headerfile.Length == 2048) { writer.Write(headerfile); }
                        else if (headerfile.Length < 2048)
                        {
                            for (i = 0; i < 512; i++)
                            {
                                writer.Write(0x00000000);
                            }
                            headersize.Add((Int32)writer.BaseStream.Position);
                            writer.BaseStream.Position = 0x0;
                            writer.Write(headerfile);
                            writer.BaseStream.Position = headersize[a];
                        }
                        else if (headerfile.Length > 2048)
                        {
                            int total = headerfile.Length / 0x800;
                            for (i = 0; i < 512 * (total + 1); i++)
                            {
                                writer.Write(0x00000000);
                            }
                            headersize.Add((Int32)writer.BaseStream.Position);
                            writer.BaseStream.Position = 0x0;
                            writer.Write(headerfile);
                            writer.BaseStream.Position = headersize[a];
                        }
                        for (a = 0; a < files; a++)
                        {
                            if (File.Exists(args[1] + "\\" + "directory.txt"))
                            {
                                string[] lista = File.ReadAllLines(args[1] + "\\" + "directory.txt");
                                ruta = lista[a];
                            }
                            else if (File.Exists(args[1] + "\\" + "extensionlist.txt"))
                            {
                                string[] lista = File.ReadAllLines(args[1] + "\\" + "extensionlist.txt");
                                ruta = lista[a];
                            }
                            else
                            {
                                Console.WriteLine("The file list doesn't exist, the program will close.");
                                System.Environment.Exit(-1);
                            }
                            pos.Add((Int64)writer.BaseStream.Position);
                            namelist = args[1] + "\\" + ruta;
                            byte[] array = File.ReadAllBytes(namelist);
                            int sizefile = array.Length;
                            filesize.Add(sizefile);
                            if (sizefile == 2048) { writer.Write(array); }
                            else if (sizefile < 2048)
                            {
                                for (i = 0; i < 512; i++)
                                {
                                    writer.Write(0xCDCDCDCD);
                                }
                                poschunk.Add((Int64)writer.BaseStream.Position);
                                writer.BaseStream.Position = pos[a];
                                writer.Write(array);
                                writer.BaseStream.Position = poschunk[a];
                            }
                            else if (sizefile > 2048)
                            {
                                int sizeint = sizefile;
                                int total = sizeint / 0x800;
                                for (i = 0; i < 512 * (total + 1); i++)
                                {
                                    writer.Write(0xCDCDCDCD);
                                }
                                poschunk.Add((Int64)writer.BaseStream.Position);
                                writer.BaseStream.Position = pos[a];
                                writer.Write(array);
                                writer.BaseStream.Position = poschunk[a];
                            }
                        }
                        writer.BaseStream.Position = 0x10;
                        for (a = 0; a < files; a++)
                        {
                            writer.Write(pos[a] / 2048);
                            writer.Write(filesize[a]);
                            writer.BaseStream.Position = writer.BaseStream.Position + 4;
                        }
                    }
                    Console.WriteLine("The file is packed.");
                    break;
                case "-zlib":
                    if (Directory.Exists(args[1] + "extracted")) { return; }
                    else { Directory.CreateDirectory(args[1] + "extracted"); }
                    using (reader = new BinaryReader(File.Open(args[1], FileMode.Open)))
                    {
                        Unknown1 = reader.ReadInt16();
                        Unknown2 = reader.ReadInt16();
                        Unknown3 = reader.ReadInt16();
                        for (a = 0; a < reader.BaseStream.Length; a++)
                        {
                            try
                            {
                                if (reader.ReadByte() == 0x78)
                                {
                                    if (reader.ReadByte() == 0xDA)
                                    {
                                        comp.Add((Int32)reader.BaseStream.Position);
                                    }
                                }
                            }
                            catch (System.IO.EndOfStreamException) { break; }
                        }
                        Console.WriteLine("Archivos comprimidos totales encontrados: " + comp.Count);
                        reader.BaseStream.Position = 0x8;
                        for (i = 0; i < comp.Count; i++)
                        {
                            try { size = comp[i + 1] - comp[i]; }
                            catch (System.ArgumentOutOfRangeException) { size = (int)reader.BaseStream.Length - comp[i]; }
                            using (BinaryWriter writer = new BinaryWriter(File.Open(args[1] + "extracted" + "\\" + i + ".or", FileMode.Create)))
                            {
                                try
                                {
                                    byte[] arrayzlib = reader.ReadBytes(size);
                                    Console.WriteLine("Posición " + "nº " + i + " : " + "{0:X}", comp[i]);
                                    Console.WriteLine("Tamaño total: " + "{0:X}", size + " Bytes");
                                    writer.Write(arrayzlib);
                                    Stream stream = new MemoryStream(arrayzlib);
                                    zOut = new ZlibStream(stream, CompressionMode.Decompress, true);
                                    using (FileStream file = new FileStream(args[1] + "extracted" + "\\" + i + ".dec", FileMode.Create, System.IO.FileAccess.Write)) { CopyStream(zOut, file); }
                                }
                                catch (Ionic.Zlib.ZlibException) { Console.WriteLine("The file is damaged or the stream is unknown, the program will close."); System.Environment.Exit(-1); }
                            }
                        }
                        using (reader = new BinaryReader(File.Open(args[1] + "extracted" + "\\" + "0.dec", FileMode.Open)))
                        {
                            magic = reader.ReadInt32();
                            if (magic == 0x47315447) { extension = ".g1t"; }
                            else { extension = ".unknown"; }
                        }
                        using (BinaryWriter writer = new BinaryWriter(File.Open(args[1] + extension, FileMode.Create)))
                        {
                            for (i = 0; i < comp.Count; i++)
                            {
                                byte[] array = File.ReadAllBytes(args[1] + "extracted" + "\\" + i + ".dec");
                                writer.Write(array);
                            }
                        }
                    }
                    System.IO.Directory.Delete(args[1] + "extracted", true);
                    break;
            }
        }
    }
}
