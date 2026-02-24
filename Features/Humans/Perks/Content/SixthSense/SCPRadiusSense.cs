namespace SwiftArcadeMode.Features.Humans.Perks.Content.SixthSense
{
    using System.Collections.Generic;
    using System.Linq;
    using LabApi.Features.Wrappers;
    using MapGeneration;
    using PlayerRoles;
    using SwiftArcadeMode.Utils.Extensions;

    public class SCPRadiusSense(SixthSense parent) : SenseBase(parent)
    {
        private int lastRand1;
        private int lastRand2;
        private int lastRand3;

        public static Dictionary<RoleTypeId, string[]> SCPNearbyMessages { get; } = new()
        {
            {
                RoleTypeId.Scp106,
                [
                    "The floor feels softer. Or deeper.",
                    "The walls breathe with a rhythm not your own.",
                    "The walls shift when you're not looking. Or was it only an illusion?",
                    "Rust spreads in your memory like mold on old meat.",
                    "Your joints ache with a pain that feels... borrowed.",
                    "The walls weep dark stains where no pipe could reach.",
                    "You remember dying, but not the body you died in.",
                    "Every breath smells like soil after a fresh grave."
                ]
            },
            {
                RoleTypeId.Scp173,
                [
                    "Your shadow twitches a half-beat behind you.",
                    "You try to blink something out of your vision—but your eyes were already closed.",
                    "You swore that you saw something jump across your vision.",
                    "Every light source is just slightly wrong in color temperature.",
                    "The lights dim, but the bulbs don't flicker.",
                    "You blink—and something is closer.",
                    "Your spine tenses like it expects a snap.",
                    "You feel watched the moment you turn your back.",
                    "Air displaces behind you. There was no breeze.",
                    "Every muscle wants to lock, but it’s not your call."
                ]
            },
            {
                RoleTypeId.Scp096,
                [
                    "Your footsteps echo with a delay too long to be real.",
                    "Something skitters across your mind—too quick to name, too loud to forget.",
                    "A scream hides behind your breath, waiting to escape.",
                    "Grief not your own tightens around your ribs.",
                    "You saw something. You don't remember. But it saw you.",
                    "Tears sting your eyes for reasons your mind denies.",
                    "Your heartbeat skips—then races to catch up with itself."
                ]
            },
            {
                RoleTypeId.Scp049,
                [
                    "A taste like static clings to the back of your tongue.",
                    "Your teeth itches.",
                    "Breathing becomes deliberate. Forgetting to inhale feels... dangerous.",
                    "Your name doesn't sound right when you think it.",
                    "Your blood feels thick—like it’s learning to clot.",
                    "You smell alcohol and ash where there is no fire.",
                    "Something tugs beneath your skin, charting infection.",
                    "The scent of rot perfumes your thoughts.",
                    "A hand reaches for your pulse—but doesn’t feel human."
                ]
            },
            {
                RoleTypeId.Scp3114,
                [
                    "You feel you're being studied by something that doesn't blink.",
                    "Your thoughts begin to echo, as if someone else is rehearsing them.",
                    "Something skitters across your mind—too quick to name, too loud to forget.",
                    "You try to blink something out of your vision—but your eyes were already closed.",
                    "Your name doesn't sound right when you think it.",
                    "You feel like the next person you see might not be a real person.",
                    "You don't remember your face quite right.",
                    "A memory surfaces—but it belongs to someone else.",
                    "Your limbs respond a half-second late.",
                    "Someone else is breathing through your lungs.",
                    "You hear your voice whisper—but your mouth is shut."
                ]
            },
            {
                RoleTypeId.Scp939,
                [
                    "The ventilation hum becomes a melody—one you shouldn't recognize.",
                    "\"Help me,\" echoes from an empty hall.",
                    "The sound of your own voice calls you from the dark.",
                    "Your breath fogs—but the air is warm.",
                    "You hear chewing—wet, deliberate, close.",
                    "Something mimics your footsteps one beat behind.",
                    "You hear imaginary footsteps trailing you."
                ]
            },
        };

        public static string[] PocketDimensionMessages { get; } =
        [
            "The floor bends like wet paper under your weight.",
            "Something skitters beneath your skin. It's wearing your voice.",
            "There are pathways here, one of them might be an exit...",
            "Time loops—but it never repeats the same way twice.",
            "You try to scream, but your mouth is filled with rust.",
            "Walls drip with memories that aren't yours.",
            "The air crackles like old bone. Breathing feels optional—and wrong.",
            "A clock ticks in reverse. It's inside your chest.",
            "Every surface wants to swallow you. Some already have."
        ];

        public static string[] SCPMessages { get; } =
        [
            "You are the danger.",
            "You crave for the sight of a human.",
            "You don't feel anything but hunger.",
            "An unknown voice commands you to slaughter.",
            "The killing is not pointless, it is necessary.",
            "Slaughter seems to be the only way to prove your existance.",
            "You don't feel your own breathing.",
            "Survival is only wishful thinking.",
            "Bloodlust fills your veins.",
            "The humans' futile struggle is what excites you."
        ];

        public virtual float Range => 30f;

        public override bool Message(out string? msg)
        {
            if (Player.IsSCP)
            {
                msg = SCPMessages.GetRandom(ref lastRand1);
                return true;
            }

            if (Player.Room?.Name == RoomName.Pocket)
            {
                msg = PocketDimensionMessages.GetRandom(ref lastRand2);
                return true;
            }

            Player? scp = GetNearSCPs();
            msg = scp != null ? SCPNearbyMessages[scp.Role].GetRandom(ref lastRand3) : string.Empty;
            return scp != null;
        }

        public Player? GetNearSCPs() => Player.List.FirstOrDefault(p => p.IsSCP && SCPNearbyMessages.ContainsKey(p.Role) && (p.Position - Player.Position).sqrMagnitude <= Range * Range);
    }
}
