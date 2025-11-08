using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNUStudentPortal.Application.DTOs.Roles
{
    public record CreateRoleRequest(string Name, string Key, string Description);
    public record UpdateRoleRequest(string Name, string Description);
    public record RoleResponse(Guid Id, string Name, string Key, string Description);
    public record RoleDetailsResponse(Guid Id, string Name, string Key, string? Description);

}
