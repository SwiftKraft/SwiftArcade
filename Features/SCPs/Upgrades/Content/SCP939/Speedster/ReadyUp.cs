namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Speedster
{
    using System;
    using CustomPlayerEffects;
    using PlayerRoles.PlayableScps.Scp939;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    public class ReadyUp(UpgradePathPerkBase parent) : UpgradeBase<Speedster>(parent)
    {
        private readonly Timer timer = new();
        private Scp939Role role = null!;
        private bool trigger;

        public override string Name => "Ready Up";

        public override string Description => $"Crouching for {Requirement}s will grant you a speed boost for {Duration}s after standing up.";

        public virtual float Duration => 8f;

        public virtual float Requirement => 3f;

        public bool CrouchState => role && role.SubroutineModule.TryGetSubroutine(out Scp939FocusAbility focus) && focus.State > 0.5f;

        public override void Init()
        {
            base.Init();
            if (Player.RoleBase is Scp939Role r)
                role = r;
            else
                throw new InvalidOperationException("ReadyUp perk was applied to a non-939 player!");

            timer.OnTimerEnd += OnTimerEnd;
        }

        public override void Tick()
        {
            base.Tick();

            if (CrouchState)
            {
                float t = Mathf.Round(timer.CurrentValue);
                if (t > 1f)
                    SendMessage("Time until boost: " + t + "s");
                timer.Tick(Time.fixedDeltaTime);
            }
            else
            {
                if (timer.Ended && trigger)
                {
                    trigger = false;
                    Player.EnableEffect<MovementBoost>(70, Duration);
                }

                timer.Reset(Requirement);
            }
        }

        public override void Remove()
        {
            base.Remove();
            timer.OnTimerEnd -= OnTimerEnd;
        }

        private void OnTimerEnd()
        {
            SendMessage("Speed boost available!");
            trigger = true;
        }
    }
}
