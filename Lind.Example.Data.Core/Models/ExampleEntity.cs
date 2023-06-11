using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.Example.Data.Models
{
    public class ExampleEntity
    {
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsArchived { get; set; }
    }
}
