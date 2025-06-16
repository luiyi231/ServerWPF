using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWPF
{
    public class DatabaseRequest
    {
        public string Operation { get; set; }
        public string Table { get; set; }
        public string SearchCriteria { get; set; }
        public int StudentId { get; set; }
        public string ListaCodEd { get; set; }
        public int CodEd { get; set; }
        public int CodEdActual { get; set; }
        public int CodEdNueva { get; set; }
        public int CodGestion { get; set; }
        public int IdCarrera { get; set; }
        public int IdMateria { get; set; }
        public int IdGestion { get; set; }
        public int IdPlanEstudio { get; set; }
        public DateTime Fecha { get; set; }
        public string RU { get; set; }
    }
}
