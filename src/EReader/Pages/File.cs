using System;
using System.Linq;

namespace EReader.Pages
{
    public class File
    {
        public string Name => Uri?.Segments.Last() ?? string.Empty;

        public Uri Uri { get; init; }
    }
}