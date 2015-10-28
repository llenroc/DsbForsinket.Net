using System.Collections.Generic;
using System.Linq;

namespace DsbForsinket.Common
{
    public static class Tags
    {
        private const int Buckets = 10;

        public static IEnumerable<string> BucketsFromTag(string tag)
        {
            // Adding some random suffix to the time tags to workaround the 10k limit per tag
            return Enumerable.Range(0, Buckets).Select(i => $"{tag}-{i}");
        }
    }
}
