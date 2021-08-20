using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Program.Exceptions;
using Program.Utils;

namespace Program
{
    public class IdIndex : IComparable<IdIndex>
    {
        public static readonly int MaxTreeDegree = 3;
        public static readonly int MinTreeDegree = 2;

        public int LeafChildrenCount
        {
            get
            {
                if (ChildNodes.First().IsLeaf)
                {
                    return ChildNodes.Count;
                }
                else
                {
                    return ChildNodes.Sum(node => node.LeafChildrenCount);
                }
            }
        }

        public int ChildNodesCount
        {
            get => ChildNodes.Count;
        }

        public int MinLeafChildrenCount
        {
            get
            {
                if (!IsLeaf)
                {
                    return ChildNodes.Min(node => node.MinLeafChildrenCount);
                }
                else
                {
                    return 999;
                }
            }
        }

        public int Level
        {
            get
            {
                if (IsLeaf)
                {
                    return 1;
                }

                return ChildNodes.Max(node => node.Level) + 1;
            }
        }

        public int ElementsCount
        {
            get
            {
                if (IsLeaf)
                {
                    return IdList.Count;
                }

                return ChildNodes.Sum(node => node.ElementsCount);
            }
        }

        public bool IsLeaf
        {
            get => ChildNodes.Count == 0;
        }

        public long MaxBorder => Borders.Max();
        public long MinBorder => Borders.Min();
        public IdIndex Parent { get; set; }

        public SortedSet<long> Borders { get; set; } = new()
        {
            DbConfig.MAX_ID,
            DbConfig.MIN_ID
        };

        public SortedSet<IdIndex> ChildNodes { get; } = new();
        public int MaxElements { get; } = 4;
        public SortedSet<long> IdList { get; set; } = new();
        public string DataFileName { get; set; }

        protected IdIndex(string dataFileName, int maxElements)
        {
            MaxElements = maxElements;
            DataFileName = dataFileName;
        }

        // leaf index
        protected IdIndex(SortedSet<long> borders, SortedSet<long> idList, int maxElements, IdIndex parent = null)
            : this(borders, idList, DirUtils.GenerateFilename(), maxElements, parent)
        {
        }

        // leaf index
        protected IdIndex(SortedSet<long> borders, SortedSet<long> idList, string dataFileName, int maxElements,
            IdIndex parent = null)
            : this(dataFileName, maxElements)
        {
            Parent = parent;
            Borders = borders;
            IdList = idList;
        }

        // non-leaf index
        protected IdIndex(SortedSet<IdIndex> childNodes, SortedSet<long> borders, int maxElements,
            IdIndex parent = null)
            : this(childNodes, borders, DirUtils.GenerateFilename(), maxElements, parent)
        {
        }

        // non-leaf index
        protected IdIndex(SortedSet<IdIndex> childNodes, SortedSet<long> borders, string dataFileName, int maxElements,
            IdIndex parent = null) : this(dataFileName, maxElements)
        {
            Parent = parent;
            Borders = borders;
            foreach (var childNode in childNodes)
            {
                AddChild(childNode);
            }
        }

        // root index
        public IdIndex(int maxElements = 20) : this(DirUtils.GenerateFilename(), maxElements)
        {
        }

        public List<string> GetAllIndexesFileNames()
        {
            var fileNamesList = new List<string>();
            if (IsLeaf)
            {
                return new List<string> {DataFileName};
            }
            else
            {
                foreach (var childNode in ChildNodes)
                {
                    fileNamesList.AddRange(childNode.GetAllIndexesFileNames());
                }
            }

            return fileNamesList;
        }

        public bool ContainsId(long dataUnitId)
        {
            var isInRange = IsInRange(dataUnitId);
            if (isInRange && IsLeaf)
            {
                return IdList.Contains(dataUnitId);
            }
            else if (isInRange && !IsLeaf)
            {
                foreach (var childNode in ChildNodes)
                {
                    if (childNode.IsInRange(dataUnitId))
                    {
                        return childNode.ContainsId(dataUnitId);
                    }
                }
            }
            return false;
        }

        public string FindDataFileNameByUnitId(long dataUnitId)
        {
            var isInRange = IsInRange(dataUnitId);
            if (isInRange && IsLeaf)
            {
                return DataFileName;
            }
            else if (isInRange && !IsLeaf)
            {
                foreach (var childNode in ChildNodes)
                {
                    if (childNode.IsInRange(dataUnitId))
                    {
                        return childNode.FindDataFileNameByUnitId(dataUnitId);
                    }
                }
            }

            throw IndexRangeException.GenerateIdNotFoundException(this, dataUnitId);
        }

