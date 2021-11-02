using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManagedDoom;

public class Program
{
    public static void Main(string[] args)
    {
        var csvFrameDef = new CsvFrameDef("framedef.csv");
        var csvThingDef = new CsvThingDef("thingdef.csv");

        var exporter = new DehExporter(csvThingDef, csvFrameDef);
        exporter.Export("test.deh");
    }

    /*
    private static void CreateNullSound(string path)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        using (var writer = new BinaryWriter(fs))
        {
            var count = 100;
            writer.Write((short)3);
            writer.Write((short)11025);
            writer.Write(count);
            for (var i = 0; i < count; i++)
            {
                writer.Write((byte)128);
            }
        }
    }
    */
}
