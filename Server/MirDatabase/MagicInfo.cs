using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Server.MirEnvir;
using S = ServerPackets;

namespace Server.MirDatabase
{
    public class MagicInfo
    {
        protected static Envir Envir
        {
            get { return Envir.Main; }
        }

        public string Name;
        public Spell Spell;
        public byte BaseCost, LevelCost, Icon;
        public byte Level1, Level2, Level3;
        public ushort Need1, Need2, Need3;
        public uint DelayBase = 1800, DelayReduction;
        public ushort PowerBase, PowerBonus;
        public ushort MPowerBase, MPowerBonus;
        public float MultiplierBase = 1.0f, MultiplierBonus;
        public byte Range = 9;

        public override string ToString()
        {
            return Name;
        }

        public MagicInfo()
        {

        }

        public MagicInfo (BinaryReader reader, int version = int.MaxValue, int Customversion = int.MaxValue)
        {
            Name = reader.ReadString();
            Spell = (Spell)reader.ReadByte();
            BaseCost = reader.ReadByte();
            LevelCost = reader.ReadByte();
            Icon = reader.ReadByte();
            Level1 = reader.ReadByte();
            Level2 = reader.ReadByte();
            Level3 = reader.ReadByte();
            Need1 = reader.ReadUInt16();
            Need2 = reader.ReadUInt16();
            Need3 = reader.ReadUInt16();
            DelayBase = reader.ReadUInt32();
            DelayReduction = reader.ReadUInt32();
            PowerBase = reader.ReadUInt16();
            PowerBonus = reader.ReadUInt16();
            MPowerBase = reader.ReadUInt16();
            MPowerBonus = reader.ReadUInt16();

            if (version > 66)
                Range = reader.ReadByte();
            if (version > 70)
            {
                MultiplierBase = reader.ReadSingle();
                MultiplierBonus = reader.ReadSingle();
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write((byte)Spell);
            writer.Write(BaseCost);
            writer.Write(LevelCost);
            writer.Write(Icon);
            writer.Write(Level1);
            writer.Write(Level2);
            writer.Write(Level3);
            writer.Write(Need1);
            writer.Write(Need2);
            writer.Write(Need3);
            writer.Write(DelayBase);
            writer.Write(DelayReduction);
            writer.Write(PowerBase);
            writer.Write(PowerBonus);
            writer.Write(MPowerBase);
            writer.Write(MPowerBonus);
            writer.Write(Range);
            writer.Write(MultiplierBase);
            writer.Write(MultiplierBonus);
        }

        public ClientMagicInfo ToClientInfo()
        {
            return new ClientMagicInfo()
            {
                Name = Name,
                Spell = Spell,
                Icon = Icon,
                Level1 = Level1,
                Level2 = Level2,
                Level3 = Level3,
                Need1 = Need1,
                Need2 = Need2,
                Need3 = Need3
            };
        }
    }

    public class UserMagic
    {
        protected static Envir Envir
        {
            get { return Envir.Main; }
        }

        public Spell Spell;
        public MagicInfo Info;        

        private byte level;
        public byte Level
        {
            get
            {
                byte value = 0;

                if (Item != null)
                    value = (byte)Item.MaxDura;
                else
                    value = level;

                UserMagic support = GetSupportMagic(Spell.Empower);
                if (support != null && support.Level > 2)
                    value++;

                return value;
            }
            set
            {
                if (Item != null)
                    Item.MaxDura = value;
                else
                    level = value;
            }
        }
        public byte RealLevel
        {
            get
            {
                if (Item != null)
                    return (byte)Item.MaxDura;
                else
                    return level;
            }
            set
            {
                if (Item != null)
                    Item.MaxDura = value;
                else
                    level = value;
            }
        }

        private ushort experience;
        public ushort Experience
        {
            get
            {
                if (Item != null)
                    return Item.CurrentDura;
                else
                    return experience;
            }
            set
            {
                if (Item != null)
                    Item.CurrentDura = value;
                else
                    experience = value;
            }
        }
        public byte Key;
        public bool IsTempSpell;
        public long CastTime;

        public UserMagic[] SupportMagics = new UserMagic[3];

        public UserItem Item;

        private MagicInfo GetMagicInfo(Spell spell)
        {
            for (int i = 0; i < Envir.MagicInfoList.Count; i++)
            {
                MagicInfo info = Envir.MagicInfoList[i];
                if (info.Spell != spell) continue;
                return info;
            }
            return null;
        }

