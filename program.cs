using System;
using System.IO;

namespace BibliotecaEstructuras
{
    class Program
    {
        static BPlusTree arbolBPlus = new BPlusTree(3);
        static MaxHeap maxHeapPrestados = new MaxHeap();
        static MinHeap minHeapCopias = new MinHeap();

        static void Main(string[] args)
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("   SISTEMA DE GESTIÓN DE BIBLIOTECA - ESTRUCTURAS ");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. Cargar libros desde archivo (.csv)");
                Console.WriteLine("2. Registrar nuevo libro manualmente");
                Console.WriteLine("3. Buscar libro por código (Árbol B+)");
                Console.WriteLine("4. Registrar préstamo de un libro");
                Console.WriteLine("5. Registrar devolución de un libro");
                Console.WriteLine("6. Ver catálogo completo ordenado (Árbol B+)");
                Console.WriteLine("7. Ver libros más prestados (Max Heap)");
                Console.WriteLine("8. Ver libros con menor stock de copias (Min Heap)");
                Console.WriteLine("9. Salir");
                Console.WriteLine("==================================================");
                Console.Write("Seleccione una opción: ");

                if (int.TryParse(Console.ReadLine(), out opcion))
                {
                    switch (opcion)
                    {
                        case 1: CargarDesdeCSV(); break;
                        case 2: RegistrarManual(); break;
                        case 3: BuscarLibro(); break;
                        case 4: RegistrarPrestamo(); break;
                        case 5: RegistrarDevolucion(); break;
                        case 6: arbolBPlus.ImprimirCatalogoOrdenado(); Pausa(); break;
                        case 7: MostrarMasPrestados(); break;
                        case 8: MostrarMenorStock(); break;
                        case 9: Console.WriteLine("Saliendo del programa..."); break;
                        default: Console.WriteLine("Opción no válida."); Pausa(); break;
                    }
                }
                else
                {
                    Console.WriteLine("Entrada inválida.");
                    Pausa();
                }
            } while (opcion != 9);
        }

        static void InsertarEnEstructuras(Libro libro)
        {
            arbolBPlus.Insertar(libro);
            maxHeapPrestados.Insertar(libro);
            minHeapCopias.Insertar(libro);
        }

        static void CargarDesdeCSV()
        {
            Console.Write("\nIngrese la ruta del archivo CSV (ej. libros.csv): ");
            string ruta = Console.ReadLine();

            if (!File.Exists(ruta))
            {
                Console.WriteLine("Error: El archivo no existe.");
                Pausa();
                return;
            }

            try
            {
                string[] lineas = File.ReadAllLines(ruta);
                int cargados = 0;
                for (int i = 0; i < lineas.Length; i++)
                {
                    if (i == 0 && lineas[i].ToLower().Contains("codigo")) continue; // Saltar encabezado
                    string[] datos = lineas[i].Split(',');
                    if (datos.Length >= 6)
                    {
                        int codigo = int.Parse(datos[0].Trim());
                        string titulo = datos[1].Trim();
                        string autor = datos[2].Trim();
                        string cat = datos[3].Trim();
                        int copias = int.Parse(datos[4].Trim());
                        int prestado = int.Parse(datos[5].Trim());

                        Libro l = new Libro(codigo, titulo, autor, cat, copias, prestado);
                        InsertarEnEstructuras(l);
                        cargados++;
                    }
                }
                Console.WriteLine($"\n¡Éxito! Se cargaron {cargados} libros en las estructuras.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar el archivo: {ex.Message}");
            }
            Pausa();
        }

        static void RegistrarManual()
        {
            Console.WriteLine("\n--- REGISTRO MANUAL DE LIBRO ---");
            Console.Write("Código único (entero): ");
            int cod = int.Parse(Console.ReadLine());
            
            if (arbolBPlus.Buscar(cod) != null)
            {
                Console.WriteLine("Error: Ya existe un libro con ese código.");
                Pausa();
                return;
            }

            Console.Write("Título: ");
            string tit = Console.ReadLine();
            Console.Write("Autor: ");
            string aut = Console.ReadLine();
            Console.Write("Categoría: ");
            string cat = Console.ReadLine();
            Console.Write("Cantidad de copias: ");
            int cop = int.Parse(Console.ReadLine());
            Console.Write("Número de veces prestado (inicial): ");
            int pres = int.Parse(Console.ReadLine());

            Libro l = new Libro(cod, tit, aut, cat, cop, pres);
            InsertarEnEstructuras(l);
            Console.WriteLine("\nLibro registrado exitosamente.");
            Pausa();
        }

        static void BuscarLibro()
        {
            Console.Write("\nIngrese el código del libro a buscar: ");
            if (int.TryParse(Console.ReadLine(), out int cod))
            {
                Libro l = arbolBPlus.Buscar(cod);
                if (l != null)
                {
                    Console.WriteLine("\nLibro encontrado:");
                    Console.WriteLine(l);
                }
                else
                {
                    Console.WriteLine("\nLibro no encontrado en el árbol B+.");
                }
            }
            Pausa();
        }

        static void RegistrarPrestamo()
        {
            Console.Write("\nIngrese el código del libro a prestar: ");
            if (int.TryParse(Console.ReadLine(), out int cod))
            {
                Libro l = arbolBPlus.Buscar(cod);
                if (l != null)
                {
                    if (l.CopiasDisponibles > 0)
                    {
                        l.CopiasDisponibles--;
                        l.VecesPrestado++;
                        Console.WriteLine($"\nPréstamo realizado. Copias restantes: {l.CopiasDisponibles}. Total préstamos: {l.VecesPrestado}");
                    }
                    else
                    {
                        Console.WriteLine("\nNo hay copias disponibles para préstamo.");
                    }
                }
                else
                {
                    Console.WriteLine("\nLibro no encontrado.");
                }
            }
            Pausa();
        }

        static void RegistrarDevolucion()
        {
            Console.Write("\nIngrese el código del libro a devolver: ");
            if (int.TryParse(Console.ReadLine(), out int cod))
            {
                Libro l = arbolBPlus.Buscar(cod);
                if (l != null)
                {
                    l.CopiasDisponibles++;
                    Console.WriteLine($"\nDevolución realizada. Copias actuales: {l.CopiasDisponibles}");
                }
                else
                {
                    Console.WriteLine("\nLibro no encontrado.");
                }
            }
            Pausa();
        }

        static void MostrarMasPrestados()
        {
            Console.WriteLine("\n--- TOP LIBROS MÁS PRESTADOS (MAX HEAP) ---");
            MaxHeap temp = ReconstruirMaxHeap();
            int top = 1;
            while (temp.Size > 0 && top <= 5)
            {
                Libro l = temp.ExtraerMax();
                Console.WriteLine($"{top}. {l.Titulo} - {l.VecesPrestado} préstamos");
                top++;
            }
            Pausa();
        }

        static void MostrarMenorStock()
        {
            Console.WriteLine("\n--- LIBROS CON MENOR STOCK (MIN HEAP) ---");
            MinHeap temp = ReconstruirMinHeap();
            int top = 1;
            while (temp.Size > 0 && top <= 5)
            {
                Libro l = temp.ExtraerMin();
                Console.WriteLine($"{top}. {l.Titulo} - {l.CopiasDisponibles} copias disponibles");
                top++;
            }
            Pausa();
        }

        static MaxHeap ReconstruirMaxHeap()
        {
            MaxHeap nuevo = new MaxHeap();
            RecorrerEInsertarMax(arbolBPlus.Root, nuevo);
            return nuevo;
        }

        static MinHeap ReconstruirMinHeap()
        {
            MinHeap nuevo = new MinHeap();
            RecorrerEInsertarMin(arbolBPlus.Root, nuevo);
            return nuevo;
        }

        static void RecorrerEInsertarMax(BPlusTreeNode node, MaxHeap heap)
        {
            BPlusTreeNode curr = node;
            while (curr != null && !curr.IsLeaf) curr = (BPlusTreeNode)curr.ChildrenOrData[0];
            while (curr != null)
            {
                for (int i = 0; i < curr.NumKeys; i++)
                    heap.Insertar((Libro)curr.ChildrenOrData[i]);
                curr = curr.Next;
            }
        }

        static void RecorrerEInsertarMin(BPlusTreeNode node, MinHeap heap)
        {
            BPlusTreeNode curr = node;
            while (curr != null && !curr.IsLeaf) curr = (BPlusTreeNode)curr.ChildrenOrData[0];
            while (curr != null)
            {
                for (int i = 0; i < curr.NumKeys; i++)
                    heap.Insertar((Libro)curr.ChildrenOrData[i]);
                curr = curr.Next;
            }
        }

        static void Pausa()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}