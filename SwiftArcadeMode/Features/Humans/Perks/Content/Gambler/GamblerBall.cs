namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using LabApi.Features.Wrappers;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class GamblerBall : GamblerEffectBase
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "I really like deez balls...";

        public override void Effect(Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                Pickup? pickup = Pickup.Create(ItemType.SCP018, player.Position);

                if (pickup is null)
                    continue;

                pickup.Spawn();
                pickup.Rigidbody?.AddForce(Random.insideUnitSphere * Random.Range(5f, 40f), ForceMode.Impulse);
            }
        }
    }
}
