using System;
using System.Collections.Generic;
using System.Text;
using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Processor
{
    public interface IDeskBookingRequestProcessor
    {
        DeskBookingResult BookDesk(DeskBookingRequest request);
    }
}
