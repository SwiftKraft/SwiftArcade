namespace SwiftArcadeMode.Features.Scoring.Saving
{
    using System;
    using System.IO;
    using System.Text;
    using LabApi.Features.Console;
    using LabApi.Features.Wrappers;

    public static class SaveManager
    {
        public static string GeneralDirectory;
        public static string SavePath;
        public static string SaveFileName;
        public static string SaveDirectory;

        public static Player IDToPlayer(string id) => Player.Get(id);

        public static void SaveScores()
        {
            StringBuilder stringBuilder = new();

            foreach (var s in ScoringManager.Scores)
            {
                stringBuilder.Append(s.Key);
                stringBuilder.Append(';');
                stringBuilder.Append(s.Value);
                stringBuilder.Append(';');
                stringBuilder.Append(ScoringManager.IDToName[s.Key]);
                stringBuilder.Append('\n');
            }

            try
            {
                if (!Directory.Exists(SaveDirectory))
                    Directory.CreateDirectory(SaveDirectory);

                File.WriteAllText(SavePath, stringBuilder.ToString());
            }
            catch { }
            Logger.Info("Saved all scores!");
        }

        public static void LoadScores()
        {
            if (!File.Exists(SavePath))
                return;

            string[] str = File.ReadAllLines(SavePath);

            ScoringManager.Scores.Clear();
            ScoringManager.IDToName.Clear();

            foreach (var s in str)
            {
                try
                {
                    string[] split = s.Split(';');
                    ScoringManager.Scores.Add(split[0], int.Parse(split[1]));
                    ScoringManager.IDToName.Add(split[0], split[2]);
                }
                catch (Exception e) { Logger.Error(e); }
            }

            Logger.Info("Loaded all scores!");
        }
    }
}
