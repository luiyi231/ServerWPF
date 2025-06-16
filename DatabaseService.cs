using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWPF
{
    public class DatabaseService
    {
        private string connectionString = @"Data Source=DESKTOP-QU50UN3;Initial Catalog=Proyecto;Integrated Security=True;";

        public DataTable Search(string table, string searchCriteria)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string crit = "%" + searchCriteria + "%";
                string columnaBusqueda = "nombre";

                switch (table)
                {
                    case "Persona":
                    case "Materia":
                    case "Facultad":
                        columnaBusqueda = "nombre";
                        break;
                    case "Estudiante":
                        columnaBusqueda = "numero_registro";
                        break;
                    default:
                        columnaBusqueda = "descripcion";
                        break;
                }

                string sql = $"SELECT * FROM {table} WHERE {columnaBusqueda} LIKE @criteria";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@criteria", crit);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public string GetStudentName(int studentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerNombreEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", studentId);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
        }

        public DataTable GetGestiones()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerGestiones", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public bool Inscribir(int studentId, string listaCodEd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_InscribirMaterias", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", studentId);
                    cmd.Parameters.AddWithValue("@lista_codEd", listaCodEd);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch { return false; }
                }
            }
        }

        public bool EliminarInscripcion(int studentId, int codEd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_EliminarInscripcion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", studentId);
                    cmd.Parameters.AddWithValue("@codEd", codEd);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch { return false; }
                }
            }
        }

        public bool ActualizarEdicion(int studentId, int codEdActual, int codEdNueva)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarInscripcionEdicion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", studentId);
                    cmd.Parameters.AddWithValue("@codEdActual", codEdActual);
                    cmd.Parameters.AddWithValue("@codEdNueva", codEdNueva);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch { return false; }
                }
            }
        }

        public DataTable GetMateriasInscritas(int studentId, int codGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ListarMateriasInscritas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", studentId);
                    cmd.Parameters.AddWithValue("@codGestion", codGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GetMateriasOfertadas(int studentId, int codGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_MateriasOfertadasParaEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudiante", studentId);
                    cmd.Parameters.AddWithValue("@codGestion", codGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GetCarreras()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerCarreras", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GetMateriasPorCarrera(int idCarrera)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerMateriasPorCarrera", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GetFechasAsistencia(int idMateria, int idGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerFechasAsistencia", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_materia", idMateria);
                    cmd.Parameters.AddWithValue("@id_gestion", idGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GenerarReporteAsistencia(int idCarrera, int idMateria, int idGestion, DateTime fecha)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ReporteAsistencia", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                    cmd.Parameters.AddWithValue("@id_materia", idMateria);
                    cmd.Parameters.AddWithValue("@id_gestion", idGestion);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GetPlanesPorCarrera(int idCarrera)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerPlanesPorCarrera", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GenerarReporteMaterias(int idCarrera, int idPlanEstudio, int idGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ReporteMaterias_Ofertadas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                    cmd.Parameters.AddWithValue("@id_plan_estudio", idPlanEstudio);
                    cmd.Parameters.AddWithValue("@id_gestion", idGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable BuscarEstudiantePorRU(string ru)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_BuscarEstudiantePorRU", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RU", ru);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GenerarReporteNotas(int idEstudiante, int idGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GenerarReporteNotasEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@ID_Gestion", idGestion);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable ObtenerEstadisticasEstudiante(int idEstudiante, int idGestion)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerEstadisticasEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Estudiante", idEstudiante);
                    cmd.Parameters.AddWithValue("@ID_Gestion", idGestion);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public bool ExisteEstudiante(string ru)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_VerificarExistenciaEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RU", ru);
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public DataTable ObtenerTodasNotasEstudiante(int idEstudiante)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerTodasNotasEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Estudiante", idEstudiante);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }

        public DataTable GenerarReporteMateriasEstudiante(int idEstudiante)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GenerarReporteMateriasEstudiante", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Estudiante", idEstudiante);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(tabla);
                }
            }
            return tabla;
        }
    }
}
