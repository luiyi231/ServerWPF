using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerWPF
{
    public class ClientHandler
    {
        private TcpClient client;
        private NetworkStream stream;
        private MainWindow server;
        private DatabaseService dbService;

        public ClientHandler(TcpClient client, MainWindow server)
        {
            this.client = client;
            this.server = server;
            this.stream = client.GetStream();
            this.dbService = new DatabaseService();
        }

        public void HandleClient()
        {
            try
            {
                byte[] buffer = new byte[4096];

                while (client.Connected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    server.LogMessage($"Solicitud recibida: {request.Substring(0, Math.Min(50, request.Length))}...");

                    string response = ProcessRequest(request);
                    SendResponse(response);
                }
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error manejando cliente: {ex.Message}");
            }
            finally
            {
                Close();
                server.RemoveClient(this);
            }
        }

        private string ProcessRequest(string request)
        {
            try
            {
                var requestObj = JsonConvert.DeserializeObject<DatabaseRequest>(request);

                switch (requestObj.Operation)
                {
                    case "SEARCH":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.Search(requestObj.Table, requestObj.SearchCriteria)
                        });

                    case "GET_STUDENT_NAME":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GetStudentName(requestObj.StudentId)
                        });

                    case "GET_GESTIONES":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GetGestiones()
                        });

                    case "INSCRIBIR":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = dbService.Inscribir(requestObj.StudentId, requestObj.ListaCodEd)
                        });

                    case "ELIMINAR_INSCRIPCION":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = dbService.EliminarInscripcion(requestObj.StudentId, requestObj.CodEd)
                        });

                    case "ACTUALIZAR_EDICION":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = dbService.ActualizarEdicion(requestObj.StudentId, requestObj.CodEdActual, requestObj.CodEdNueva)
                        });

                    case "GET_MATERIAS_INSCRITAS":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GetMateriasInscritas(requestObj.StudentId, requestObj.CodGestion)
                        });

                    case "GET_MATERIAS_OFERTADAS":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GetMateriasOfertadas(requestObj.StudentId, requestObj.CodGestion)
                        });

                    case "GET_CARRERAS":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GetCarreras()
                        });

                    case "GET_MATERIAS_POR_CARRERA":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GetMateriasPorCarrera(requestObj.IdCarrera)
                        });

                    case "GET_FECHAS_ASISTENCIA":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GetFechasAsistencia(requestObj.IdMateria, requestObj.IdGestion)
                        });

                    case "GENERAR_REPORTE_ASISTENCIA":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GenerarReporteAsistencia(requestObj.IdCarrera, requestObj.IdMateria, requestObj.IdGestion, requestObj.Fecha)
                        });

                    case "GET_PLANES_POR_CARRERA":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GetPlanesPorCarrera(requestObj.IdCarrera)
                        });

                    case "GENERAR_REPORTE_MATERIAS":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GenerarReporteMaterias(requestObj.IdCarrera, requestObj.IdPlanEstudio, requestObj.IdGestion)
                        });

                    case "BUSCAR_ESTUDIANTE_POR_RU":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.BuscarEstudiantePorRU(requestObj.RU)
                        });

                    case "GENERAR_REPORTE_NOTAS":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GenerarReporteNotas(requestObj.StudentId, requestObj.IdGestion)
                        });

                    case "OBTENER_ESTADISTICAS_ESTUDIANTE":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.ObtenerEstadisticasEstudiante(requestObj.StudentId, requestObj.IdGestion)
                        });

                    case "EXISTE_ESTUDIANTE":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.ExisteEstudiante(requestObj.RU)
                        });

                    case "OBTENER_TODAS_NOTAS_ESTUDIANTE":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.ObtenerTodasNotasEstudiante(requestObj.StudentId)
                        });

                    case "GENERAR_REPORTE_MATERIAS_ESTUDIANTE":
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = true,
                            Data = dbService.GenerarReporteMateriasEstudiante(requestObj.StudentId)
                        });

                    default:
                        return JsonConvert.SerializeObject(new DatabaseResponse
                        {
                            Success = false,
                            ErrorMessage = "Operación no reconocida"
                        });
                }
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error procesando solicitud: {ex.Message}");
                return JsonConvert.SerializeObject(new DatabaseResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        private void SendResponse(string response)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(response);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error enviando respuesta: {ex.Message}");
            }
        }

        public void Close()
        {
            try
            {
                stream?.Close();
                client?.Close();
            }
            catch (Exception ex)
            {
                server.LogMessage($"Error cerrando conexión: {ex.Message}");
            }
        }
    }
}
