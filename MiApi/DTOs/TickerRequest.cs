using MiSolucion;

namespace MiApi.DTOs
{
    //DTO para recibir los datos al crear un nuevo ticket
    public class TicketRequest
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public Prioridad Prioridad { get; set; }
    }
}