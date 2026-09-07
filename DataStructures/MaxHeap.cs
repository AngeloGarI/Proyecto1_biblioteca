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

        private void Resize()
        {
            capacity *= 2;
            Libro[] newHeap = new Libro[capacity];
            Array.Copy(heap, newHeap, Size);
            heap = newHeap;
        }
    }
}