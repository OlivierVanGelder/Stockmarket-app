namespace Logic.Models;

public class UserModel
{
    public string id { get; set; }
    public string userName { get; set; }
    public int balance { get; set; }

    public UserModel(string userid, string username, int userbalance)
    {
        id = userid;
        userName = username;
        balance = userbalance;
    }
}
