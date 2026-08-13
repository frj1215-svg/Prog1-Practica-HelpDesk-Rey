using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MiSolucion
{
    //en esta clase se implementa la logica de persistencia de datos
    //es decir, leer y guardar los tickets en un archivo JSON
    //esto sirve para que los tickets se mantengan entre ejecuciones de la aplicacion
    public class TicketRepository
    {
        private readonly string _rutaArchivo;

        public TicketRepository(string rutaArchivo)
        {
            _rutaArchivo = rutaArchivo;
        }

        public List<Ticket> Leer()
        {
            if (!File.Exists(_rutaArchivo))
                return new List<Ticket>();

            //esto es para leer el contenido del archivo JSON y convertirlo en una lista de tickets
            string json = File.ReadAllText(_rutaArchivo);
            
            if (string.IsNullOrWhiteSpace(json))
                return new List<Ticket>();

            return JsonConvert.DeserializeObject<List<Ticket>>(json) ?? new List<Ticket>();
        }

        public void Guardar(List<Ticket> lista)
        {
            //aca se guarda la lista de tickets en el archivo JSON
            string json = JsonConvert.SerializeObject(lista, Formatting.Indented);
            File.WriteAllText(_rutaArchivo, json);
        }
    }
}