using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;

namespace BELibrary.Persistence.Repositories
{
    public class DKTiemChungRepository : Repository<DKTiemChung>, IDKTiemChungRepository
    {
        public DKTiemChungRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}