using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRecordsEngine.Entities
{
    public partial class SLUserDashboard
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public string Json { get; set; }
        public virtual SecureUser SecureUser { get; set; }
        public bool? IsFav { get; set; }

    }
}
