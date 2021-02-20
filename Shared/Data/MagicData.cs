using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class ClientMagicInfo
{
    public string Name;
    public Spell Spell;
    public byte Icon;
    public byte Level1, Level2, Level3;
    public ushort Need1, Need2, Need3;

    public override string ToString()
    {
        return Name;
    }

    public ClientMagicInfo()
    {

    }

    public ClientMagicInfo(BinaryReader reader, int version = int.MaxValue, int Customversion = int.MaxValue)
    {
        Name = reader.ReadString();
        Spell = (Spell)reader.ReadByte();
        Icon = reader.ReadByte();
        Level1 = reader.ReadByte();
        Level2 = reader.ReadByte();
        Level3 = reader.ReadByte();
        Need1 = reader.ReadUInt16();
        Need2 = reader.ReadUInt16();
        Need3 = reader.ReadUInt16();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(Name);
        writer.Write((byte)Spell);
        writer.Write(Icon);
        writer.Write(Level1);
        writer.Write(Level2);
        writer.Write(Level3);
        writer.Write(Need1);
        writer.Write(Need2);
        writer.Write(Need3);
    }
}