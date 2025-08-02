using Microsoft.AspNetCore.Mvc;
using Dapper;
using JerarquiaEmpleadosAPI.Data;
using JerarquiaEmpleadosAPI.Models;
using Microsoft.Data.SqlClient;
using JerarquiaEmpleadosAPI.Data.JerarquiaEmpleados.API.Data;
using System.Data;

namespace JerarquiaEmpleadosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadoController : ControllerBase
    {
        private readonly SqlConnectionFactory _factory;

        public EmpleadoController(SqlConnectionFactory factory)
        {
            _factory = factory;

        }

        [HttpGet("jerarquia")]
        public async Task<IActionResult> ObtenerJerarquia()
        {
            using var connection = _factory.CreateConnection();

            var empleados = await connection.QueryAsync<Empleado>(
                "sp_ObtenerJerarquia",
                commandType: System.Data.CommandType.StoredProcedure);

            return Ok(empleados);
        }



        [HttpPost]
        public async Task<IActionResult> InsertarEmpleado([FromBody] Empleado empleado)
        {
            using var connection = _factory.CreateConnection();

            var parametros = new
            {
                empleado.Puesto,
                empleado.Nombre,
                empleado.CodigoJefe
            };

            var filas = await connection.ExecuteAsync(
                "sp_InsertarEmpleado",
                parametros,
                commandType: CommandType.StoredProcedure);

            return Ok(new { mensaje = "Empleado insertado correctamente", filasAfectadas = filas });
        }



        [HttpPut("{codigo}")]
        public async Task<IActionResult> ActualizarEmpleado(int codigo, [FromBody] Empleado empleado)
        {
            if (codigo != empleado.Codigo)
                return BadRequest("El código no coincide con el empleado enviado.");

            using var connection = _factory.CreateConnection();

            var parametros = new
            {
                empleado.Codigo,
                empleado.Puesto,
                empleado.Nombre,
                empleado.CodigoJefe
            };

            var filas = await connection.ExecuteAsync(
                "sp_ActualizarEmpleado",
                parametros,
                commandType: CommandType.StoredProcedure);

            return Ok(new { mensaje = "Empleado actualizado correctamente", filasAfectadas = filas });
        }



        [HttpDelete("{codigo}")]
        public async Task<IActionResult> EliminarEmpleado(int codigo)
        {
            using var connection = _factory.CreateConnection();

            var filas = await connection.ExecuteAsync(
                "sp_EliminarEmpleado",
                new { Codigo = codigo },
                commandType: CommandType.StoredProcedure);

            return Ok(new { mensaje = "Empleado eliminado correctamente", filasAfectadas = filas });
        }


        private List<Empleado> ConstruirArbol(List<Empleado> empleados)
        {
            var dict = empleados.ToDictionary(e => e.Codigo);
            var raiz = new List<Empleado>();

            foreach (var empleado in empleados)
            {
                if (empleado.CodigoJefe.HasValue && dict.ContainsKey(empleado.CodigoJefe.Value))
                {
                    dict[empleado.CodigoJefe.Value].Subordinados.Add(empleado);
                }
                else
                {
                    raiz.Add(empleado);
                }
            }

            return raiz;
        }


        [HttpGet("jerarquia/arbol")]
        public async Task<IActionResult> ObtenerJerarquiaComoArbol()
        {
            using var connection = _factory.CreateConnection();

            var empleados = (await connection.QueryAsync<Empleado>(
                "sp_ObtenerJerarquia",
                commandType: CommandType.StoredProcedure)).ToList();

            var jerarquia = ConstruirArbol(empleados);

            return Ok(jerarquia);
        }



        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleadosPlano()
        {
            using var connection = _factory.CreateConnection();

            var empleados = await connection.QueryAsync<Empleado>("SELECT * FROM Empleado");

            return Ok(empleados);
        }



    }




}
