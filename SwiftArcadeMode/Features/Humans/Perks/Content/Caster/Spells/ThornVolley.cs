namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using MEC;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class ThornVolley : SpellBase
    {
        private CoroutineHandle coroutine;

        public ThornVolley(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Thorn Volley";

        public override Color BaseColor => new(0f, 0.4f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 0.5f;

        public override void Cast()
        {
            new ThornShot.Projectile(this, Caster.Player, Caster.Player.Camera.position, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 60f).Init();
            PlaySound("cast");

            coroutine = Timing.CallPeriodically(0.61f, 0.025f, () =>
            {
                if (!Caster.Player.IsAlive)
                {
                    Timing.KillCoroutines(coroutine);
                    return;
                }

                PlaySound("cast");
                new ThornShot.Projectile(this, Caster.Player, Caster.Player.Camera.position + (Caster.Player.Camera.forward * 0.4f) + (Random.insideUnitSphere * 0.3f), Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 60f).Init();
            });
        }
    }
}
