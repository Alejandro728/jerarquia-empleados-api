namespace JerarquiaEmpleadosAPI.Models
{
    public class Empleado
    {
        public int Codigo { get; set; }
        public string Puesto { get; set; }
        public string Nombre { get; set; }
        public int? CodigoJefe { get; set; }
        public int Nivel { get; set; } 
        public List<Empleado> Subordinados { get; set; } = new();

    }

}
