namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    using System.Collections.Generic;
    using System.Text;
    using Hints;
    using LabApi.Features.Wrappers;
    using MEC;
    using Random = UnityEngine.Random;

    public class UpgradeQueue(Player p)
    {
        public Player Parent { get; } = p;

        public Queue<Item> Upgrades { get; } = [];

        public void Create(int amount, List<UpgradePathAttribute> maxed)
        {
            for (int i = 0; i < amount; i++)
            {
                UpgradePathAttribute? att = Parent.Role.GetRandomUpgradePath(maxed);
                if (att != null)
                    maxed.Add(att);
            }

            if (maxed.Count > 0)
            {
                Upgrades.Enqueue(new Item(this, maxed.ToArray()));
                Parent.SendHint("<color=#00FF00><b>Upgrades available!</b></color>\nPress \"~\" and type \".su\" (for more detail) to see available choices, type \".c <number>\" to choose an upgrade. \nOR bind a key in <b>Server Specific Settings</b>\n<size=0.3em>An upgrade will be automatically chosen in 30s</size>", [HintEffectPresets.FadeOut()], 15f);
                Parent.SendConsoleMessage(Peek(), "white");
            }
        }

        public string Peek(bool brief = false)
        {
            string unBrief = Peek(out string bri);
            return !brief ? unBrief : bri;
        }

        public string Peek(out string brief)
        {
            if (Upgrades.Count <= 0)
            {
                brief = "No upgrades available.";
                return brief;
            }

            Item t = Upgrades.Peek();
            StringBuilder sb = new("Choices: \n");
            StringBuilder br = new("Choices: \n");

            for (int i = 0; i < t.Choices.Length; i++)
            {
                string fancyName = t.Choices[i].Perk.HollowInstance.GetFancyName(Parent);

                sb.Append(i + 1);
                sb.Append(" - ");
                sb.Append(fancyName);
                sb.AppendLine();
                br.Append(i + 1);
                br.Append(" - ");
                br.Append(fancyName);
                br.AppendLine(string.Empty);
                sb.AppendLine(t.Choices[i].Perk.HollowInstance.GetDescription(Parent));
                sb.AppendLine();
            }

            brief = br.ToString();
            return sb.ToString();
        }

        public bool Choose(int index, out string name)
        {
            if (Upgrades.Count <= 0)
            {
                name = string.Empty;
                return false;
            }

            Item t = Upgrades.Peek();

            if (index < 0 || index >= t.Choices.Length)
            {
                name = string.Empty;
                return false;
            }

            if (Parent.TryGetPerkInventory(out PerkInventory inv) && inv.TryGetPerk(t.Choices[index].Perk.Perk, out PerkBase? perkBase) && perkBase is UpgradePathPerkBase b)
            {
                if (b.Maxed)
                {
                    name = string.Empty;
                    return false;
                }

                b.Progress++;
            }
            else
                Parent.GivePerk(t.Choices[index].Perk);

            Upgrades.Dequeue();

            name = t.Choices[index].Perk.HollowInstance.GetFancyName(Parent);
            return true;
        }

        public class Item
        {
            public Item(UpgradeQueue queue, params UpgradePathAttribute[] choices)
            {
                Choices = choices;

                if (Core.CoreConfig.ScpUpgradeAutopickTime is 0)
                    return;

                Timing.CallDelayed(Core.CoreConfig.ScpUpgradeAutopickTime, () =>
                {
                    if (queue.Upgrades.Peek() != this)
                        return;
                    queue.Upgrades.Dequeue();

                    int len = choices.Length;
                    if (len > 0)
                    {
                        if (queue.Choose(Random.Range(0, len), out string name))
                        {
                            queue.Parent.SendHint("<align=\"left\">Chosen: " + name + "</align>", [HintEffectPresets.FadeOut()], 10f);
                        }
                    }
                });
            }

            public UpgradePathAttribute[] Choices { get; }
        }
    }
}
