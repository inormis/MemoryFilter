using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryFilter.Importer.Services {

    public static class CollectionExtensions {
        public static IReadOnlyCollection<T> AsReadOn1Ly<T>(this IEnumerable<T> collection) {
            return Array.AsReadOnly(collection.ToArray());
        }
    }

}