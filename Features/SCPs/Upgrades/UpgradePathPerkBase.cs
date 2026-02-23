namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using LabApi.Features.Wrappers;
    using UnityEngine;

    public abstract class UpgradePathPerkBase(PerkInventory inv) : PerkBase(inv)
    {
        public readonly List<UpgradeBase> Path = [];

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
            get => _progress;
            set
            {
                if (Path.Count <= 0)
                    return;

                value = Mathf.Clamp(value, 0, Path.Count - 1);
                _progress = Mathf.Clamp(_progress, 0, Path.Count - 1);

                if (_progress == value)
                    return;

                if (value > _progress)
                {
                    for (int i = _progress; i <= value; i++)
                        Path[i].Init();
                }
                else
                {
                    for (int i = value + 1; i <= _progress; i++)
                        Path[i].Remove();
                }

                _progress = value;

                SendMessage("Progress changed! \nPress \"~\" and type \".sp\" (for more detail) \nOR bind a key in <b>Server Specific Settings</b> to check.");
                Inventory.OnPerksUpdated();
            }
        }

        private int _progress = -1;

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

    public abstract class UpgradeBase(UpgradePathPerkBase parent)
    {
        public readonly UpgradePathPerkBase Parent = parent;

        public Player Player => Parent.Player;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual void Init()
        {
        }

        public virtual void Tick()
        {
        }

        public virtual void Remove()
        {
        }

        public virtual void SendMessage(string message) => Parent.SendMessage($"<size=28><b>{Name}</b></size>\n{message}");
    }

    public abstract class UpgradeBase<T>(UpgradePathPerkBase parent) : UpgradeBase(parent) where T : UpgradePathPerkBase
    {
        public new T Parent => base.Parent is T t ? t : null;
    }
}
