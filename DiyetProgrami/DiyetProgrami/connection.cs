using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DiyetProgrami
{
    internal class connection
    {
        
         
        public static string connectionstring { get; set; } = ConfigurationManager.ConnectionStrings["DiyetProgramiConnectionString"].ConnectionString;
        //Sağda bulunan dbveritabani.mdf dosyasına çif tıklarsan data source görüceksin. onu alıp tırnak içinde yapıştırdığım yere yapıştır.

    }
}
