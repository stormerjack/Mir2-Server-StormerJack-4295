using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Client.MirSounds;
using System.Windows.Forms;

namespace Client.MirScenes.Dialogs
{
    public sealed class LootFilterDialog : MirImageControl
    {
        public MirButton CloseButton;

        public LootFilterDialog()
        {
            Index = 18;
            Library = Libraries.PrguseCustom;
            Movable = true;
            Sort = true;
            Location = Center;

            CloseButton = new MirButton
            {
                Location = new Point(183, 3),
                HoverIndex = 361,
                Index = 360,
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();
        }
    }
}
