using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManagedDoom;

public class DehExporter
{
    private CsvThingDef csvThingDef;
    private CsvFrameDef csvFrameDef;

    private CsvFrame[] csvFrames;
    private MobjStateDef[] sourceFrames;

    public DehExporter(CsvThingDef csvThingDef, CsvFrameDef csvFrameDef)
    {
        this.csvThingDef = csvThingDef;
        this.csvFrameDef = csvFrameDef;

        var monsterStart = 174;
        var monsterEnd = 762;
        var demonStart = 477;
        var demonEnd = 489;
        var fireStart = 791;
        var fireEnd = 798;

        var normalSourceFrames = new Queue<MobjStateDef>();
        var demonSourceFrames = new Queue<MobjStateDef>();

        for (var i = monsterStart; i <= monsterEnd; i++)
        {
            if (DoomInfo.States[i].MobjAction != null)
            {
                if (demonStart <= i && i <= demonEnd)
                {
                    demonSourceFrames.Enqueue(DoomInfo.States[i]);
                }
                else
                {
                    normalSourceFrames.Enqueue(DoomInfo.States[i]);
                }
            }
        }

        for (var i = fireStart; i <= fireEnd; i++)
        {
            if (DoomInfo.States[i].MobjAction != null)
            {
                normalSourceFrames.Enqueue(DoomInfo.States[i]);
            }
        }

        csvFrames = csvFrameDef.Frames.ToArray();
        sourceFrames = new MobjStateDef[csvFrames.Length];

        for (var i = 0; i < csvFrames.Length; i++)
        {
            sourceFrames[i] = GetSourceFrame(csvFrames[i], normalSourceFrames, demonSourceFrames);
        }
    }

    public void Export(string path)
    {
        using (var writer = new StreamWriter(path))
        {
            WriteHeader(writer);
            ExportThings(writer);
            ExportFrames(writer);
            ExportPointers(writer);
            WriteOptions(writer);
        }
    }

    private void WriteHeader(StreamWriter writer)
    {
        writer.WriteLine("Patch File for DeHackEd v3.0");
        writer.WriteLine();
        writer.WriteLine("# Note: Use the pound sign ('#') to start comment lines.");
        writer.WriteLine();
        writer.WriteLine("Doom version = 19");
        writer.WriteLine("Patch format = 6");
        writer.WriteLine();
        writer.WriteLine();
    }

    private void WriteOptions(StreamWriter writer)
    {
        writer.WriteLine("Misc 0");
        writer.WriteLine("Monsters Infight = 221");
        writer.WriteLine("");
        writer.WriteLine("Text 6 6");
        writer.WriteLine("manatkplpain");
        writer.WriteLine("");
        writer.WriteLine("Text 6 6");
        writer.WriteLine("vilatkdshtgn");
        writer.WriteLine("");
        writer.WriteLine("Text 5 5");
        writer.WriteLine("metaldbopn");
        writer.WriteLine("");
        writer.WriteLine("Text 6 6");
        writer.WriteLine("bspwlkdbload");
        writer.WriteLine("");
        writer.WriteLine("Text 4 5");
        writer.WriteLine("hoofdbcls");
        writer.WriteLine("");
    }

