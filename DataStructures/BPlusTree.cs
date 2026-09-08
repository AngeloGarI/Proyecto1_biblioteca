using System;

namespace BibliotecaEstructuras
{
    public class BPlusTree
    {
        private readonly int order;
        public BPlusTreeNode Root { get; private set; }

        public BPlusTree(int order = 4)
        {
            this.order = order;
            Root = new BPlusTreeNode(true, order);
        }

        public Libro Buscar(int codigo)
        {
            return BuscarRecursivo(Root, codigo);
        }

        private Libro BuscarRecursivo(BPlusTreeNode node, int codigo)
        {
            int i = 0;
            while (i < node.NumKeys && codigo > node.Keys[i])
                i++;

            if (node.IsLeaf)
            {
                if (i < node.NumKeys && node.Keys[i] == codigo)
                    return (Libro)node.ChildrenOrData[i];
                return null;
            }

            return BuscarRecursivo((BPlusTreeNode)node.ChildrenOrData[i], codigo);
        }

        public void Insertar(Libro libro)
        {
            BPlusTreeNode r = Root;
            if (r.NumKeys == order - 1)
            {
                BPlusTreeNode s = new BPlusTreeNode(false, order);
                Root = s;
                s.ChildrenOrData[0] = r;
                DividirHijo(s, 0, r);
                InsertarNoLleno(s, libro);
            }
            else
            {
                InsertarNoLleno(r, libro);
            }
        }

        private void InsertarNoLleno(BPlusTreeNode node, Libro libro)
        {
            int i = node.NumKeys - 1;

            if (node.IsLeaf)
            {
                while (i >= 0 && libro.Codigo < node.Keys[i])
                {
                    node.Keys[i + 1] = node.Keys[i];
                    node.ChildrenOrData[i + 1] = node.ChildrenOrData[i];
                    i--;
                }
                node.Keys[i + 1] = libro.Codigo;
                node.ChildrenOrData[i + 1] = libro;
                node.NumKeys++;
            }
            else
            {
                while (i >= 0 && libro.Codigo < node.Keys[i])
                    i--;
                i++;
                BPlusTreeNode child = (BPlusTreeNode)node.ChildrenOrData[i];
                if (child.NumKeys == order - 1)
                {
                    DividirHijo(node, i, child);
                    if (libro.Codigo > node.Keys[i])
                        i++;
                }
                InsertarNoLleno((BPlusTreeNode)node.ChildrenOrData[i], libro);
            }
        }

        private void DividirHijo(BPlusTreeNode parent, int index, BPlusTreeNode child)
        {
            int t = order / 2;
            BPlusTreeNode z = new BPlusTreeNode(child.IsLeaf, order);
            
            if (child.IsLeaf)
            {
                z.NumKeys = child.NumKeys - t;
                for (int j = 0; j < z.NumKeys; j++)
                {
                    z.Keys[j] = child.Keys[j + t];
                    z.ChildrenOrData[j] = child.ChildrenOrData[j + t];
                }
                child.NumKeys = t;
                z.Next = child.Next;
                child.Next = z;

                for (int j = parent.NumKeys; j >= index + 1; j--)
                    parent.ChildrenOrData[j + 1] = parent.ChildrenOrData[j];
                parent.ChildrenOrData[index + 1] = z;

                for (int j = parent.NumKeys - 1; j >= index; j--)
                    parent.Keys[j + 1] = parent.Keys[j];
                parent.Keys[index] = z.Keys[0];
                parent.NumKeys++;
            }
            else
            {
                z.NumKeys = t - 1;
                for (int j = 0; j < z.NumKeys; j++)
                {
                    z.Keys[j] = child.Keys[j + t];
                    z.ChildrenOrData[j] = child.ChildrenOrData[j + t];
                }
                z.ChildrenOrData[z.NumKeys] = child.ChildrenOrData[child.NumKeys];

                child.NumKeys = t - 1;

                for (int j = parent.NumKeys; j >= index + 1; j--)
                    parent.ChildrenOrData[j + 1] = parent.ChildrenOrData[j];
                parent.ChildrenOrData[index + 1] = z;

                for (int j = parent.NumKeys - 1; j >= index; j--)
                    parent.Keys[j + 1] = parent.Keys[j];
                parent.Keys[index] = child.Keys[t - 1];
                parent.NumKeys++;
            }
        }
        public bool Eliminar(int codigo)
        {
            BPlusTreeNode leaf = LocalizarHoja(Root, codigo);
            if (leaf == null) return false;

            int index = -1;
            for (int i = 0; i < leaf.NumKeys; i++)
            {
                if (leaf.Keys[i] == codigo)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) return false;

            for (int i = index; i < leaf.NumKeys - 1; i++)
            {
                leaf.Keys[i] = leaf.Keys[i + 1];
                leaf.ChildrenOrData[i] = leaf.ChildrenOrData[i + 1];
            }
            leaf.NumKeys--;
            leaf.ChildrenOrData[leaf.NumKeys] = null;

            return true;
        }

