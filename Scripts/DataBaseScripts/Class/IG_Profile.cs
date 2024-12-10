using SQLite4Unity3d;

public class IG_Profile
{
    [PrimaryKey]
    public string ID { get; set; }

    [NotNull]
    public string ID_User { get; set; }

    [NotNull, Unique]
    public string Ig_Name { get; set; }
    public string Role { get; set; }
    public override string ToString()
    {
        return $"[IG_Profile: ID={ID}, ID_UserAccount={ID_User}, Ig_Name={Ig_Name}, Role={Role}]";
    }
}
