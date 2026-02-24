namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public abstract class UpgradePathPerkBase(PerkInventory inv) : PerkBase(inv)
    {
        public List<UpgradeBase> Path { get; } = [];

        public override string Name => PathName + (Progress >= 0 ? " | Level " + (Progress < Path.Count - 1 ? (Progress + 1) : "MAX") : string.Empty);

        public abstract string PathName { get; }

        public override string Description
        {
            get
            {
                StringBuilder stringBuilder = new(PathDescription);

                for (int i = 0; i <= Progress; i++)
                {
                    stringBuilder.Append("\n\n<b>");
                    stringBuilder.Append(Path[i].Name);
                    stringBuilder.Append("</b>\n");
                    stringBuilder.Append(Path[i].Description);
                }

                return stringBuilder.ToString();
            }
        }

        public abstract string PathDescription { get; }

        public bool Maxed => Progress >= AllUpgrades.Length - 1;

        public int Progress
        {
            get;
            set
            {
                if (Path.Count <= 0)
                    return;

                value = Mathf.Clamp(value, 0, Path.Count - 1);
                field = Mathf.Clamp(field, 0, Path.Count - 1);

                if (field == value)
                    return;

                if (value > field)
                {
                    for (int i = field; i <= value; i++)
                        Path[i].Init();
                }
                else
                {
                    for (int i = value + 1; i <= field; i++)
                        Path[i].Remove();
                }

                field = value;

                SendMessage(
                    "Progress changed! \nPress \"~\" and type \".sp\" (for more detail) \nOR bind a key in <b>Server Specific Settings</b> to check.");
                Inventory.OnPerksUpdated();
            }
        }

        = -1;

        public abstract Type[] AllUpgrades { get; }

        public override void Init()
        {
            base.Init();

            foreach (Type upgrade in AllUpgrades)
            {
                if (upgrade.IsAbstract || !upgrade.IsSubclassOf(typeof(UpgradeBase)))
                    continue;

                UpgradeBase b = (UpgradeBase)Activator.CreateInstance(upgrade, this);
                Path.Add(b);
            }

            if (Path.Count > 0)
                Path[0].Init();
            Progress = 0;
        }

        public override void Tick()
        {
            base.Tick();

            if (Path.Count <= 0)
                return;

            for (int i = 0; i <= Progress; i++)
                Path[i].Tick();
        }

        public override void Remove()
        {
            base.Remove();

            if (Path.Count <= 0)
                return;

            for (int i = 0; i <= Progress; i++)
                Path[i].Remove();
        }
    }
}
