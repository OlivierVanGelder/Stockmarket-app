using Microsoft.AspNetCore.Identity;

namespace DAL.Tables;
public class User : IdentityUser
{
    public override string UserName { get; set; } = "";
    public override string Email { get; set; } = "";
    public override string NormalizedUserName { get; set; } = "";
    public override string NormalizedEmail { get; set; } = "";
    public override string PasswordHash { get; set; } = "";
    public override string SecurityStamp { get; set; } = "";

    public int BalanceInCents { get; set; }
    public List<User_Stock> UserStocks { get; set; } = [];
}
