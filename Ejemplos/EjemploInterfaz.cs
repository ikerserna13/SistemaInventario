namespace SistemaInventario.Utilidades.Ejemplos
{

    public class Program
    {
        public Program(ILibro libro)
        {
            libro.GetName();
        }
    }
    public interface ILibro
    {
        string GetName();

    }

    public class LibroSimple : ILibro
    {
        public string GetName()
        {
            return "LibroSimple";
        }
    }

    public class LibroComplejo : ILibro
    {
        public string GetName()
        {
            return "LibroComplejo";
        }
    }
}
