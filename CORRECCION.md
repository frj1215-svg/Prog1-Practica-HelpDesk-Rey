# Corrección — Práctica HelpDesk (Unidad 6)

## Build y tests
`dotnet build MiSolucion.sln` compila sin errores ni warnings.

No hay proyecto de tests en el repositorio (no se encontró ningún `.csproj` de tipo test project, ni carpeta `Tests`/`*.Tests`). Se deja constancia como "no implementado" — no hay tests para correr.

## Arquitectura por capas
Buena separación: `Ticket.cs` (entidad), `TicketService.cs` (lógica de negocio), `TicketRepository.cs` (persistencia), `Enum.cs` y `Exceptions.cs` aparte. Es una arquitectura limpia y fácil de seguir.

## Entidad Ticket y reglas de negocio
- Validaciones de título (vacío, >100 caracteres) y descripción (vacía) implementadas como parte de los *setters* de las propiedades (`Ticket.cs:12-43`), con una excepción propia `TicketValidationException`. Es un enfoque válido (auto-validación en la propiedad) aunque no es exactamente lo que pide el enunciado en cuanto al tipo de excepción (ver sección de las 5 reglas más abajo).
- El enum `Prioridad` (`Enum.cs`) define `Baja, Media, Alta` — **falta el valor `Critica`** que pide el enunciado ("baja, media, alta o crítica").
- `FechaCreacion` se asigna automáticamente y el ticket nace en `Abierto`, correcto.
- Constructor con validación (`Ticket(int, string, string, Prioridad, EstadoTicket)`) más un constructor vacío `Ticket()` pensado explícitamente para deserialización JSON (comentario en línea 49) — buena práctica, evita el problema de deserialización que sí aparece en otras entregas que solo tienen constructor con parámetros.
- Máquina de estados en `Ticket.CambiarEstado` (línea 73) usa un `switch` de tuplas para validar transiciones. **Ojo con esta línea:**
  ```csharp
  (EstadoTicket.EnProceso, EstadoTicket.Cerrado) => true,
  ```
  (`Ticket.cs:95`) — esto permite pasar de `EnProceso` directo a `Cerrado`, **saltéandose el paso `Resuelto`**, lo cual contradice el enunciado ("no se puede saltear pasos... solo puede avanzar en un orden fijo"). Es la única transición inválida que se cuela; el resto de las reglas (no retroceder, no modificar un `Cerrado`, no cerrar un ticket que nunca fue tomado) están bien resueltas.

## Persistencia
Correctamente resuelta. `TicketRepository.Leer()` (`TicketRepository.cs:19`) devuelve lista vacía si el archivo no existe o está vacío, y usa Newtonsoft.Json (`JsonConvert`) tal como sugiere la consigna. `Guardar()` escribe la lista completa cada vez, siguiendo el patrón "leer todo → modificar en memoria → guardar todo" — se ve bien aplicado en `TicketService` (cada método lee la lista completa, modifica el objeto encontrado, y llama `_repositorio.Guardar(tickets)`).

La ruta del archivo la decide quien construye `TicketService`/`TicketRepository` (el `Program.cs` de `MiApi`, fuera del alcance de esta entrega), no el repositorio — correcto según el diseño pedido.

## Tests unitarios
No hay tests entregados (no implementado).

