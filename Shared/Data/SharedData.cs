using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class SelectInfo
{
    public int Index;
    public string Name = string.Empty;
    public ushort Level;
    public MirClass Class;
    public MirGender Gender;
    public DateTime LastAccess;

    public SelectInfo() { }

    public SelectInfo(BinaryReader reader)
    {
        Index = reader.ReadInt32();
        Name = reader.ReadString();
        Level = reader.ReadUInt16();
        Class = (MirClass)reader.ReadByte();
        Gender = (MirGender)reader.ReadByte();
        LastAccess = DateTime.FromBinary(reader.ReadInt64());
    }
    public void Save(BinaryWriter writer)
    {
        writer.Write(Index);
        writer.Write(Name);
        writer.Write(Level);
        writer.Write((byte)Class);
        writer.Write((byte)Gender);
        writer.Write(LastAccess.ToBinary());
    }
}

public class Door
{
    public byte index;
    public DoorState DoorState;
    public byte ImageIndex;
    public long LastTick;
    public Point Location;
}

public class RankCharacterInfo
{
    public long PlayerId;
    public string Name;
    public MirClass Class;
    public int level;

    public long Experience;//clients shouldnt care about this only server
    public object info;//again only keep this on server!

    public RankCharacterInfo()
    {

    }
    public RankCharacterInfo(BinaryReader reader)
    {
        PlayerId = reader.ReadInt64();
        Name = reader.ReadString();
        level = reader.ReadInt32();
        Class = (MirClass)reader.ReadByte();

    }
    public void Save(BinaryWriter writer)
    {
        writer.Write(PlayerId);
        writer.Write(Name);
        writer.Write(level);
        writer.Write((byte)Class);
    }
}

public class QuestItemReward
{
    public ItemInfo Item;
    public uint Count = 1;

    public QuestItemReward() { }

    public QuestItemReward(BinaryReader reader)
    {
        Item = new ItemInfo(reader);
        Count = reader.ReadUInt32();
    }

    public void Save(BinaryWriter writer)
    {
        Item.Save(writer);
        writer.Write(Count);
    }
}

public class ClientMovementInfo
{
    public int DestinationIndex;
    public Point Source;

    public ClientMovementInfo() { }

    public ClientMovementInfo(BinaryReader reader)
    {
        DestinationIndex = reader.ReadInt32();
        Source = new Point(reader.ReadInt32(), reader.ReadInt32());
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(DestinationIndex);
        writer.Write(Source.X);
        writer.Write(Source.Y);
    }
}

public class ClientMapInfo
{
    public int MapIndex;
    public Size MapSize;
    public string MapName;
    public int BigMap;
    public List<ClientMovementInfo> Movements = new List<ClientMovementInfo>();

    public ClientMapInfo() { }

    public ClientMapInfo(BinaryReader reader)
    {
        MapIndex = reader.ReadInt32();
        MapName = reader.ReadString();
        BigMap = reader.ReadInt32();
        MapSize = new Size(reader.ReadInt32(), reader.ReadInt32());

        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            Movements.Add(new ClientMovementInfo(reader));
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(MapIndex);
        writer.Write(MapName);
        writer.Write(BigMap);
        writer.Write(MapSize.Width);
        writer.Write(MapSize.Height);

        writer.Write(Movements.Count);
        foreach (ClientMovementInfo movement in Movements)
            movement.Save(writer);
    }
}