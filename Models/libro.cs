namespace BibliotecaEstructuras
{
    public class Libro
    {
        public int Codigo { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Categoria { get; set; }
        public int CopiasDisponibles { get; set; }
        public int VecesPrestado { get; set; }

        public Libro(int codigo, string titulo, string autor, string categoria, int copias, int prestado)
        {
            Codigo = codigo;
            Titulo = titulo;
            Autor = autor;
            Categoria = categoria;
            CopiasDisponibles = copias;
            VecesPrestado = prestado;
        }

        public override string ToString()
        {
            return $"[{Codigo}] {Titulo} - {Autor} | Cat: {Categoria} | Copias: {CopiasDisponibles} | Prestado: {VecesPrestado} veces";
        }
    }
}