using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class DuelDialog : MirImageControl
    {
        public bool[] ActiveRules = new bool[Enum.GetNames(typeof(DuelRules)).Length];

        public MirButton[] RuleButtons = new MirButton[Enum.GetNames(typeof(DuelRules)).Length];
        public MirButton CloseButton;
        public MirLabel RulesLabel, StakeLabel, OpponentStakeLabel;

        public DuelDialog()
        {
            Index = 8;
            Library = Libraries.PrguseCustom;
            Movable = true;
            Sort = true;
            Location = Center;

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(Size.Width - 30, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            RuleButtons[(int)DuelRules.NoPets] = new MirButton
            {
                HoverIndex = 9,
                Index = 9,
                Location = new Point(18, 211),
                Library = Libraries.PrguseCustom,
                Parent = this,
                PressedIndex = 9,
                Sound = SoundList.ButtonA,
            };
            RuleButtons[(int)DuelRules.NoPets].Click += (o, e) =>
            {
                RuleButtons[(int)DuelRules.NoPets].Enabled = false;
                Network.Enqueue(new C.DuelRule { Rule = DuelRules.NoPets });
            };

            RuleButtons[(int)DuelRules.NoFireWall] = new MirButton
            {
                HoverIndex = 9,
                Index = 9,
                Location = new Point(121, 211),
                Library = Libraries.PrguseCustom,
                Parent = this,
                PressedIndex = 9,
                Sound = SoundList.ButtonA,
            };
            RuleButtons[(int)DuelRules.NoFireWall].Click += (o, e) =>
            {
                RuleButtons[(int)DuelRules.NoFireWall].Enabled = false;
                Network.Enqueue(new C.DuelRule { Rule = DuelRules.NoFireWall });
            };

            RuleButtons[(int)DuelRules.NoHealingSkill] = new MirButton
            {
                HoverIndex = 9,
                Index = 9,
                PressedIndex = 9,
                Location = new Point(256, 211),
                Library = Libraries.PrguseCustom,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            RuleButtons[(int)DuelRules.NoHealingSkill].Click += (o, e) =>
            {
                RuleButtons[(int)DuelRules.NoHealingSkill].Enabled = false;
                Network.Enqueue(new C.DuelRule { Rule = DuelRules.NoHealingSkill });
            };

            RuleButtons[(int)DuelRules.CanDeathDrop] = new MirButton
            {
                HoverIndex = 9,
                Index = 9,
                PressedIndex = 9,
                Location = new Point(378, 211),
                Library = Libraries.PrguseCustom,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            RuleButtons[(int)DuelRules.CanDeathDrop].Click += (o, e) =>
            {
                RuleButtons[(int)DuelRules.CanDeathDrop].Enabled = false;
                Network.Enqueue(new C.DuelRule { Rule = DuelRules.CanDeathDrop });
            };

            RuleButtons[(int)DuelRules.TimeLimit] = new MirButton
            {
                HoverIndex = 9,
                Index = 9,
                PressedIndex = 9,
                Location = new Point(75, 244),
                Library = Libraries.PrguseCustom,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            RuleButtons[(int)DuelRules.TimeLimit].Click += (o, e) =>
            {
                RuleButtons[(int)DuelRules.TimeLimit].Enabled = false;
                Network.Enqueue(new C.DuelRule { Rule = DuelRules.TimeLimit });
            };

            StakeLabel = new MirLabel
            {
                Text = "0",
                Parent = this,
                Size = new Size(83, 17),
                Location = new Point(57, 332),
            };

            RulesLabel = new MirLabel
            {
                Text = "",
                Parent = this,
                Size = new Size(110, 120),
                Location = new Point(30, 53),
            };

            OpponentStakeLabel = new MirLabel
            {
                Text = "0",
                Parent = this,
                Size = new Size(83, 17),
                Location = new Point(315, 332),
            };
        }

        public void Show()
        {
            for (int i = 0; i < ActiveRules.Length; i++)
            {
                ActiveRules[i] = false;
                RuleButtons[i].Index = 9;
                RuleButtons[i].HoverIndex = 9;
                RuleButtons[i].PressedIndex = 9;
            }
            RefreshLists();
        }

        public void SetRule(DuelRules rule, bool active)
        {
            ActiveRules[(int)rule] = active;
            RuleButtons[(int)rule].Enabled = true;
            RuleButtons[(int)rule].Index = active ? 10 : 9;
            RuleButtons[(int)rule].HoverIndex = active ? 10 : 9;
            RuleButtons[(int)rule].PressedIndex = active ? 10 : 9;
            RefreshLists();
        }

        private void RefreshLists()
        {
            RulesLabel.Text = string.Empty;

            for (int i = 0; i < ActiveRules.Length; i++)
            {
                if (!ActiveRules[i]) continue;
               
                Type type = ((DuelRules)i).GetType();
                MemberInfo[] infos = type.GetMember(((DuelRules)i).ToString());
                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                RulesLabel.Text += description.Description + Environment.NewLine;
            }
        }
    }
}
