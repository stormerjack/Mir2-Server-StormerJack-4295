using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class SkillSlotDialog : MirImageControl
    {
        public MirButton CloseButton;
        public MirItemCell[] Grid;
        public static UserItem Item;

        public int StartIndex = 0;

        public SkillSlotDialog()
        {
            Index = 2;
            Library = Libraries.PrguseCustom;
            Movable = true;
            Sort = true;
            Location = new Point(10, 30);

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            Grid = new MirItemCell[Enum.GetNames(typeof(SkillSlot)).Length];

            Grid[(int)SkillSlot.Active] = new MirItemCell
            {
                ItemSlot = (int)SkillSlot.Active,
                GridType = MirGridType.SkillSlot,
                Parent = this,
                Location = new Point(206, 123),
                Size = new Size(34, 30)
            };

            Grid[(int)SkillSlot.Support1] = new MirItemCell
            {
                ItemSlot = (int)SkillSlot.Support1,
                GridType = MirGridType.SkillSlot,
                Parent = this,
                Location = new Point(157, 175),
                Size = new Size(34, 30)
            };

            Grid[(int)SkillSlot.Support2] = new MirItemCell
            {
                ItemSlot = (int)SkillSlot.Support2,
                GridType = MirGridType.SkillSlot,
                Parent = this,
                Location = new Point(206, 175),
                Size = new Size(34, 30)
            };

            Grid[(int)SkillSlot.Support3] = new MirItemCell
            {
                ItemSlot = (int)SkillSlot.Support3,
                GridType = MirGridType.SkillSlot,
                Parent = this,
                Location = new Point(255, 175),
                Size = new Size(34, 30)
            };
        }

        public new void Show(UserItem item)
        {
            Item = item;
            RefreshInterface();
            if (Visible) return;
            Visible = true;
        }

        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }

        public void RefreshInterface()
        {
            Grid[(int)SkillSlot.Support1].Visible = false;
            Grid[(int)SkillSlot.Support2].Visible = false;
            Grid[(int)SkillSlot.Support3].Visible = false;
            Index = 0;

            switch (Item.Info.Type)
            {
                case ItemType.Weapon:
                case ItemType.Armour:
                    Grid[(int)SkillSlot.Support1].Visible = true;
                    Grid[(int)SkillSlot.Support2].Visible = true;
                    Grid[(int)SkillSlot.Support3].Visible = true;
                    Index = 2;
                    break;
                case ItemType.Helmet:
                case ItemType.Boots:
                    Grid[(int)SkillSlot.Support1].Visible = true;
                    Index = 1;
                    break;
            }
        }
    }
}
