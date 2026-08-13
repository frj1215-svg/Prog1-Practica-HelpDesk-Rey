namespace MiSolucion
{
    public class TicketService
    {
        private readonly TicketRepository _repositorio;

        // Constructor que recibe un repositorio de tickets    
        public TicketService(TicketRepository repositorio)
        {
            _repositorio = repositorio;
        }

        //metodo para Obtener todos los tickets
        public List<Ticket> ObtenerTodosLosTickets()
        {
            return _repositorio.ObtenerTodosLosTickets();
        }

        //metodo para Obtener un ticket por su id
        public Ticket ObtenerTicketPorId(int id)
        {
            var ticket = _repositorio.ObtenerTicketPorId(id);
            if (ticket == null)
            {
                throw new TicketNotFoundException(id);
            }

            return ticket;
        }

        
        public Ticket Crear(string titulo, string descripcion, Prioridad prioridad)
        {
            if (string.IsNullOrWhiteSpace(titulo) || titulo.Length > 100)
                throw new ArgumentException("El título es obligatorio y no puede superar los 100 caracteres.");
            
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción es obligatoria.");

            var tickets = _repositorio.LeerTodos();
            
            int nuevoId = 1;
            if (tickets.Any())
            {
                nuevoId = tickets.Max(t => t.Id) + 1;
            }

            var nuevoTicket = new Ticket
            {
                Id = nuevoId,
                Titulo = titulo,
                Descripcion = descripcion,
                Prioridad = prioridad,
                Estado = EstadoTicket.Abierto, // Todo ticket nace Abierto[cite: 2]
                FechaCreacion = DateTime.Now   // Se asigna automáticamente[cite: 2]
            };

            tickets.Add(nuevoTicket);
            _repositorio.GuardarTodos(tickets); // Guardamos la lista completa[cite: 2]

            return nuevoTicket;
        }      
    }
}