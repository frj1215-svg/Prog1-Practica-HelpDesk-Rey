using Microsoft.AspNetCore.Mvc;
using MiSolucion;
using MiApi.DTOs;

namespace MiApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public TicketsController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        //metodo para obtener todos los tickets
        [HttpGet]
        public ActionResult<List<TicketResponse>> ObtenerTodos()
        {
            try
            {
                var tickets = _ticketService.ObtenerTodosLosTickets();
                //mapeamos cada ticket a su DTO de respuesta
                return Ok(tickets.Select(t => t.ATicketResponse()).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener tickets", detalle = ex.Message });
            }
        }

        //metodo para obtener un ticket por su id
        [HttpGet("{id}")]
        public ActionResult<TicketResponse> ObtenerPorId(int id)
        {
            try
            {
                var ticket = _ticketService.ObtenerTicketPorId(id);
                //mapeamos el ticket a su DTO de respuesta
                return Ok(ticket.ATicketResponse());
            }
            catch (TicketNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener ticket", detalle = ex.Message });
            }
        }

        //metodo para crear un nuevo ticket
        [HttpPost]
        public ActionResult<TicketResponse> Crear([FromBody] TicketRequest dto)
        {
            try
            {
                var ticket = _ticketService.Crear(dto.Titulo, dto.Descripcion, dto.Prioridad);
                //mapeamos el ticket creado a su DTO de respuesta
                return CreatedAtAction(nameof(ObtenerPorId), new { id = ticket.Id }, ticket.ATicketResponse());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear ticket", detalle = ex.Message });
            }
        }

        //metodo para tomar un ticket, cambia el estado a EnProceso
        [HttpPatch("{id}/tomar")]
        public ActionResult<TicketResponse> TomarTicket(int id)
        {
            try
            {
                var ticket = _ticketService.TomarTicket(id);
                //mapeamos el ticket actualizado a su DTO de respuesta
                return Ok(ticket.ATicketResponse());
            }
            catch (TicketNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidTicketStateTransitionException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al tomar ticket", detalle = ex.Message });
            }
        }

        //metodo para resolver un ticket, cambia el estado a Resuelto
        [HttpPatch("{id}/resolver")]
        public ActionResult<TicketResponse> ResolverTicket(int id)
        {
            try
            {
                var ticket = _ticketService.ResolverTicket(id);
                //mapeamos el ticket actualizado a su DTO de respuesta
                return Ok(ticket.ATicketResponse());
            }
            catch (TicketNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidTicketStateTransitionException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al resolver ticket", detalle = ex.Message });
            }
        }

        //metodo para cerrar un ticket, cambia el estado a Cerrado
        [HttpPatch("{id}/cerrar")]
        public ActionResult<TicketResponse> CerrarTicket(int id)
        {
            try
            {
                var ticket = _ticketService.CerrarTicket(id);
                //mapeamos el ticket actualizado a su DTO de respuesta
                return Ok(ticket.ATicketResponse());
            }
            catch (TicketNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidTicketStateTransitionException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cerrar ticket", detalle = ex.Message });
            }
        }

        //metodo para obtener tickets filtrados por su estado
        [HttpGet("por-estado")]
        public ActionResult<List<TicketResponse>> ObtenerPorEstado([FromQuery] EstadoTicket estado)
        {
            try
            {
                var tickets = _ticketService.ObtenerPorEstado(estado);
                //mapeamos cada ticket a su DTO de respuesta
                return Ok(tickets.Select(t => t.ATicketResponse()).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener tickets por estado", detalle = ex.Message });
            }
        }

        //metodo para buscar tickets por titulo
        [HttpGet("buscar")]
        public ActionResult<List<TicketResponse>> BuscarPorTitulo([FromQuery] string titulo)
        {
            try
            {
                //validamos que el titulo no este vacio
                if (string.IsNullOrWhiteSpace(titulo))
                {
                    return BadRequest(new { mensaje = "El título de búsqueda no puede estar vacío" });
                }

                var tickets = _ticketService.BuscarPorTitulo(titulo);
                //mapeamos cada ticket a su DTO de respuesta
                return Ok(tickets.Select(t => t.ATicketResponse()).ToList());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al buscar tickets", detalle = ex.Message });
            }
        }
    }
}
