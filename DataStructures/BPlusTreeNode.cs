namespace Proyecto1_biblioteca.DataStructures
{
    public class BPlusTreeNode
    {
        public bool IsLeaf;
        public int Numkeys;
        public int[] Keys;
        public object[] ChildrenOrData;
        public BPlusTreeNode Next;

        public BPlusTreeNode(bool isLeaf, int order)
        {
            IsLeaf = isLeaf;
            Numkeys = 0;
            Keys = new int[order];
            ChildrenOrData = new object[order + 1];
            Next = null;
        }
    }
}