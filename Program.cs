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
}
