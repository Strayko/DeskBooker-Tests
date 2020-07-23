using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;

namespace DeskBooker.DataAccess.Repositories
{
    public class DeskRepository : IDeskRepository
    {
        private readonly DeskBookerContext _context;

        public DeskRepository(DeskBookerContext context)
        {
            _context = context;
        }

        public IEnumerable<Desk> GetAll()
        {
            return _context.Desk.ToList();
        }

        public IEnumerable<Desk> GetAvailableDesks(DateTime date)
        {
            var bookerDeskIds = _context.DeskBooking.Where(x => x.Date == date)
                .Select(b => b.DeskId)
                .ToList();

            return _context.Desk
                .Where(x => !bookerDeskIds.Contains(x.Id))
                .ToList();
        }
    }
}
