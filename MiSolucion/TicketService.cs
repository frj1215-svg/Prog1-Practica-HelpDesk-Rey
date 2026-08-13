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
            return _repositorio.Leer();
        }

        //metodo para Obtener un ticket por su id
        public Ticket ObtenerTicketPorId(int id)
        {
            var tickets = _repositorio.Leer();
            var ticket = tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                throw new TicketNotFoundException(id);
            }

            return ticket;
        }

        //metodo para crear un nuevo ticket
        public Ticket Crear(string titulo, string descripcion, Prioridad prioridad)
        {
            if (string.IsNullOrWhiteSpace(titulo) || titulo.Length > 100)
                throw new ArgumentException("El título es obligatorio y no puede superar los 100 caracteres.");
            
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción es obligatoria.");

            var tickets = _repositorio.Leer();
            
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
                Estado = EstadoTicket.Abierto, // Todo ticket nace Abierto
                FechaCreacion = DateTime.Now   // Se asigna automáticamente
            };

            tickets.Add(nuevoTicket);
            _repositorio.Guardar(tickets); // Guardamos la lista completa

            return nuevoTicket;
        }

        // Procedimiento para tomar un ticket (cambiar de Abierto a EnProceso)
        public Ticket TomarTicket(int id)
        {
            var tickets = _repositorio.Leer();
            var ticket = tickets.FirstOrDefault(t => t.Id == id);
            
            if (ticket == null)
            {
                throw new TicketNotFoundException(id);
            }

            ticket.CambiarEstado(EstadoTicket.EnProceso);
            _repositorio.Guardar(tickets);
            
            return ticket;
        }

        // Procedimiento para resolver un ticket (cambiar de EnProceso a Resuelto)
        public Ticket ResolverTicket(int id)
        {
            var tickets = _repositorio.Leer();
            var ticket = tickets.FirstOrDefault(t => t.Id == id);
            
            if (ticket == null)
            {
                throw new TicketNotFoundException(id);
            }

            ticket.CambiarEstado(EstadoTicket.Resuelto);
            _repositorio.Guardar(tickets);
            
            return ticket;
        }

        // Procedimiento para cerrar un ticket (cambiar a Cerrado)
        public Ticket CerrarTicket(int id)
        {
            var tickets = _repositorio.Leer();
            var ticket = tickets.FirstOrDefault(t => t.Id == id);
            
            if (ticket == null)
            {
                throw new TicketNotFoundException(id);
            }

            ticket.CambiarEstado(EstadoTicket.Cerrado);
            _repositorio.Guardar(tickets);
            
            return ticket;
        }

        // Procedimiento para obtener tickets por estado
        public List<Ticket> ObtenerPorEstado(EstadoTicket estado)
        {
            var tickets = _repositorio.Leer();
            return tickets.Where(t => t.Estado == estado).ToList();
        }

        // Procedimiento para buscar tickets por título (búsqueda parcial)
        public List<Ticket> BuscarPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                throw new ArgumentException("El título de búsqueda no puede estar vacío.");
            }

            var tickets = _repositorio.Leer();
            return tickets.Where(t => t.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase)).ToList();
        }

    }
}