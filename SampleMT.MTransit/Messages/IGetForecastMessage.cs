﻿using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Messages
{
    public interface IGetForecastMessage : CorrelatedBy<Guid>
    {
        DateOnly Date { get; set; }
    }
}
