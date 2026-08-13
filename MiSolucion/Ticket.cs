using System;

namespace MiSolucion
{
    public class Ticket
    {
        private string titulo = string.Empty;
        private string descripcion = string.Empty;

        public int Id { get; set; }

        public string Titulo
        {
            get => titulo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new TicketValidationException("El título no puede ser nulo o vacío.");
                }

                if (value.Length > 100)
                {
                    throw new TicketValidationException("El título no puede tener más de 100 caracteres.");
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
                    throw new TicketValidationException("La descripción no puede ser nula o vacía.");
                }

                descripcion = value;
            }
        }

        public Prioridad Prioridad { get; set; }
        public EstadoTicket Estado { get; set; }
        public DateTime FechaCreacion { get; set; }

        //constructor por defecto para JSON deserialization
        public Ticket() { }

        public Ticket(int identificador, string titulo, string descripcion, Prioridad prioridad, EstadoTicket estado)
        {
            if (identificador < 0)
            {
                throw new TicketValidationException("El identificador no puede ser negativo.");
            }

            Id = identificador;
            Titulo = titulo;
            Descripcion = descripcion;
            Prioridad = prioridad;

            if (estado != EstadoTicket.Abierto)
            {
                throw new TicketValidationException("El estado inicial de un ticket debe ser Abierto.");
            }

            Estado = EstadoTicket.Abierto;
            FechaCreacion = DateTime.Now;   
        }

        public void CambiarEstado(EstadoTicket nuevoEstado)
        {
            if (nuevoEstado == Estado)
            {
                throw new TicketValidationException($"El ticket ya se encuentra {Estado}.");
            }

            if (!EsTransicionValida(Estado, nuevoEstado))
            {
                throw new InvalidTicketStateTransitionException(Estado, nuevoEstado);
            }

            Estado = nuevoEstado;
        }

        //se valida si el estado al que quiero cambiar es valido
        private static bool EsTransicionValida(EstadoTicket estadoActual, EstadoTicket estadoDestino)
        {
            return (estadoActual, estadoDestino) switch
            {
                (EstadoTicket.Abierto, EstadoTicket.EnProceso) => true,
                (EstadoTicket.EnProceso, EstadoTicket.Resuelto) => true,
                (EstadoTicket.EnProceso, EstadoTicket.Cerrado) => true,
                (EstadoTicket.Resuelto, EstadoTicket.Cerrado) => true,
                _ => false,
            };
        }
    }
}
