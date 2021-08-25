using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Server.MirEnvir;
using Server.MirDatabase;
using System.Drawing;
using S = ServerPackets;

namespace Server.MirObjects
{
    public class GroupObject
    {
        protected static Envir Envir
        {
            get { return Envir.Main; }
        }
        public List<CharacterInfo> GroupMemberCharacters = new List<CharacterInfo>();
        public List<PlayerObject> GroupMembers = new List<PlayerObject>();

        public GroupObject()
        {
        }
        public GroupObject(BinaryReader reader)
        {
            int version = reader.ReadInt32();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                CharacterInfo character = Envir.GetCharacterInfo(reader.ReadInt32());
                GroupMemberCharacters.Add(character);
                character.Group = this;
            }
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(Envir.Version);
            writer.Write(GroupMemberCharacters.Count);
            for (int i = 0; i < GroupMemberCharacters.Count; i++)
                writer.Write(GroupMemberCharacters[i].Index);
        }

        public void SendMessage(string message, ChatType type = ChatType.Group)
        {
            foreach (PlayerObject player in GroupMembers)
            {
                if (player == null) continue;
                player.ReceiveChat(message, type);
            }
        }

        public void Connect(PlayerObject player)
        {
            Packet p = new S.GroupConnect { Name = player.Name };

            for (int i = 0; i < GroupMembers.Count; i++)
                GroupMembers[i].Enqueue(p);

            GroupMembers.Add(player);

            if (GroupMembers[0] == player)
            {
                GroupMemberCharacters.Remove(player.Info);
                GroupMemberCharacters.Insert(0, player.Info);
            }

            SendMembers(player);
        }

        public void Disconnect(PlayerObject player)
        {
            if (GroupMembers[0] == player)
            {
                GroupMembers.Remove(player);
                if (GroupMembers.Count > 0)
                    SendMessage($"{GroupMembers[0].Name} has been promoted to group leader.");
            }
            else
                GroupMembers.Remove(player);

            Packet p = new S.GroupDisconnect { Name = player.Name };

            for (int i = 0; i < GroupMembers.Count; i++)
                GroupMembers[i].Enqueue(p);

            GroupMemberCharacters.Remove(player.Info);
            GroupMemberCharacters.Add(player.Info);
        }

        public void Add(PlayerObject player)
        {           
            Packet p = new S.AddMember { Name = player.Name, Online = true };

            for (int i = 0; i < GroupMembers.Count; i++)
                GroupMembers[i].Enqueue(p);

            GroupMembers.Add(player);
            GroupMemberCharacters.Add(player.Info);

            player.Group = this;

            SendMembers(player);           
        }

        public void Remove(PlayerObject player)
        {
            GroupMembers.Remove(player);
            GroupMemberCharacters.Remove(player.Info);
            player.Group = null;

            player.Enqueue(new S.DeleteGroup());

            Packet p = new S.DeleteMember { Name = player.Name };

            for (int i = 0; i < GroupMembers.Count; i++)
                GroupMembers[i].Enqueue(p);

            if (GroupMemberCharacters.Count < 2)
            {
                GroupMembers[0].Enqueue(new S.DeleteGroup());
                GroupMembers[0].Group = null;
                Envir.Groups.Remove(this);
            }
        }

        public void Remove(CharacterInfo player)
        {
            GroupMemberCharacters.Remove(player);
            player.Group = null;

            Packet p = new S.DeleteMember { Name = player.Name };

            for (int i = 0; i < GroupMembers.Count; i++)
                GroupMembers[i].Enqueue(p);

            if (GroupMemberCharacters.Count < 2)
            {
                GroupMembers[0].Enqueue(new S.DeleteGroup());
                GroupMembers[0].Group = null;
                Envir.Groups.Remove(this);
            }
        }

        public void SendMembers(PlayerObject player)
        {
            for (int i = 0; i < GroupMemberCharacters.Count; i++)
            {
                CharacterInfo character = GroupMemberCharacters[i];

                player.Enqueue(new S.AddMember { Name = character.Name, Online = character.Player != null });

                if (character.Player != null)
                {
                    PlayerObject member = character.Player;
                    if (player.CurrentMap != member.CurrentMap || !Functions.InRange(player.CurrentLocation, member.CurrentLocation, Globals.DataRange)) continue;

                    byte time = Math.Min(byte.MaxValue, (byte)Math.Max(5, (player.RevTime - Envir.Time) / 1000));

                    member.Enqueue(new S.ObjectHealth { ObjectID = player.ObjectID, Percent = player.PercentHealth, Expire = time });
                    player.Enqueue(new S.ObjectHealth { ObjectID = member.ObjectID, Percent = member.PercentHealth, Expire = time });

                    for (int j = 0; j < member.Pets.Count; j++)
                    {
                        MonsterObject pet = member.Pets[j];

                        player.Enqueue(new S.ObjectHealth { ObjectID = pet.ObjectID, Percent = pet.PercentHealth, Expire = time });
                    }
                }
            }
        }
    }
}
