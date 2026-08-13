using System;

namespace MiSolucion
{
    //excepcion personalizada para validaciones del ticket
    public class TicketValidationException : Exception
    {
        public TicketValidationException(string mensaje) : base(mensaje) { }
    }

    //excepcion personalizada para cuando no se encuentra un ticket
    public class TicketNotFoundException : Exception
    {
        public TicketNotFoundException(int id) 
            : base($"El ticket con ID {id} no fue encontrado.")
        {
        }
    }

    //excepcion personalizada para transiciones de estado invalidas
    public class InvalidTicketStateTransitionException : Exception
    {
        public InvalidTicketStateTransitionException(EstadoTicket estadoActual, EstadoTicket estadoDestino)
            : base($"No se puede cambiar de {estadoActual} a {estadoDestino}.")
        {
        }
    }
}
