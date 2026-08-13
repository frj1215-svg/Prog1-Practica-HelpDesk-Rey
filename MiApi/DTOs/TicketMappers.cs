using MiSolucion;

namespace MiApi.DTOs
{
    //extensión para convertir un Ticket a TicketResponse
    public static class TicketMapper
    {
        //este metodo mapea un ticket a su DTO de respuesta
        public static TicketResponse ATicketResponse(this Ticket ticket)
        {
            return new TicketResponse
            {
                Id = ticket.Id,
                Titulo = ticket.Titulo,
                Descripcion = ticket.Descripcion,
                Prioridad = ticket.Prioridad,
                Estado = ticket.Estado,
                FechaCreacion = ticket.FechaCreacion
            };
        }
    }
}