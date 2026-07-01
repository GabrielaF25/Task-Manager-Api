using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Infrastructure.Services.Authentication;

public class JwtSettings
{

    public const string SectionName = "Jwt";
    public string Key { get; init; } = string.Empty;
    public string Issuer {  get; init; } = string.Empty;
    public string Audience {  get; init; } = string.Empty;
    public int ExpirationInMinute {  get; init; }
    public int RefreshTokenExpiryMinutes { get; init; }
}
