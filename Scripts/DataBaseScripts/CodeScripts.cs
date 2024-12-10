using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CodeScripts
{
    //CRUD
    DB db;

    public CodeScripts()
    {
        db = new DB();
    }

    public UserAccount GetUserByUserName(string username)
    {
        return db.GetConnection().Table<UserAccount>().Where(x => x.Username == username).FirstOrDefault();
    }
    public IG_Profile GetProfileByUserID(string userid)
    {
        return db.GetConnection().Table<IG_Profile>().Where(x => x.ID_User == userid).FirstOrDefault();
    }

    public string GenerateUserId()
    {
        // Lấy ID lớn nhất hiện tại từ bảng UserAccount
        var maxId = db.GetConnection().Table<UserAccount>()
            .OrderByDescending(x => x.ID)
            .FirstOrDefault()?.ID;

        if (maxId == null)
        {
            return "USER000"; // Trường hợp chưa có người dùng nào
        }

        // Chuyển ID hiện tại thành số và cộng thêm 1
        int newIdNumber = int.Parse(maxId.Substring(4)) + 1;  // Lấy phần số sau "USER"
        return $"USER{newIdNumber:D3}";  // Tạo ID mới với 3 chữ số (ví dụ: USER001, USER002,...)
    }

    public string GenerateProfileId()
    {
        // Lấy ID lớn nhất hiện tại từ bảng IG_Profile
        var maxId = db.GetConnection().Table<IG_Profile>()
            .OrderByDescending(x => x.ID)
            .FirstOrDefault()?.ID;

        if (maxId == null)
        {
            return "IG000"; // Trường hợp chưa có profile nào
        }

        // Chuyển ID hiện tại thành số và cộng thêm 1
        int newIdNumber = int.Parse(maxId.Substring(2)) + 1;  // Lấy phần số sau "IG"
        return $"IG{newIdNumber:D3}";  // Tạo ID mới với 3 chữ số (ví dụ: IG001, IG002,...)
    }


    public UserAccount CreateNewAccount(string username, string email, string password, DateTime dateTime)
    {
        var existingUser = db.GetConnection().Table<UserAccount>()
            .Where(x => x.Username == username).FirstOrDefault();

        if (existingUser != null)
        {
            // Nếu đã tồn tại thì trả về null hoặc throw exception tùy thuộc vào yêu cầu
            Debug.Log("Username already exists!");
            return null;
        }

        // Tạo UserAccount mới
        var userAccount = new UserAccount
        {
            ID = GenerateUserId(),
            Username = username,
            Email = email,
            Password = password,
            Create_at = dateTime.ToString("yyyy-MM-dd HH:mm:ss")  // Chuyển DateTime thành chuỗi phù hợp với SQLite
        };
        CreateNewProfile(userAccount.ID);
        // Chèn UserAccount vào cơ sở dữ liệu
        db.GetConnection().Insert(userAccount);
        // Trả về đối tượng tài khoản mới
        return userAccount;
    }

    public void CreateNewProfile(string userId)
    {
        // Tạo IgProfile mới
        var igProfile = new IG_Profile
        {
            ID = GenerateProfileId(),
            ID_User = userId,  // Liên kết IgProfile với UserAccount thông qua ID
            Ig_Name = null,  // Bạn có thể để mặc định là null, hoặc nếu muốn, có thể yêu cầu người dùng nhập Ig_Name
            Role = "Member"  // Gán mặc định Role là "Member"
        };
        // Chèn IgProfile vào cơ sở dữ liệu
        db.GetConnection().Insert(igProfile);
    }

    public IEnumerable<IG_Record> GetAllRecordByProfileID(string proID)
    {
        return db.GetConnection().Table<IG_Record>().Where(x => x.ID_Profile == proID);
    }

    public string GenerateRecordId()
    {

        // Lấy ID lớn nhất hiện tại từ bảng IG_Record
        var maxId = db.GetConnection().Table<IG_Record>()
            .OrderByDescending(x => x.ID)
            .FirstOrDefault()?.ID;

        if (maxId == null)
        {
            return "REC000"; // Trường hợp chưa có record nào
        }

        // Chuyển ID hiện tại thành số và cộng thêm 1
        int newIdNumber = int.Parse(maxId.Substring(3)) + 1;  // Lấy phần số sau "REC"
        return $"REC{newIdNumber:D3}";  // Tạo ID mới với 3 chữ số (ví dụ: REC001, REC002,...)
    }


    public void SaveNewRecord(string profileID, int timez, int scorez, int kill)
    {

        var igRecord = new IG_Record
        {
            ID = GenerateRecordId(),
            ID_Profile = profileID,
            Time = timez,
            Kill = kill,
            Score = scorez,
            Save_at = DateTime.Now
        };
        db.GetConnection().Insert(igRecord);
    }
}
