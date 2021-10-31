using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CsvThingDef
{
    private List<CsvThing> things;

    public CsvThingDef(string path)
    {
        things = new List<CsvThing>();

        foreach (var line in File.ReadLines(path).Skip(1))
        {
            var split = line.Split(',');
            var type = int.Parse(split[0]);
            var spawn = split[1];
            var run = split[2];
            var pain = split[3];

            var thing = new CsvThing(type, spawn, run, pain);
            things.Add(thing);
        }
    }

    public IReadOnlyList<CsvThing> Things => things;
}



public class CsvThing
{
    public readonly int Type;
    public readonly string Spawn;
    public readonly string Run;
    public readonly string Pain;

    public CsvThing(int type, string spawn, string run, string pain)
    {
        Type = type;
        Spawn = spawn;
        Run = run;
        Pain = pain;
    }

    public override string ToString()
    {
        return Type + ", " + Spawn + ", " + Run + ", " + Pain;
    }
}
