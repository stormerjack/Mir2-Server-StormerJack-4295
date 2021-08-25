using System.IO;

public class ClientMonsterData
{
    public int Index;
    public ushort Level;
    public uint Experience;
    public ushort MinAC, MaxAC, MinMAC, MaxMAC;
    public ushort MinDC, MaxDC, MinMC, MaxMC, MinSC, MaxSC;
    public byte Accuracy, Agility;
    public bool IsTameable, Undead;
    public uint HP;

    public ClientMonsterData()
    {
    }
    public ClientMonsterData(BinaryReader reader)
    {
        Index = reader.ReadInt32();
        Level = reader.ReadUInt16();
        Experience = reader.ReadUInt32();
        MinAC = reader.ReadUInt16();
        MaxAC = reader.ReadUInt16();
        MinMAC = reader.ReadUInt16();
        MaxMAC = reader.ReadUInt16();
        MinDC = reader.ReadUInt16();
        MaxDC = reader.ReadUInt16();
        MinMC = reader.ReadUInt16();
        MaxMC = reader.ReadUInt16();
        MinSC = reader.ReadUInt16();
        MaxSC = reader.ReadUInt16();
        Accuracy = reader.ReadByte();
        Agility = reader.ReadByte();
        IsTameable = reader.ReadBoolean();
        Undead = reader.ReadBoolean();
        HP = reader.ReadUInt16();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(Index);
        writer.Write(Level);
        writer.Write(Experience);
        writer.Write(MinAC);
        writer.Write(MaxAC);
        writer.Write(MinMAC);
        writer.Write(MaxMAC);
        writer.Write(MinDC);
        writer.Write(MaxDC);
        writer.Write(MinMC);
        writer.Write(MaxMC);
        writer.Write(MinSC);
        writer.Write(MaxSC);
        writer.Write(Accuracy);
        writer.Write(Agility);
        writer.Write(IsTameable);
        writer.Write(Undead);
        writer.Write(HP);
    }
}