using System;
using System.IO;

namespace BibliotecaEstructuras
{
    class Program
    {
        static BPlusTree arbolBPlus = new BPlusTree(4); // Actualizado a orden 4 por defecto
        static MaxHeap maxHeapPrestados = new MaxHeap();
        static MinHeap minHeapCopias = new MinHeap();
        static string rutaArchivoActual = Path.Combine("Data", "libros.csv");

        static void Main(string[] args)
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("   SISTEMA DE GESTIÓN DE BIBLIOTECA - ESTRUCTURAS ");
                Console.WriteLine("1. Cargar libros desde archivo (.csv)");
                Console.WriteLine("2. Registrar nuevo libro manualmente");
                Console.WriteLine("3. Buscar libro por código (Árbol B+)");
                Console.WriteLine("4. Eliminar libro (De todas las estructuras)");
                Console.WriteLine("5. Registrar préstamo de un libro");
                Console.WriteLine("6. Registrar devolución de un libro");
                Console.WriteLine("7. Ver catálogo ordenado por CÓDIGO (Árbol B+)");
                Console.WriteLine("8. Ver catálogo ordenado por TÍTULO (QuickSort)");
                Console.WriteLine("9. Ver libros más prestados (Max Heap)");
                Console.WriteLine("10. Ver libros con menor stock de copias (Min Heap)");
                Console.WriteLine("11. Ver estructuras de Heaps por niveles");
                Console.WriteLine("12. Guardar cambios y salir");
                Console.Write("Seleccione una opción: ");

                if (int.TryParse(Console.ReadLine(), out opcion))
                {
                    switch (opcion)
                    {
                        case 1: CargarDesdeCSV(); break;
                        case 2: RegistrarManual(); break;
                        case 3: BuscarLibro(); break;
                        case 4: EliminarLibro(); break;
                        case 5: RegistrarPrestamo(); break;
                        case 6: RegistrarDevolucion(); break;
                        case 7: arbolBPlus.ImprimirCatalogoOrdenado(); Pausa(); break;
                        case 8: arbolBPlus.ImprimirPorTitulo(); Pausa(); break;
                        case 9: MostrarMasPrestados(); break;
                        case 10: MostrarMenorStock(); break;
                        case 11: MostrarEstructuraHeaps(); break;
                        case 12: GuardarEnCSV(); Console.WriteLine("Saliendo del programa..."); break;
                        default: Console.WriteLine("Opción no válida."); Pausa(); break;
                    }
                }
                else
                {
                    Console.WriteLine("Entrada inválida.");
                    Pausa();
                }
            } while (opcion != 12);
        }

        static void InsertarEnEstructuras(Libro libro)
        {
            arbolBPlus.Insertar(libro);
            maxHeapPrestados.Insertar(libro);
            minHeapCopias.Insertar(libro);
        }

        static void CargarDesdeCSV()
        {
            Console.Write("\nIngrese la ruta del archivo CSV (ej. Data/libros.csv): ");
            string ruta = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(ruta)) ruta = Path.Combine("Data", "libros.csv");

            if (!File.Exists(ruta))
            {
                Console.WriteLine("Error: El archivo no existe.");
                Pausa();
                return;
            }

            rutaArchivoActual = ruta;
            arbolBPlus = new BPlusTree(4); // Inicializa orden 4 al recargar
            maxHeapPrestados = new MaxHeap();
            minHeapCopias = new MinHeap();

            try
            {
                string[] lineas = File.ReadAllLines(ruta);
                int cargados = 0;
                for (int i = 0; i < lineas.Length; i++)
                {
                    if (i == 0 && lineas[i].ToLower().Contains("codigo")) continue;
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

        static void GuardarEnCSV()
        {
            try
            {
                string directorio = Path.GetDirectoryName(rutaArchivoActual);
                if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                using (StreamWriter sw = new StreamWriter(rutaArchivoActual, false))
                {
                    sw.WriteLine("Codigo,Titulo,Autor,Categoria,CopiasDisponibles,VecesPrestado");

                    BPlusTreeNode curr = arbolBPlus.Root;
                    while (curr != null && !curr.IsLeaf) curr = (BPlusTreeNode)curr.ChildrenOrData[0];

                    while (curr != null)
                    {
                        for (int i = 0; i < curr.NumKeys; i++)
                        {
                            Libro l = (Libro)curr.ChildrenOrData[i];
                            sw.WriteLine($"{l.Codigo},{l.Titulo},{l.Autor},{l.Categoria},{l.CopiasDisponibles},{l.VecesPrestado}");
                        }
                        curr = curr.Next;
                    }
                }
                Console.WriteLine($"\n[Persistencia] Datos guardados exitosamente en '{rutaArchivoActual}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al guardar en el archivo: {ex.Message}");
            }
        }

        static void RegistrarManual()
        {
            Console.WriteLine("\n--- REGISTRO MANUAL DE LIBRO ---");
            Console.Write("Código único (entero): ");
            if (!int.TryParse(Console.ReadLine(), out int cod))
            {
                Console.WriteLine("Código inválido.");
                Pausa();
                return;
            }

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
            GuardarEnCSV();
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

        static void EliminarLibro()
        {
            Console.Write("\nIngrese el código del libro a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out int cod))
            {
                bool exitoBPlus = arbolBPlus.Eliminar(cod);
                bool exitoMax = maxHeapPrestados.Eliminar(cod);
                bool exitoMin = minHeapCopias.Eliminar(cod);

                if (exitoBPlus)
                {
                    GuardarEnCSV();
                    Console.WriteLine($"\nLibro con código {cod} eliminado de todas las estructuras.");
                }
                else
                {
                    Console.WriteLine("\nEl libro no fue encontrado.");
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
                        GuardarEnCSV();
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
                    GuardarEnCSV();
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

        static void MostrarEstructuraHeaps()
        {
            maxHeapPrestados.ImprimirHeap();
            minHeapCopias.ImprimirHeap();
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