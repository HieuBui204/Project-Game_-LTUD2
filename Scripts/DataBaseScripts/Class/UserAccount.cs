using SQLite4Unity3d;

public class UserAccount
{
    [PrimaryKey]
    public string ID { get; set; }

    [Unique]
    public string Username { get; set; }

    [Unique]
    public string Email { get; set; }

    public string Password { get; set; }

    // [Default(value: "CURRENT_TIMESTAMP")]
    public string Create_at { get; set; } // Sử dụng chuỗi vì SQLite lưu trữ thời gian dạng text.

    public override string ToString()
    {
        return $"[UserAccount: ID={ID}, Username={Username}, Email={Email}, Password={Password}, Create_at={Create_at}]";
    }
}