    private void ExportThings(StreamWriter writer)
    {
        foreach (var csvThing in csvThingDef.Things)
        {
            writer.WriteLine("Thing " + csvThing.Type + " (" + csvThing.Name + ")");
            writer.WriteLine("Initial frame = " + GetSourceFrame(csvThing.Spawn, csvFrames, sourceFrames).Number);
            writer.WriteLine("Hit points = " + csvThing.Hp);
            writer.WriteLine("First moving frame = " + GetSourceFrame(csvThing.Run, csvFrames, sourceFrames).Number);
            writer.WriteLine("Alert sound = 0");
            writer.WriteLine("Reaction time = 8");
            writer.WriteLine("Attack sound = 0");
            writer.WriteLine("Injury frame = " + GetSourceFrame(csvThing.Pain, csvFrames, sourceFrames).Number);
            writer.WriteLine("Pain chance = 255");
            writer.WriteLine("Pain sound = 0");
            writer.WriteLine("Close attack frame = 0");
            writer.WriteLine("Far attack frame = 0");
            writer.WriteLine("Death frame = 158");
            writer.WriteLine("Exploding frame = 165");
            writer.WriteLine("Death sound = 0");
            writer.WriteLine("Speed = " + csvThing.Speed);
            writer.WriteLine("Width = " + Fixed.FromInt(20).Data);
            writer.WriteLine("Height = " + Fixed.FromInt(56).Data);
            writer.WriteLine("Mass = 100");
            writer.WriteLine("Missile damag = 0");
            writer.WriteLine("Action sound = 0");
            writer.WriteLine("Bits = " + GetBitsFromColor(csvThing.Color));
            writer.WriteLine("Respawn frame = 0");
            writer.WriteLine();
        }

        writer.WriteLine("Thing 30 (Demon spawn fire)");
        writer.WriteLine("Initial frame = 130");
        writer.WriteLine();

        writer.WriteLine("Thing 37 (Arachnotron projectile)");
        writer.WriteLine("Initial frame = 107");
        writer.WriteLine("Death frame = 109");
        writer.WriteLine();
    }

    private void ExportFrames(StreamWriter writer)
    {
        for (var i = 0; i < csvFrames.Length; i++)
        {
            var csvFrame = csvFrames[i];
            var sourceFrame = sourceFrames[i];

            var nextCsvFrame = csvFrameDef[csvFrame.Next];
            var nextSourceFrame = GetSourceFrame(nextCsvFrame, csvFrames, sourceFrames);

            writer.WriteLine("Frame " + sourceFrame.Number);
            writer.WriteLine("Sprite number = 28");
            writer.WriteLine("Sprite subnumber = " + csvFrame.Sprite);
            writer.WriteLine("Duration = " + csvFrame.Duration);
            writer.WriteLine("Next frame = " + nextSourceFrame.Number);
            writer.WriteLine();
        }
    }

    private void ExportPointers(StreamWriter writer)
    {
        for (var i = 0; i < csvFrames.Length; i++)
        {
            var csvFrame = csvFrames[i];
            var sourceFrame = sourceFrames[i];

            writer.WriteLine("Pointer 0 (Frame " + sourceFrame.Number + ")");
            writer.WriteLine("Codep Frame = " + GetCodep(csvFrame.Action));
            writer.WriteLine();
        }
    }

    private static MobjStateDef GetSourceFrame(CsvFrame csvFrame, Queue<MobjStateDef> normal, Queue<MobjStateDef> demon)
    {
        if (csvFrame.Duration == 0 && demon.Count > 0)
        {
            return demon.Dequeue();
        }
        else
        {
            return normal.Dequeue();
        }
    }

    private static MobjStateDef GetSourceFrame(CsvFrame csvFrame, CsvFrame[] csvFrames, MobjStateDef[] sourceFrames)
    {
        for (var i = 0; i < csvFrames.Length; i++)
        {
            if (csvFrames[i] == csvFrame)
            {
                return sourceFrames[i];
            }
        }

        throw new Exception();
    }

    private static MobjStateDef GetSourceFrame(string label, CsvFrame[] csvFrames, MobjStateDef[] sourceFrames)
    {
        var csvFrame = csvFrames.First(x => x.Label == label);

        for (var i = 0; i < csvFrames.Length; i++)
        {
            if (csvFrames[i] == csvFrame)
            {
                return sourceFrames[i];
            }
        }

        throw new Exception();
    }

    private static int GetBitsFromColor(string color)
    {
        switch (color)
        {
            case "green":
                return 20980742;

            case "indigo":
                return 88089606;

            case "brown":
                return 155198470;

            case "red":
                return 222307334;

            default:
                throw new Exception();
        }
    }

    private static int GetCodep(string action)
    {
        switch (action)
        {
            case "look":
                return 174;

            case "chase":
                return 176;

            case "face":
                return 184;

            case "refire":
                return 618;

            case "pain":
                return 376;

            case "shotgun":
                return 218;

            case "chaingun":
                return 185;

            case "rocket":
                return 685;

            case "plasma":
                return 648;

            case "ssg":
                return 255;

            case "reload1":
                return 603;

            case "reload2":
                return 635;

            case "reload3":
                return 676;

            default:
                throw new Exception();
        }
    }
}
