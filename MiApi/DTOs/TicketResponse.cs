using MiSolucion;

namespace MiApi.DTOs
{
    //DTO para enviar los datos del ticket en las respuestas
    public class TicketResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public Prioridad Prioridad { get; set; }
        public EstadoTicket Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}