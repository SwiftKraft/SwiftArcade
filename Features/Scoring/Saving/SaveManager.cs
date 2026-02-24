namespace SwiftArcadeMode.Features.Scoring.Saving
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using LabApi.Features.Console;
    using LabApi.Features.Wrappers;

    public static class SaveManager
    {
        public static string GeneralDirectory { get; set; } = string.Empty;

        public static string SavePath { get; set; } = string.Empty;

        public static string SaveFileName { get; set; } = string.Empty;

        public static string SaveDirectory { get; set; } = string.Empty;

        public static Player? IDToPlayer(string id) => Player.Get(id);

        public static void SaveScores()
        {
            StringBuilder stringBuilder = new();

            foreach (KeyValuePair<string, int> kvp in ScoringManager.Scores)
            {
                stringBuilder.Append(kvp.Key);
                stringBuilder.Append(';');
                stringBuilder.Append(kvp.Value);
                stringBuilder.Append(';');
                stringBuilder.Append(ScoringManager.IDToName[kvp.Key]);
                stringBuilder.Append('\n');
            }

            try
            {
                if (!Directory.Exists(SaveDirectory))
                    Directory.CreateDirectory(SaveDirectory);

                File.WriteAllText(SavePath, stringBuilder.ToString());
            }
            catch
            {
                // ignored
            }

            Logger.Info("Saved all scores!");
        }

        public static void LoadScores()
        {
            if (!File.Exists(SavePath))
                return;

            string[] lines = File.ReadAllLines(SavePath);

            ScoringManager.Scores.Clear();
            ScoringManager.IDToName.Clear();

            foreach (string line in lines)
            {
                try
                {
                    string[] split = line.Split(';');
                    ScoringManager.Scores.Add(split[0], int.Parse(split[1]));
                    ScoringManager.IDToName.Add(split[0], split[2]);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            Logger.Info("Loaded all scores!");
        }
    }
}