        public UserMagic(Spell spell)
        {
            Spell = spell;
            
            Info = GetMagicInfo(Spell);
        }
        public UserMagic(BinaryReader reader)
        {
            Spell = (Spell) reader.ReadByte();
            Info = GetMagicInfo(Spell);

            Level = reader.ReadByte();
            Key = reader.ReadByte();
            Experience = reader.ReadUInt16();

            if (Envir.LoadVersion < 15) return;
            IsTempSpell = reader.ReadBoolean();

            if (Envir.LoadVersion < 65) return;
            CastTime = reader.ReadInt64();
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write((byte) Spell);

            writer.Write(Level);
            writer.Write(Key);
            writer.Write(Experience);
            writer.Write(IsTempSpell);
            writer.Write(CastTime);
        }

        public Packet GetInfo()
        {
            return new S.NewMagic
                {
                    Magic = CreateClientMagic()
                };
        }

        public ClientMagic CreateClientMagic()
        {
            return new ClientMagic
            {
                    Name = Info.Name,
                    Spell = Spell,
                    BaseCost = Info.BaseCost,
                    LevelCost = Info.LevelCost,
                    Icon = Info.Icon,
                    Level1 = Info.Level1,
                    Level2 = Info.Level2,
                    Level3 = Info.Level3,
                    Need1 = Info.Need1,
                    Need2 = Info.Need2,
                    Need3 = Info.Need3,
                    Level = Level,
                    Key = Key,
                    Experience = Experience,
                    IsTempSpell = IsTempSpell,
                    Delay = GetDelay(),
                    Range = Info.Range,
                    CastTime = (CastTime != 0) && (Envir.Time > CastTime)? Envir.Time - CastTime: 0
            };
        }

        public int GetDamage(int DamageBase)
        {
            return (int)((DamageBase + GetPower()) * GetMultiplier());
        }

        public float GetMultiplier()
        {
            return (Info.MultiplierBase + (Level * Info.MultiplierBonus));
        }

        public int GetPower()
        {
            return (int)Math.Round((MPower() / 4F) * (Level + 1) + DefPower());
        }

        public int MPower()
        {
            if (Info.MPowerBonus > 0)
            {
                return Envir.Random.Next(Info.MPowerBase, Info.MPowerBonus + Info.MPowerBase);
            }
            else
                return Info.MPowerBase;
        }
        public int DefPower()
        {
            if (Info.PowerBonus > 0)
            {
                return Envir.Random.Next(Info.PowerBase, Info.PowerBonus + Info.PowerBase);
            }
            else
                return Info.PowerBase;
        }

        public int GetPower(int power)
        {
            return (int)Math.Round(power / 4F * (Level + 1) + DefPower());
        }

        public long GetDelay()
        {
            return Info.DelayBase - (Level * Info.DelayReduction);
        }

        public UserMagic GetSupportMagic(Spell spell)
        {
            for (int i = 0; i < SupportMagics.Length; i++)
                if (SupportMagics[i] != null && SupportMagics[i].Spell == spell)
                    return SupportMagics[i];

            return null;
        }

        public long IncreaseDurationCalculation(long value)
        {
            return value += (long)(value / 100F * ((Level + 1) * 5));
        }

        public int AddedPhysicalDamageCalculation(int value)
        {
            return value + 2 * Level + 1;
        }

        public int AddedSpiritualDamageCalculation(int value)
        {
            return value + Level + 1;
        }

        public int AddedMagicalDamageCalculation(int value)
        {
            return value + Level + 1;
        }

        public byte AdditionalAccuracyCalculation(byte value)
        {
            return (byte)(value + Level + 1);
        }

        public int FasterAttacksCalculation => (Level + 1) * 60; //60ms = 1 attack speed

        public int VileToxinsCalculation(int value)
        {
            return value + (int)(value / 100F * (10 + (Level * 5)));
        }

        public int ChanceToBleedCalculation => 2 + Level;

        public int FortifyCalculation => 5 + 5 * Level;

        public int IncreasedAuraCalculation(int value)
        {
            return value + (int)(value / 100F * (5 + (Level * 5)));
        }

        public ushort MinionDamageCalculation(ushort value)
        {
            return (ushort)(value / 100F * (10 + 10 * Level));
        }

        public uint MinionLifeCalculation(uint value)
        {
            return (ushort)(value / 100F * (10 + 10 * Level));
        }

        public ushort MinionDefenceCalculation => (ushort)(5 + 5 * Level);

        public int FeedingFrenzyCalculation => Level + 1;

        public byte IncreasedCriticalDamageCalculation(byte value)
        {
            return (byte)(value / 100F * (2 + 2 * Level));
        }
    }
}
