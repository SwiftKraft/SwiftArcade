namespace SwiftArcadeMode.Features.Humans.Perks.Content.SixthSense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Extensions;
    using ReflectionExtensions = SwiftArcadeMode.Utils.Extensions.ReflectionExtensions;

    [Perk("SixthSense", Rarity.Uncommon)]
    public class SixthSense(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public static HashSet<Type> SenseCache => field ??= ReflectionExtensions.GetAllNonAbstractSubclasses<SenseBase>();

        public List<SenseBase> Senses { get; } = [];

        public override string Name => "Sixth Sense";

        public override string Description => "Provides obscure, but useful information regarding enemies.";

        public override string PerkDescription => string.Empty;

        public override string ReadyMessage
        {
            get
            {
                List<SenseBase> toCheck = new(Senses);

                SenseBase? sense = toCheck.GetRandom();
                if (sense is null)
                    return string.Empty;

                while (!sense.Message(out _))
                {
                    toCheck.Remove(sense);
                    sense = toCheck.GetRandom();
                    if (sense is null)
                        return string.Empty;
                }

                return sense.Message(out string? m) ? m : string.Empty;
            }
        }

        public static void RegisterSenses()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            List<Type> types = [.. callingAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(SenseBase).IsAssignableFrom(t))];

            foreach (Type t in types)
            {
                SenseCache.Add(t);
            }
        }

        public override float GetCooldown(Player player) => 10F;

        public override void Init()
        {
            base.Init();
            foreach (Type t in SenseCache)
                Senses.Add((SenseBase)Activator.CreateInstance(t, this));
        }

        public override void Effect()
        {
        }
    }
}
