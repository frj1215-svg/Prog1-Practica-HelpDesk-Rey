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

        //metodo para crear un ticket
        public Ticket CrearTicket(string titulo, string descripcion, Prioridad prioridad)
        {
            var nuevoTicket = new Ticket(0, titulo, descripcion, prioridad, EstadoTicket.Abierto);

            _repositorio.AgregarTicket(nuevoTicket);

            return nuevoTicket;
        }       
    }
}