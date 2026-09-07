namespace BibliotecaEstructuras
{
    public class BPlusTreeNode
    {
        public bool IsLeaf;
        public int NumKeys;
        public int[] Keys;
        public object[] ChildrenOrData; // Punteros a hijos (BPlusTreeNode) o datos (Libro)
        public BPlusTreeNode Next;     // Enlace de lista enlazada para hojas

        public BPlusTreeNode(bool isLeaf, int order)
        {
            IsLeaf = isLeaf;
            NumKeys = 0;
            Keys = new int[order];
            ChildrenOrData = new object[order + 1];
            Next = null;
        }
    }
}