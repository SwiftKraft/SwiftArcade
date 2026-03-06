namespace SwiftArcadeMode.Features.Humans.Perks
{
    using System;
    using LabApi.Features.Wrappers;
    using MapGeneration;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class PerkSpawnRulesBasic : PerkSpawnRulesBase
    {
        public virtual RoomName[] SpawnRooms => [
            RoomName.Lcz914,
            RoomName.Lcz173,
            RoomName.LczGlassroom,
            RoomName.LczArmory,
            RoomName.EzIntercom,
            RoomName.EzRedroom,
            RoomName.Hcz096,
            RoomName.Hcz127,
            RoomName.HczServers
        ];

        public RoomName[] CachedSpawnRooms => SpawnRooms;

        public virtual Func<PerkAttribute, bool> Criteria => (p) => p.Restriction == PerkRestriction.None || p.Restriction == PerkRestriction.Human;

        public override void SpawnPerks()
        {
            foreach (Room r in Room.List)
            {
                if (r == null || r.Base == null || (Random.Range(0f, 1f) > Mathf.Lerp(0.3f, 0.6f, Mathf.InverseLerp(5, 25, Server.PlayerCount)) && !CachedSpawnRooms.Contains(r.Name)))
                    continue;

                PerkAttribute? perk = PerkManager.GetRandomPerk(Criteria);
                if (perk is null)
                    return;

                Pickup? pick = PerkSpawner.SpawnPerk(perk, r.Position + (Vector3.up * 2f));

                if (pick != null && pick.Rigidbody)
                    pick.Rigidbody.AddForce(Random.insideUnitSphere * 3f);
            }
        }
    }
}
