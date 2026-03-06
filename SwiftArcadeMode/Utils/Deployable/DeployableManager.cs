namespace SwiftArcadeMode.Utils.Deployable
{
    using System.Collections.Generic;
    using LabApi.Features.Console;

    public static class DeployableManager
    {
        public static readonly List<DeployableBase> AllDeployables = [];

        public static void Tick()
        {
            for (int i = 0; i < AllDeployables.Count; i++)
            {
                try
                {
                    AllDeployables[i].Tick();
                }
                catch (System.Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }
    }
}
