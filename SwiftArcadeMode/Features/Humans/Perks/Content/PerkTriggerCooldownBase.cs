namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    public abstract class PerkTriggerCooldownBase(PerkInventory inv) : PerkCooldownBase(inv)
    {
        public override string ReadyMessage => "Triggered!";

        public override void Tick()
        {
            base.Tick();
            Trigger();
        }
    }
}
