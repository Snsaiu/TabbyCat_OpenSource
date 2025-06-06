using TabbyCat.Models;

namespace TabbyCat.IServices;

public interface IOccupationService
{
    Task<IEnumerable<OccupationType>> GetAllOccupationsAsync();
}