## Preparación para la Web API (5 reglas de diseño)
1. **Service sin Console/ReadLine**: cumplido, `TicketService` no tiene I/O de consola.
2. **`ObtenerPorId` devuelve null si no existe**: **no se cumple**. `TicketService.ObtenerTicketPorId` (línea 20) lanza `TicketNotFoundException` en vez de devolver `null` cuando no encuentra el ticket. El enunciado pide explícitamente lo contrario ("Buscar por id que no existe devuelve null, no lanza excepción").
3. **Excepciones tipadas (`ArgumentException`/`InvalidOperationException`)**: parcialmente cumplido. `TicketService.Crear` sí usa `ArgumentException` para título/descripción inválidos (líneas 35-39). Pero las validaciones dentro de `Ticket` (setters de `Titulo`/`Descripcion`) y las transiciones de estado inválidas usan excepciones propias (`TicketValidationException`, `InvalidTicketStateTransitionException`) que heredan directamente de `Exception`, no de `ArgumentException`/`InvalidOperationException` como pide la consigna. Es un enfoque prolijo (excepciones de dominio con nombre propio y mensaje descriptivo) pero no sigue la tipificación pedida.
4. **Nombres de métodos alineados a verbos HTTP**: mayormente cumplido, aunque con nombres un poco más largos de lo sugerido: `ObtenerTodosLosTickets` (vs. `ObtenerTodos`), `ObtenerTicketPorId` (vs. `ObtenerPorId`). `Crear`, `TomarTicket`, `ResolverTicket`, `CerrarTicket`, `ObtenerPorEstado`, `BuscarPorTitulo` coinciden con lo sugerido.
5. **Ruta no hardcodeada como absoluta**: en `MiApi/Program.cs` se arma la ruta con `Path.Combine(AppContext.BaseDirectory, "tickets.json")` — no es una ruta absoluta *hardcodeada* en el código fuente, sino calculada en base al directorio de ejecución, que es una práctica robusta y razonable (aunque técnicamente da una ruta absoluta en tiempo de ejecución, en vez de simplemente pasar `"tickets.json"` como sugiere la consigna). Como esto vive en `MiApi`, que no es parte de esta entrega, se menciona solo a título informativo.

## Observaciones generales
El código está prolijo y demuestra buen entendimiento de las capas y de la validación por excepciones propias — el uso de excepciones de dominio (`TicketNotFoundException`, `TicketValidationException`, `InvalidTicketStateTransitionException`) con mensajes descriptivos es una práctica que se valora, aunque en este caso se aparta de lo que pide puntualmente el enunciado (tipos `ArgumentException`/`InvalidOperationException`, y `ObtenerPorId` debiendo devolver `null`). El constructor vacío de `Ticket` pensado explícitamente para la deserialización JSON es un buen detalle que evita el problema de deserialización que aparece en otras entregas. Los dos puntos a mejorar de cara a la próxima etapa: permitir el salto `EnProceso → Cerrado` (revisar la tabla de transiciones válidas) y agregar el proyecto de tests, que no está presente en esta entrega.

## Web API

El proyecto `MiApi/MiApi.csproj` (`Microsoft.NET.Sdk.Web`) es, junto con el de Schneider, de los más completos del grupo: **compila sin errores** (`dotnet build MiApi/MiApi.csproj`, solo 4 warnings de nullabilidad en los DTOs) y cubre bien la mayoría de la rúbrica:

