using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Models
{
    internal record MartenPersistenceProvider
    {
        public required string ConnectionString { get; set; }
    }
}
