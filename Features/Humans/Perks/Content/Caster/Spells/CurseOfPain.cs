namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using SwiftArcadeMode.Utils.Effects;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    public class CurseOfPain : SpellBase
    {
        public static readonly LayerMask CastMask = LayerMask.GetMask("Default", "Door", "Glass", "Hitbox");

        public override string Name => "Curse of Pain";

        public override Color BaseColor => new(0.5f, 0f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 0.5f;

        public override string[] SoundList => ["cast", "curse"];

        public override void Cast()
        {
            Caster.Player.Damage(10f, "Out of Blood.");
            ShootRay();
            PlaySound("cast");
        }

        public void ShootRay()
        {
            if (!Caster.Player.IsAlive)
                return;

            Vector3 origin = Caster.Player.Camera.position;
            Vector3 direction = Caster.Player.Camera.forward;
            float maxDistance = 30f;

            RaycastHit[] hits = Physics.SphereCastAll(origin, 0.2f, direction, maxDistance, CastMask, QueryTriggerInteraction.Ignore);

            if (hits.Length == 0)
                return;

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            RaycastHit? validHit = null;

            foreach (var hit in hits)
            {
                Transform t = hit.transform;
                if (t == Caster.Player.GameObject.transform || t.IsChildOf(Caster.Player.GameObject.transform))
                    continue;

                validHit = hit;
                break;
            }

            if (!validHit.HasValue)
                return;

            var _hit = validHit.Value;

            if (_hit.collider.transform.TryGetComponentInParent(out ReferenceHub hub) && hub != Caster.Player.ReferenceHub && hub.GetFaction() != Caster.Player.Faction)
            {
                Player.Get(hub)?.AddCustomEffect(new Effect(10f, this));
                PlaySound(hub.transform.position, "curse");
                Caster.Player.SendHitMarker(0.5f);
            }
        }

        public class Effect(float duration) : CustomEffectBase(duration)
        {
            private LightSourceToy light;

            public override int StackCount => 1;

            public readonly CurseOfPain ParentSpell;

            public readonly Timer Ticker = new(0.5f, false);

            public Effect(float duration, CurseOfPain spell) : this(duration) => ParentSpell = spell;

            public override void Add()
            {
                Parent.Player.Damage(20f * (Parent.Player.IsSCP ? 3f : 1f), ParentSpell.Caster.Player, default, 100);
                ParentSpell.Caster.Player.SendHitMarker(1.5f);

                if (Parent.Player.IsAlive)
                {
                    light = LightSourceToy.Create(Parent.Player.Position, null, false);
                    light.Color = Color.red;
                    light.Intensity = 1f;
                    light.SyncInterval = 0f;
                    light.ShadowType = LightShadows.None;
                    light.Spawn();
                }
                else
                    ParentSpell.Caster.Player.Heal(30f);
            }

            public override void Remove() => light?.Destroy();

            public override void Tick()
            {
                Ticker.Tick(Time.fixedDeltaTime);

                if (Ticker.Ended)
                {
                    Ticker.Reset();
                    ParentSpell.Caster.Player.Heal(2f);
                    Parent.Player.Damage(3f * (Parent.Player.IsSCP ? 3f : 1f), ParentSpell.Caster.Player, default, 100);
                    ParentSpell.Caster.Player.SendHitMarker(0.3f);
                }

                if (light != null)
                {
                    light.Position = Parent.Player.Position;
                    light.Intensity = Mathf.Sin(Time.time * 4f) * 2f + 3f;
                }
            }
        }
    }
}