        public bool IsInRange(long id)
        {
            return id <= MaxBorder && id >= MinBorder;
        }

        public Dictionary<string, SortedSet<long>> Divide()
        {
            if (Parent != null || IsLeaf)
            {
                var firstIndexIds = new SortedSet<long>(IdList.Take(MaxElements / 2));
                var firstIndexBorders = new SortedSet<long>()
                {
                    MinBorder,
                    firstIndexIds.Max()
                };

                var secIndexIds = new SortedSet<long>(IdList.Where(id => !firstIndexIds.Contains(id)));
                var secIndexBorders = new SortedSet<long>()
                {
                    firstIndexIds.Max() + 1,
                    MaxBorder
                };

                var firstIndex = new IdIndex(firstIndexBorders, firstIndexIds, MaxElements);
                var secIndex = new IdIndex(secIndexBorders, secIndexIds, MaxElements);

                if (Parent != null)
                {
                    Parent.ChildNodes.Remove(this);
                    Parent.AddChild(firstIndex);
                    Parent.AddChild(secIndex);
                }
                else if (IsLeaf)
                {
                    AddChild(firstIndex);
                    AddChild(secIndex);
                    IdList.Clear();
                }

                return new Dictionary<string, SortedSet<long>>()
                {
                    {secIndex.DataFileName, secIndex.IdList},
                    {firstIndex.DataFileName, firstIndex.IdList}
                };
            }

            throw new Exception($"Can't divide index [{MinBorder}..{MaxBorder}] no parent or index isn't leaf");
        }

        public void AddChild(IdIndex childIndex)
        {
            ChildNodes.Add(childIndex);
            childIndex.Parent = this;
            Borders = new SortedSet<long>()
            {
                Math.Max(MaxBorder, ChildNodes.Max(node => node.MaxBorder)),
                Math.Min(MinBorder, ChildNodes.Min(node => node.MinBorder))
            };
            if (ChildNodes.Count == MaxTreeDegree + 1)
            {
                var newParent = new IdIndex();

                var leftNodes = new SortedSet<IdIndex>(ChildNodes.Take(MinTreeDegree));
                var newLeftIndex = new IdIndex(leftNodes, GetBordersFromNodes(leftNodes), MaxElements);

                var rightNodes = new SortedSet<IdIndex>(ChildNodes.Except(leftNodes));
                var newRightIndex = new IdIndex(rightNodes, GetBordersFromNodes(rightNodes), MaxElements);

                if (Parent == null)
                {
                    newParent.AddChild(newLeftIndex);
                    newParent.AddChild(newRightIndex);
                    Parent = newParent;
                }
                else
                {
                    Parent.ChildNodes.Remove(this);
                    Parent.AddChild(newLeftIndex);
                    Parent.AddChild(newRightIndex);
                }
            }
        }

        public void Unite(IdIndex emptyIndex)
        {
            ChildNodes.Remove(emptyIndex);
            var rootIndex = this;
            while (rootIndex.Parent != null)
            {
                rootIndex = rootIndex.Parent;
            }

            if (ChildNodes.Count == MinTreeDegree)
            {
                var nearestNonEmptyIndex = FindNearestIndex(this, emptyIndex);
                var indexesToMerge = new List<IdIndex>()
                {
                    nearestNonEmptyIndex,
                    emptyIndex
                };
                var mergedIndex = MergeLeafIndexes(this, indexesToMerge, nearestNonEmptyIndex.DataFileName);
                ChildNodes.Remove(nearestNonEmptyIndex);
                ChildNodes.Add(mergedIndex);
            }
            else if (ChildNodes.Count == 1 && Parent != null)
            {
                var indexesToMerge = new List<IdIndex>()
                {
                    ChildNodes.First(),
                    emptyIndex
                };
                var mergedIndex = MergeLeafIndexes(this, indexesToMerge, ChildNodes.First().DataFileName);
                ChildNodes.Clear();
                ChildNodes.Add(mergedIndex);

                var singleChildIndex = this;
                while (singleChildIndex != null && singleChildIndex.Parent != null)
                {
                    CheckStructureAfterUnite(rootIndex, singleChildIndex);
                    singleChildIndex = FindSingleChildNodeParent(rootIndex);
                    while (singleChildIndex != null &&
                           (singleChildIndex.Parent == null || singleChildIndex.Parent.ChildNodesCount == 1))
                    {
                        var childs = singleChildIndex.ChildNodes.First().ChildNodes;
                        singleChildIndex.ChildNodes.Clear();
                        foreach (var child in childs)
                        {
                            singleChildIndex.AddChild(child);
                        }
                        singleChildIndex = FindSingleChildNodeParent(rootIndex);
                    }
                }
            }
            else if (ChildNodes.Count == 1)
            {
                DataFileName = ChildNodes.First().DataFileName;
                IdList = ChildNodes.First().IdList;
                ChildNodes.Clear();
            }
        }

