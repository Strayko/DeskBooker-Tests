using System;
using System.Collections.Generic;
using System.Text;
using DeskBooker.Core.Domain;

namespace DeskBooker.Core.DataInterface
{
    public interface IDeskRepository
    {
        IEnumerable<Desk> GetAvailableDesks(DateTime date);
        IEnumerable<Desk> GetAll();
    }
}
