namespace NJade
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public static class FsModule
    {
        private static readonly Dictionary<string, Func<byte[], string>> Decoders = new Dictionary<string, Func<byte[], string>>
        {
            {"ascii", Encoding.ASCII.GetString },
            {"utf8", Encoding.UTF8.GetString },
            {"utf16le", Encoding.GetEncoding("utf-16").GetString },
            {"ucs2", Encoding.GetEncoding("utf-16").GetString },
            {"base64", Convert.ToBase64String },
            {"hex", x => BitConverter.ToString(x).Replace("-","") },
        };

        public static string readFileSync(string path, string encoding = "ascii")
        {
            var bytes = File.ReadAllBytes(path);
            return Decoders[encoding].Invoke(bytes);
        }
    }
}