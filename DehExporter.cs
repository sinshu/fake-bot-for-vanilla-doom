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

        csvFrames = csvFrameDef.Frames.ToArray();
        sourceFrames = new MobjStateDef[csvFrames.Length];

        for (var i = 0; i < csvFrames.Length; i++)
        {
            sourceFrames[i] = GetSourceFrame(csvFrames[i], normalSourceFrames, demonSourceFrames);
        }
    }

    public void ExportThings(StreamWriter writer)
    {
        foreach (var csvThing in csvThingDef.Things)
        {
            writer.WriteLine("Thing " + csvThing.Type + " (Fake Bot " + csvThing.Type + ")");
            writer.WriteLine("Initial frame = " + GetSourceFrame(csvThing.Spawn, csvFrames, sourceFrames).Number);
            writer.WriteLine("First moving frame = " + GetSourceFrame(csvThing.Run, csvFrames, sourceFrames).Number);
            writer.WriteLine("Injury frame = " + GetSourceFrame(csvThing.Pain, csvFrames, sourceFrames).Number);
            writer.WriteLine("Close attack frame = 0");
            writer.WriteLine("Far attack frame = 0");
            writer.WriteLine("Death frame = 158");
            writer.WriteLine("Exploding frame = 165");
            writer.WriteLine("Bits = 4194342");
            writer.WriteLine("Respawn frame = 0");
            writer.WriteLine();
        }
    }

    public void ExportFrames(StreamWriter writer)
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

    public void ExportPointers(StreamWriter writer)
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

            case "shotgun":
                return 218;

            default:
                throw new Exception();
        }
    }
}
