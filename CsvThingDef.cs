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
            var name = split[4];
            var hp = int.Parse(split[5]);
            var speed = int.Parse(split[6]);
            var color = split[7];

            var thing = new CsvThing(type, spawn, run, pain, name, hp, speed, color);
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
    public readonly string Name;
    public readonly int Hp;
    public readonly int Speed;
    public readonly string Color;

    public CsvThing(int type, string spawn, string run, string pain, string name, int hp, int speed, string color)
    {
        Type = type;
        Spawn = spawn;
        Run = run;
        Pain = pain;
        Name = name;
        Hp = hp;
        Speed = speed;
        Color = color;
    }

    public override string ToString()
    {
        return Type + ", " + Spawn + ", " + Run + ", " + Pain;
    }
}
