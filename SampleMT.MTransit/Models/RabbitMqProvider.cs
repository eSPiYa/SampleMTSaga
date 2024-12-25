using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Models
{
    internal record RabbitMqProvider
    {
        public required string Host { get; set; }
        public required string VirtualHost { get; set; }
        public required string User { get; set; }
        public required string Password { get; set; }
    }
}
