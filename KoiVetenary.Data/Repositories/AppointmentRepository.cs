using KoiVetenary.Common;
using KoiVetenary.Data.Base;
using KoiVetenary.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiVetenary.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>
    {
        public AppointmentRepository() { }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _context.Appointments.Include(a => a.Owner).ToListAsync();
        }

        public async Task<List<Appointment>> GetAllPendingAsync()
        {
            return await _context.Appointments.Include(a => a.Owner).Where(a => a.Status.Equals(AppointmentStatus.Pending)).ToListAsync();
        }

        public async Task<Appointment> GetByIdAsync(int id)
        {
            var entity = await _context.Appointments.Include(a => a.Owner).FirstOrDefaultAsync(a => a.AppointmentId == id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }
    }
}
