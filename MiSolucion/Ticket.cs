namespace MiSolucion
{
    public class Ticket
    {        
       public int Identificador { get;internal set; }
       public string Titulo
    {
            get => titulo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El título no puede ser nulo o vacío");
                }

                if (value.Length > 100)
                {
                    throw new ArgumentException("El título no puede tener más de 100 caracteres");
                }
                titulo = value;
            }
        }

       public string Descripcion
        {
            get => descripcion;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("La descripción no puede ser nula o vacía");
                }
                descripcion = value;
            }
        }
       public Prioridad Prioridad { get; set; }
       public EstadoTicket Estado { get; private set; }
       public DateTime FechaCreacion { get;  private set; }
    

    public Ticket(int identificador, string titulo, string descripcion, Prioridad prioridad, EstadoTicket estado)
    {
        Identificador = identificador;
        Titulo = titulo;
        Descripcion = descripcion;
        Prioridad = prioridad;
        Estado = EstadoTicket.Abierto;
        FechaCreacion = DateTime.Now;
    }
}
}