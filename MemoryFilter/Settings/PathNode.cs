using System;
using System.Collections.Generic;
using System.Linq;
using MemoryFilter.Domain;

namespace MemoryFilter.Settings {

    public class PathNode {
        private readonly List<PathNode> _children = new List<PathNode>();

        private readonly List<IMediaFile> _files = new List<IMediaFile>();
        private readonly string _title;

        public PathNode(params string[] paths) {
            _title = paths.Last();
            IsRoot = paths.Length == 1;
            FullPath = string.Join(@"\", paths);
            ParentPath = string.Join(@"\", paths.Take(paths.Length - 1));
        }

        public string FullPath { get; }
        public string ParentPath { get; }
        public IEnumerable<PathNode> Children => _children;
        public IEnumerable<IMediaFile> Files => _files;

        public bool IsRoot { get; }

        public bool IsParentOf(PathNode node) {
            return string.Compare(node.ParentPath, FullPath, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public void SetChildren(IEnumerable<PathNode> nodes) {
            _children.Clear();
            _children.AddRange(nodes);
        }

        public void SetFiles(IEnumerable<IMediaFile> files) {
            _files.Clear();
            _files.AddRange(files);
        }

        protected bool Equals(PathNode other) {
            return string.Equals(FullPath, other.FullPath);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((PathNode) obj);
        }

        public override int GetHashCode() {
            return FullPath != null ? FullPath.GetHashCode() : 0;
        }

        public IMediaItem GetMediaItem() {
            var items = new List<IMediaItem>(_children.Count + _files.Count);
            foreach (var child in _children) {
                items.Add(child.GetMediaItem());
            }

            items.AddRange(_files);
            return new MediaDirectory(_title, FullPath, items.ToArray());
        }
    }

}