- **Arquitectura de 3 proyectos**: `MiApi.csproj` referencia a `..\MiSolucion\MiSolucion.csproj` (única `ProjectReference`), sin referencia circular. Nota: el `.sln` (`MiSolucion.sln`) solo incluye el proyecto `MiSolucion`, no `MiApi` — igual que en el resto del grupo, así que si se corre `dotnet build MiSolucion.sln` a secas, la Web API queda fuera del build.
- **Controller flaco**: `TicketsController` (`MiApi/Controllers/TicketControllers.cs`) usa `[ApiController]` + `[Route("api/[controller]")]`, recibe `TicketService` por constructor, y no contiene lógica de negocio propia — solo llama al service y traduce el resultado/excepción a una respuesta HTTP.
- **Endpoints**: cubre las operaciones esperadas — `GET /api/tickets` (todos), `GET /api/tickets/{id}`, `POST /api/tickets` (crear), `PATCH /api/tickets/{id}/tomar|resolver|cerrar` (transiciones — usa `[HttpPatch]`, una elección más ajustada semánticamente que el `POST`/`PUT` que usan otras entregas del grupo para lo mismo), `GET /api/tickets/por-estado` (filtro) y `GET /api/tickets/buscar` (texto).
- **DTOs**: `TicketRequest` y `TicketResponse` (`MiApi/DTOs/`) están bien ubicados en el proyecto Web API y no en el dominio. El `TicketService` (`MiSolucion/TicketService.cs`) no importa ni conoce los DTOs en ningún momento, solo trabaja con `Ticket` — cumple el punto 4 de la rúbrica.
- **Mapeo**: `TicketMapper.ATicketResponse()` (`MiApi/DTOs/TicketMappers.cs`) es el extension method usado en el controller para la dirección entidad→DTO, siguiendo el patrón de la unidad. No hay mapeo inverso explícito (`TicketRequest`→`Ticket`); en su lugar el controller desarma el DTO y pasa los campos sueltos a `_ticketService.Crear(dto.Titulo, dto.Descripcion, dto.Prioridad)`, que sí coincide exactamente con la sobrecarga real de `TicketService.Crear(string, string, Prioridad)` (a diferencia de Martinez/Pettinati, acá el mapeo manual funciona porque las firmas están alineadas).
- **Validación**: acá está el punto más flojo — `TicketRequest` (`MiApi/DTOs/TickerRequest.cs`, nombre de archivo con errata: "Ticker" en vez de "Ticket") **no tiene ningún Data Annotation** (`[Required]`, `[StringLength]`, etc.). La validación de título/descripción vacíos o largos sigue ocurriendo únicamente dentro de `TicketService.Crear` (que lanza `ArgumentException`, capturada por el controller y traducida a 400). Funcionalmente el resultado es el mismo (un título vacío sigue dando 400), pero no se aprovecha el mecanismo de validación automática de `[ApiController]` que pide la consigna.
- **Códigos de estado HTTP**: bien resueltos y explícitos — cada acción tiene su propio `try/catch` que traduce `TicketNotFoundException`→404, `InvalidTicketStateTransitionException`/`ArgumentException`→400, y cualquier otra excepción→500 con un objeto `{mensaje, detalle}`. Es más verboso que un manejo centralizado, pero cubre bien los casos pedidos por la rúbrica, incluyendo el 404 real para "ticket inexistente" en las transiciones de estado (algo que, por ejemplo, Martinez/Pettinati no logran porque su excepción de "no encontrado" no está tipada).
- **Inyección de dependencias**: `Program.cs` registra `TicketRepository` y `TicketService` como `Scoped` (`builder.Services.AddScoped(sp => new TicketRepository(rutaArchivo)); builder.Services.AddScoped<TicketService>();`) e inyecta `TicketService` por constructor en el controller — patrón correcto y funciona porque `TicketRepository`/`TicketService` calzan con las firmas reales de sus constructores.
- **Extras**: Swagger configurado (`AddSwaggerGen`) además de `AddOpenApi`/`MapOpenApi` (el generador nativo de .NET 10) — un poco redundante tener ambos, pero no incorrecto. No hay CORS configurado. No se usa `ILogger`.

Vale la pena conectar esto con la sección "Preparación para la Web API" de más arriba, donde se señaló que `TicketService.ObtenerTicketPorId` lanza `TicketNotFoundException` en vez de devolver `null` — una desviación de las "5 reglas" pensadas para que el Service sea agnóstico de HTTP. Visto ahora desde el lado de la Web API, esa decisión en realidad **funciona bien**: el controller atrapa `TicketNotFoundException` explícitamente y la traduce a 404, así que el resultado HTTP es correcto igual, solo que por un camino distinto al sugerido en la consigna (excepción de dominio tipada en vez de `null` + chequeo en el controller).

Un detalle de dominio que sí impacta directamente a la Web API: la transición inválida `EnProceso → Cerrado` señalada en la sección "Entidad Ticket y reglas de negocio" (`Ticket.cs:95`) es alcanzable a través de `PATCH /api/tickets/{id}/cerrar`, que llama a `ticket.CambiarEstado(EstadoTicket.Cerrado)` sin pasar por `ResolverTicket` — es decir, el bug de la máquina de estados queda expuesto tal cual a través del endpoint HTTP.
