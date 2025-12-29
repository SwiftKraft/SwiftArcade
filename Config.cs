namespace SwiftArcadeMode
{
    public class Config
    {
        public bool AllowBaseContent { get; set; } = true;
        public bool AllowPerkSpawning { get; set; } = true;
        public bool AllowCustomGameModes { get; set; } = true;
        public bool AllowScpLeveling { get; set; } = true;
        public bool SoundLogs { get; set; } = false;
        public bool SpeedUpSchematics { get; set; } = true;
        public bool SkeletonBalance { get; set; } = true;
        public bool Replace096 { get; set; } = true;
        public string SchematicPrefix { get; set; } = "SAM-";
    }
}
