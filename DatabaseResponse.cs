using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWPF
{
    public class DatabaseResponse
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}