        protected void CheckStructureAfterUnite(IdIndex rootIndex, IdIndex singleChildIndex)
        {
            var indexToMerge = singleChildIndex.ChildNodes.First();
            var nearestForSingleChildIndex = FindNearestIndex(rootIndex, indexToMerge);
            var nearestForSingleChildIndexParent = nearestForSingleChildIndex.Parent;

            if (nearestForSingleChildIndexParent.ChildNodesCount == MinTreeDegree)
            {
                singleChildIndex.Parent.ChildNodes.Remove(singleChildIndex);
                nearestForSingleChildIndexParent.AddChild(indexToMerge);
            }
            else if (nearestForSingleChildIndexParent.ChildNodesCount == MaxTreeDegree)
            {
                nearestForSingleChildIndexParent.ChildNodes.Remove(nearestForSingleChildIndex);
                singleChildIndex.AddChild(nearestForSingleChildIndex);
            }
            rootIndex.RestoreAllBorders();
        }

        private void RestoreAllBorders()
        {
            foreach (var node in ChildNodes)
            {
                if (!node.IsLeaf)
                {
                    node.RestoreAllBorders();
                    node.RestoreBordersByChildNodes();
                }
            }
        }

        protected IdIndex FindNearestIndex(IdIndex from, IdIndex index)
        {
            IdIndex nearestIndex;
            if (index.MinBorder <= from.MinBorder)
            {
                nearestIndex = FindNextIndex(from, index);
            }
            else
            {
                nearestIndex = FindPreviousIndex(from, index);
            }

            if (nearestIndex == null)
            {
                throw IndexRangeException.GenerateNearestIndexNotFoundException(from, index);
            }

            return nearestIndex;
        }

        protected IdIndex FindNextIndex(IdIndex from, IdIndex index)
        {
            foreach (var childNode in from.ChildNodes)
            {
                if (childNode.Level == index.Level && childNode.MinBorder == index.MaxBorder + 1 && !childNode.Equals(index))
                {
                    return childNode;
                }
                else if (!childNode.IsLeaf && childNode.MaxBorder > index.MaxBorder)
                {
                    var ind = childNode.FindNextIndex(childNode, index);
                    if (ind != null)
                    {
                        return ind;
                    }
                }
            }

            return null;
        }

        protected IdIndex FindPreviousIndex(IdIndex from, IdIndex index)
        {
            foreach (var childNode in from.ChildNodes)
            {
                if (childNode.Level == index.Level && childNode.MaxBorder == index.MinBorder - 1 && !childNode.Equals(index))
                {
                    return childNode;
                }
                else if (!childNode.IsLeaf && childNode.MinBorder < index.MinBorder)
                {
                    var ind = childNode.FindPreviousIndex(childNode, index);
                    if (ind != null)
                    {
                        return ind;
                    }
                }
            }

            return null;
        }

        public IndexDivideRequest AddDataUnitIndex(long dataUnitId)
        {
            if (IsInRange(dataUnitId))
            {
                if (IsLeaf && ElementsCount < MaxElements)
                {
                    IdList.Add(dataUnitId);
                    return null;
                }
                else if (IsLeaf && IdList.Count == MaxElements)
                {
                    IdList.Add(dataUnitId);
                    return new IndexDivideRequest(DataFileName, Divide());
                }
                else if (!IsLeaf)
                {
                    foreach (var childNode in ChildNodes)
                    {
                        if (childNode.IsInRange(dataUnitId))
                        {
                            return childNode.AddDataUnitIndex(dataUnitId);
                        }
                    }
                }
            }

            throw IndexRangeException.GenerateIdNotFoundException(this, dataUnitId);
        }

        public IdIndex RemoveDataUnitIndex(long dataUnitId)
        {
            if (IsInRange(dataUnitId))
            {
                if (IsLeaf)
                {
                    IdList.Remove(dataUnitId);
                    if (ElementsCount == 0 && Parent != null)
                    {
                        Parent.Unite(this);
                    }
                    return this;
                }
                else if (!IsLeaf)
                {
                    foreach (var childNode in ChildNodes)
                    {
                        if (childNode.IsInRange(dataUnitId))
                        {
                            return childNode.RemoveDataUnitIndex(dataUnitId);
                        }
                    }
                }
            }

            throw IndexRangeException.GenerateIdNotFoundException(this, dataUnitId);
        }

