using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using APIAgendaNeon.Context;
using APIAgendaNeon.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System.Drawing;


namespace APIAgendaNeon.Funciones
{
    public class PersonaFunction
    {
        private readonly ILogger<PersonaFunction> _logger;
        private readonly AgendaBDContext _context;

        public PersonaFunction(ILogger<PersonaFunction> logger, AgendaBDContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Listar Persona

        [Function("ListarPersona")]
        [OpenApiOperation(operationId: "Run", Summary = "Obtener personas", Description = "Lista a todas las personas de la agenda")]
        [OpenApiSecurity("Authorization", SecuritySchemeType.ApiKey, Name = "Basic", In = OpenApiSecurityLocationType.Header)]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "personas")] HttpRequestData req)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);

            try
            {
                var personas = await _context.Personas.ToListAsync();
                await response.WriteAsJsonAsync(personas);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener personas: {ex.Message}");
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("Error al obtener personas");
            }

            return response;
        }



        // Método para eliminar una persona
        [Function("EliminarPersona")]
        [OpenApiOperation(operationId: "Eliminar", Summary = "Eliminar persona", Description = "Elimina una persona de la agenda por ID")]
        [OpenApiSecurity("Authorization", SecuritySchemeType.ApiKey, Name = "Basic", In = OpenApiSecurityLocationType.Header)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "ID de la persona a eliminar")]
        public async Task<HttpResponseData> EliminarPersona([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "persona/{id:int}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("Intentando eliminar persona con ID: {Id}", id);

            var response = req.CreateResponse();

            try
            {
                var persona = await _context.Personas.FindAsync(id);
                if (persona == null)
                {
                    _logger.LogWarning("Persona con ID: {Id} no encontrada", id);
                    response.StatusCode = HttpStatusCode.NotFound;
                    await response.WriteStringAsync("Persona no encontrada");
                    return response;
                }

                _context.Personas.Remove(persona);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Persona con ID: {Id} eliminada exitosamente", id);
                response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar persona: {ex.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                await response.WriteStringAsync("Error al eliminar persona");
            }

            return response;
        }

        // Insertar persona

        [Function("InsertarPersona")]
        [OpenApiOperation(operationId: "Insertar", Summary = "Insertar persona", Description = "Inserta una nueva persona en la agenda")]
        [OpenApiSecurity("Authorization", SecuritySchemeType.ApiKey, Name = "Basic", In = OpenApiSecurityLocationType.Header)]
        [OpenApiRequestBody("application/json", typeof(Persona), Description = "Detalles de la persona a insertar", Required = true)]
        public async Task<HttpResponseData> InsertarPersona([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "insertarpersona")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para insertar personas.");

            var response = req.CreateResponse();

            try
            {
                var persona = await req.ReadFromJsonAsync<Persona>() ?? throw new Exception("Debe ingresar una persona con todos sus datos");

                _context.Personas.Add(persona);
                await _context.SaveChangesAsync();

                response.StatusCode = HttpStatusCode.Created;
                await response.WriteAsJsonAsync(persona);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al insertar persona: {e.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                await response.WriteStringAsync("Error al insertar persona");
            }

            return response;
        }

        // Editar persona

        /*[Function("EdtarPersona")]
        [OpenApiOperation(operationId: "Editar", Summary = "Editar persona", Description = "Editar una persona en la agenda")]
        [OpenApiSecurity("Authorization", SecuritySchemeType.ApiKey, Name = "Basic", In = OpenApiSecurityLocationType.Header)]
        [OpenApiRequestBody("application/json", typeof(Persona), Description = "Detalles de la persona a Editar", Required = true)]
        public async Task<HttpResponseData> EditarPersona([HttpTrigger(AuthorizationLevel.Function, "post", Route = "editarpersona")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para Editar personas.");

            var response = req.CreateResponse();

            try
            {
                var persona = await req.ReadFromJsonAsync<Persona>() ?? throw new Exception("Debe ingresar una persona con todos sus datos");

                _context.Personas.Update(persona);
                await _context.SaveChangesAsync();

                response.StatusCode = HttpStatusCode.Created;
                await response.WriteAsJsonAsync(persona);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al editar persona: {e.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                await response.WriteStringAsync("Error al editar persona");
            }

            return response;
        }*/

        // Segundo metodo para editar una persona

        [Function("EditarPersona")]
        [OpenApiOperation(operationId: "Editar", Summary = "Editar persona", Description = "Edita los detalles de una persona en la agenda")]
        [OpenApiSecurity("Authorization", SecuritySchemeType.ApiKey, Name = "Basic", In = OpenApiSecurityLocationType.Header)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "ID de la persona a editar")]
        [OpenApiRequestBody("application/json", typeof(Persona), Description = "Detalles de la persona a editar", Required = true)]
        public async Task<HttpResponseData> EditarPersona([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "persona/{id:int}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("Intentando editar persona con ID: {Id}", id);

            var response = req.CreateResponse();

            try
            {
                var persona = await _context.Personas.FindAsync(id);
                if (persona == null)
                {
                    _logger.LogWarning("Persona con ID: {Id} no encontrada", id);
                    response.StatusCode = HttpStatusCode.NotFound;
                    await response.WriteStringAsync("Persona no encontrada");
                    return response;
                }

                var updatedPersona = await req.ReadFromJsonAsync<Persona>();

                if (updatedPersona == null)
                {
                    _logger.LogWarning("Datos de persona no proporcionados en el cuerpo de la solicitud");
                    response.StatusCode = HttpStatusCode.BadRequest;
                    await response.WriteStringAsync("Datos de persona no proporcionados");
                    return response;
                }
                
                persona.nombre = updatedPersona.nombre;
                persona.apellido = updatedPersona.apellido;
                persona.fechanacimiento = updatedPersona.fechanacimiento;
                persona.ci = updatedPersona.ci;
                persona.telefono = updatedPersona.telefono;

                _context.Personas.Update(persona);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Persona con ID: {Id} editada exitosamente", id);
                response.StatusCode = HttpStatusCode.OK;
                await response.WriteAsJsonAsync(persona);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al editar persona: {ex.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                await response.WriteStringAsync("Error al editar persona");
            }

            return response;
        }

    }
}

