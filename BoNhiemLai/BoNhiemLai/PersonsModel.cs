using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoNhiemLai
{
    public class PersonsModel
    {
        public int Id { get; set; }
        public int? Stt { get; set; }
        public string NamePerson { get; set; }
        public System.DateTime Date { get; set; }
        public string WorkPlace { get; set; }
    }
}
