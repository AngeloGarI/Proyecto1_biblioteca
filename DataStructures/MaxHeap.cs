using System;

namespace BibliotecaEstructuras
{
    public class MaxHeap
    {
        private Libro[] heap;
        private int capacity;
        public int Size { get; private set; }

        public MaxHeap(int capacity = 100)
        {
            this.capacity = capacity;
            this.Size = 0;
            this.heap = new Libro[capacity];
        }

        public void Insertar(Libro libro)
        {
            if (Size == capacity) Resize();
            heap[Size] = libro;
            Size++;
            SiftUp(Size - 1);
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (heap[index].VecesPrestado > heap[parent].VecesPrestado)
                {
                    Libro temp = heap[index];
                    heap[index] = heap[parent];
                    heap[parent] = temp;
                    index = parent;
                }
                else break;
            }
        }

        public Libro ExtraerMax()
        {
            if (Size == 0) return null;
            Libro max = heap[0];
            heap[0] = heap[Size - 1];
            Size--;
            SiftDown(0);
            return max;
        }

        private void SiftDown(int index)
        {
            while (2 * index + 1 < Size)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int largest = index;

                if (left < Size && heap[left].VecesPrestado > heap[largest].VecesPrestado)
                    largest = left;
                if (right < Size && heap[right].VecesPrestado > heap[largest].VecesPrestado)
                    largest = right;

                if (largest != index)
                {
                    Libro temp = heap[index];
                    heap[index] = heap[largest];
                    heap[largest] = temp;
                    index = largest;
                }
                else break;
            }
        }

        public Libro Buscar(int codigo)
        {
            for (int i = 0; i < Size; i++)
            {
                if (heap[i].Codigo == codigo)
                    return heap[i];
            }
            return null;
        }

        public bool Eliminar(int codigo)
        {
            int index = -1;
            for (int i = 0; i < Size; i++)
            {
                if (heap[i].Codigo == codigo)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) return false;

            heap[index] = heap[Size - 1];
            Size--;
            SiftDown(index);
            SiftUp(index);
            return true;
        }

        public void ImprimirHeap()
        {
            if (Size == 0)
            {
                Console.WriteLine("El Heap está vacío.");
                return;
            }

            Console.WriteLine("\n--- ESTRUCTURA INTERNA DEL MAX HEAP (POR NIVELES) ---");
            int nivelActual = 0;
            int nodosEnNivel = 1;
            int contadorNivel = 0;

            for (int i = 0; i < Size; i++)
            {
                if (contadorNivel == 0)
                {
                    Console.Write($"\n[Nivel {nivelActual}]: ");
                }

                Console.Write($"({heap[i].Codigo} - Prestado: {heap[i].VecesPrestado}) ");
                contadorNivel++;

                if (contadorNivel == nodosEnNivel)
                {
                    nivelActual++;
                    nodosEnNivel *= 2;
                    contadorNivel = 0;
                }
            }
            Console.WriteLine();
        }

        private void Resize()
        {
            capacity *= 2;
            Libro[] newHeap = new Libro[capacity];
            Array.Copy(heap, newHeap, Size);
            heap = newHeap;
        }
    }
}