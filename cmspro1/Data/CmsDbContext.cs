using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace cmspro1.Data
{
    public class CmsDbContext:IdentityDbContext
    {
        public CmsDbContext(DbContextOptions<CmsDbContext> Options) :base (Options)
        {

        }
    }
}
