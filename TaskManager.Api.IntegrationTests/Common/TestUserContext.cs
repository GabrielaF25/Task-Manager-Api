using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.IntegrationTests.Common;

public static class TestUserContext
{
    public static int UserId { get; set; } = 1;
    public static UserRole Role { get; set; } = UserRole.Admin;
}
