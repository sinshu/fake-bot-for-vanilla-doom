using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CsvFrameDef
{
    private List<CsvFrame> frames;

    public CsvFrameDef(string path)
    {
        frames = new List<CsvFrame>();

        foreach (var line in File.ReadLines(path).Skip(1))
        {
            var split = line.Split(',');
            var sprite = int.Parse(split[0]);
            var duration = int.Parse(split[1]);
            var action = split[2];
            var label = split[3];
            var next = split[4];

            var frame = new CsvFrame(sprite, duration, action, label, next);
            frames.Add(frame);
        }
    }

    public CsvFrame this[string label] => frames.First(frame => frame.Label == label);

    public IReadOnlyList<CsvFrame> Frames => frames;
}



public class CsvFrame
{
    public readonly int Sprite;
    public readonly int Duration;
    public readonly string Action;
    public readonly string Label;
    public readonly string Next;

    public CsvFrame(int sprite, int duration, string action, string label, string next)
    {
        Sprite = sprite;
        Duration = duration;
        Action = action;
        Label = label;
        Next = next;
    }

    public override string ToString()
    {
        return Sprite + ", " + Duration + ", " + Action + ", " + Label + ", " + Next;
    }
}
