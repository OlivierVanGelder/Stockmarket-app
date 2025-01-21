namespace Logic.Models;

public class UserModel
{
    public string id { get; set; }
    public string userName { get; set; }
    public int balance { get; set; }
    public bool isFrozen { get; set; }

    public UserModel(string userid, string username, int userbalance, bool isfrozen)
    {
        id = userid;
        userName = username;
        balance = userbalance;
        isFrozen = isfrozen;
    }
}