        private BPlusTreeNode LocalizarHoja(BPlusTreeNode node, int codigo)
        {
            if (node == null) return null;
            if (node.IsLeaf) return node;

            int i = 0;
            while (i < node.NumKeys && codigo > node.Keys[i])
                i++;

            return LocalizarHoja((BPlusTreeNode)node.ChildrenOrData[i], codigo);
        }

        public void ImprimirCatalogoOrdenado()
        {
            BPlusTreeNode curr = Root;
            while (curr != null && !curr.IsLeaf)
            {
                curr = (BPlusTreeNode)curr.ChildrenOrData[0];
            }

            if (curr == null)
            {
                Console.WriteLine("El catálogo está vacío.");
                return;
            }

            Console.WriteLine("\n--- CATÁLOGO COMPLETO (ORDENADO POR CÓDIGO/HOJAS B+) ---");
            while (curr != null)
            {
                for (int i = 0; i < curr.NumKeys; i++)
                {
                    Libro lib = (Libro)curr.ChildrenOrData[i];
                    Console.WriteLine(lib);
                }
                curr = curr.Next;
            }
        }

        // ==========================================
        // ORDENAMIENTO POR TÍTULO (QUICKSORT PROPIO)
        // ==========================================
        public void ImprimirPorTitulo()
        {
            int total = 0;
            BPlusTreeNode curr = Root;
            while (curr != null && !curr.IsLeaf) curr = (BPlusTreeNode)curr.ChildrenOrData[0];
            BPlusTreeNode temp = curr;
            while (temp != null)
            {
                total += temp.NumKeys;
                temp = temp.Next;
            }

            if (total == 0)
            {
                Console.WriteLine("El catálogo está vacío.");
                return;
            }

            Libro[] arreglo = new Libro[total];
            int idx = 0;
            while (curr != null)
            {
                for (int i = 0; i < curr.NumKeys; i++)
                {
                    arreglo[idx++] = (Libro)curr.ChildrenOrData[i];
                }
                curr = curr.Next;
            }

            QuickSortPorTitulo(arreglo, 0, total - 1);

            Console.WriteLine("\n--- CATÁLOGO COMPLETO (ORDENADO POR TÍTULO) ---");
            for (int i = 0; i < total; i++)
            {
                Console.WriteLine(arreglo[i]);
            }
        }

        private void QuickSortPorTitulo(Libro[] arr, int low, int high)
        {
            if (low < high)
            {
                int pi = PartitionPorTitulo(arr, low, high);
                QuickSortPorTitulo(arr, low, pi - 1);
                QuickSortPorTitulo(arr, pi + 1, high);
            }
        }

        private int PartitionPorTitulo(Libro[] arr, int low, int high)
        {
            string pivot = arr[high].Titulo;
            int i = (low - 1);

            for (int j = low; j <= high - 1; j++)
            {
                if (string.Compare(arr[j].Titulo, pivot, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    i++;
                    Libro temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }
            Libro temp2 = arr[i + 1];
            arr[i + 1] = arr[high];
            arr[high] = temp2;
            return (i + 1);
        }
    }
}