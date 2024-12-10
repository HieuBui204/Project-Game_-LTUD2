using System;
using SQLite4Unity3d;

public class IG_Record
{
    [PrimaryKey]
    public string ID { get; set; }

    public string ID_Profile { get; set; }

    [NotNull]
    public int Time { get; set; }

    [NotNull]
    public int Kill { get; set; }

    [NotNull]
    public int Score { get; set; }
    public DateTime Save_at { get; set; }

    public override string ToString()
    {
        return $"[IG_Record: ID={ID}, ID_Profile={ID_Profile}, TimeSurvive={Time}, Kills={Kill}, Scores={Score}, Save_at={Save_at}]";
    }
}
