using System.IO;

namespace TraceBot
{
    internal class ResourceReader
    {
        internal string Read(string filename)
        {
            var assembly = GetType().Assembly;
            var resourceName = $"TraceBot.Client.{filename}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
