namespace SwiftArcadeMode.Utils.Extensions
{
    using System;

    public static class StringExtensions
    {
        [Obsolete("Will be removed in favor of SchematicsDirectory")]
        public static string ApplySchematicPrefix(this string schematicName) => Core.CoreConfig.SchematicPrefix + schematicName;
    }
}
