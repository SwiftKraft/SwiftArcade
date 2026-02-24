namespace SwiftArcadeMode.Utils.Projectiles
{
    using System.Collections.Generic;

    public static class ProjectileManager
    {
        public static readonly List<ProjectileBase> All = [];

        public static void Tick()
        {
            for (int i = 0; i < All.Count; i++)
                All[i].Tick();
        }
    }
}
