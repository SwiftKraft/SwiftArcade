namespace SwiftArcadeMode.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string ApplySchematicPrefix(this string schematicName) => Core.CoreConfig.SchematicPrefix + schematicName;
    }
}