        protected void RestoreBordersByChildNodes()
        {
            if (!IsLeaf)
            {
                Borders = GetBordersFromNodes(ChildNodes);
            }
        }

        public List<byte> Serialize()
        {
            var bytes = new List<byte>();
            bytes.Add(SerializeUtils.IntToByte(MaxElements));
            bytes.AddRange(SerializeUtils.StringToBytes(DataFileName));
            bytes.Add(SerializeUtils.BoolToByte(IsLeaf));
            bytes.Add(SerializeUtils.IntToByte(Borders.Count));
            foreach (var border in Borders)
            {
                bytes.AddRange(SerializeUtils.LongToBytes(border));
            }

            if (!IsLeaf)
            {
                bytes.Add(SerializeUtils.IntToByte(ChildNodes.Count));
                foreach (var childNode in ChildNodes)
                {
                    bytes.AddRange(childNode.Serialize());
                }
            }
            else
            {
                bytes.Add(SerializeUtils.IntToByte(IdList.Count));
                foreach (var id in IdList)
                {
                    bytes.AddRange(SerializeUtils.LongToBytes(id));
                }
            }

            return bytes;
        }

        public static IdIndex Deserialize(FileStream fileStream)
        {
            var maxElements = SerializeUtils.ReadNextInt(fileStream);
            var fileName = SerializeUtils.ReadNextString(fileStream);
            var isLeaf = SerializeUtils.ReadNextBool(fileStream);
            var bordersCount = SerializeUtils.ReadNextInt(fileStream);
            var borders = new SortedSet<long>();
            for (var i = 0; i < bordersCount; i++)
            {
                borders.Add(SerializeUtils.ReadNextLong(fileStream));
            }

            if (!isLeaf)
            {
                var childNodesCount = SerializeUtils.ReadNextInt(fileStream);
                var index = new IdIndex(borders, new SortedSet<long>(), fileName, maxElements);
                for (var i = 0; i < childNodesCount; i++)
                {
                    var newIndex = Deserialize(fileStream);
                    index.AddChild(newIndex);
                }

                return index;
            }
            else
            {
                var idsCount = SerializeUtils.ReadNextInt(fileStream);
                var idList = new SortedSet<long>();
                for (var i = 0; i < idsCount; i++)
                {
                    idList.Add(SerializeUtils.ReadNextLong(fileStream));
                }

                return new IdIndex(borders, idList, fileName, maxElements);
            }
        }

        public IdIndex FindSingleChildNodeParent(IdIndex from)
        {
            IdIndex nodeToUnite = null;
            if (from.ChildNodesCount > 1)
            {
                foreach (var node in from.ChildNodes)
                {
                    nodeToUnite = node.FindSingleChildNodeParent(node);
                    if (nodeToUnite != null)
                    {
                        break;
                    }
                }
            }
            else if (!from.IsLeaf)
            {
                return from;
            }

            return nodeToUnite;
        }

        public SortedSet<long> GetBordersFromNodes(SortedSet<IdIndex> nodes)
        {
            return new()
            {
                nodes.Min(node => node.MinBorder),
                nodes.Max(node => node.MaxBorder)
            };
        }

        protected IdIndex MergeLeafIndexes(IdIndex parent, List<IdIndex> indexesToMerge, string filename)
        {
            if (indexesToMerge.Any(node => node.ChildNodesCount > 0))
            {
                throw IndexUniteException.GenerateIndexMergeException();
            }
            var newIndexIds = new SortedSet<long>();
            foreach (var uniteNode in indexesToMerge)
            {
                newIndexIds.UnionWith(uniteNode.IdList);
            }

            var newBorders = new SortedSet<long>()
            {
                indexesToMerge.Min(index => index.MinBorder),
                indexesToMerge.Max(index => index.MaxBorder)
            };
            var mergedIndex = new IdIndex(newBorders, newIndexIds, filename, MaxElements, parent);
            return mergedIndex;
        }

        public bool Equals(IdIndex other)
        {
            return DataFileName == other.DataFileName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IdIndex) obj);
        }

        public override int GetHashCode()
        {
            return (DataFileName != null ? DataFileName.GetHashCode() : 0);
        }

        public int CompareTo(IdIndex other)
        {
            var thisBordersSum = new BigInteger(MaxBorder + MinBorder);
            var otherBordersSum = new BigInteger(other.MaxBorder + other.MinBorder);
            return thisBordersSum.CompareTo(otherBordersSum);
        }
    }
}