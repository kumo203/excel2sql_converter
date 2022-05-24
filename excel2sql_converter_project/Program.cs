using Csv;
using System;
using System.Collections.Generic;
using System.Text;

namespace excel2sql_converter // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        class AddingUser
        {
            public int Id { get; set; }
            public int MemberId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string LastNameKana { get; set; }
            public string FirstNameKana { get; set; }
            public string NickName { get; set; }
            public string Email { get; set; }
            public string Director { get; set; }
            public string Location { get; set; }
            public string BusinessContent { get; set; }
            public string SpecialSkill { get; set; }
            public string Skill { get; set; }
            public string Remark { get; set; }
            public string Crown { get; set; }
            public string SwimmyPoint { get; set; }
            public string CashPoint { get; set; }
            public string ExperiencePoint { get; set; }
            //public string CreatedAt { get; set; } 
            //public string UpdatedAt { get; set; }
            public string EncriptedPassword { get; set; }
            public string PasswordText { get; set; }
        }

        static readonly string PasswordPatternName = "password_pattern.csv";
        static readonly string TimeAt = DateTime.Now.ToString("yyy-MM-dd") + " 00:00:00.000000";
        static readonly string UserSqlName = "AddingUser.sql";
        static readonly string UserPointSqlName = "AddingUserPoint.sql";
        static readonly string UserCrownSqlName = "AddingUserCrown.sql";
        static readonly string UserPasswordList = "AddingUserPasswordList.csv";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 4)
                {
                    Console.WriteLine("csv2sql_converter target.csv usersテーブル追加開始ID user_pointsテーブル追加開始ID user_crownsテーブル追加開始ID {usersテーブル追加開始member_id}");
                    return;
                }
                var target = args[0];
                int id = int.Parse(args[1]);
                int userPointIdCount = int.Parse(args[2]);
                int userCrownIdCount = int.Parse(args[3]);

                int memberId = 1_000_000_000;
                if (args.Length > 4)
                {
                    memberId = int.Parse(args[4]);
                }

                var options = new CsvOptions
                {
                    AllowNewLineInEnclosedFieldValues = true,
                };

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
 
                string password_pattern_csv = new StreamReader(PasswordPatternName, Encoding.GetEncoding(932)).ReadToEnd();
                List<string[]> passwordPatterns = new List<string[]>();
                foreach (var line in CsvReader.ReadFromText(password_pattern_csv, options))
                {
                    passwordPatterns.Add(new string[] { line[0], line[1] });
                }

                string target_csv = new StreamReader(target, Encoding.GetEncoding(932)).ReadToEnd();
                List<AddingUser> list = new List<AddingUser>();
                int i = 0;
                foreach (var line in CsvReader.ReadFromText(target_csv, options))
                {
                    var user = new AddingUser() {
                        Id = id++,
                        MemberId = memberId++,
                        LastName = line["姓"],
                        FirstName = line["名"],
                        LastNameKana = line["姓カナ"],
                        FirstNameKana = line["名カナ"],
                        NickName = line["ニックネーム"],
                        Email = line["Eメール"],
                        Director = line["役職"],
                        Location = line["所在"],
                        BusinessContent = line["業務内容"],
                        SpecialSkill = line["特技"],
                        Skill = line["スキル"],
                        Remark = line["備考"],
                        Crown = line["トロフィー"],
                        SwimmyPoint = line["Sp"],
                        CashPoint = line["Cp"],
                        ExperiencePoint = line["Exp"],
                        PasswordText = passwordPatterns[i][0],
                        EncriptedPassword = passwordPatterns[i][1]
                    };
                    list.Add(user);

                    if (++i>= passwordPatterns.Count)
                    {
                        i = 0;
                    }
                }

                StringBuilder userSqlSb = new StringBuilder(); 
                foreach (var user in list)
                {
                    userSqlSb.AppendLine(String.Format(
                        "INSERT INTO `app_development`.`users` (" +
                        "`id`, " +
                        "`email`, " +
                        "`encrypted_password`, " +
                        "`created_at`, " +
                        "`updated_at`, " +
                        "`member_id`, " +
                        "`first_name`, " +
                        "`last_name`, " +
                        "`first_name_kana`, " +
                        "`last_name_kana`, " +
                        "`nickname`, " +
                        "`director`, " +
                        "`location`, " +
                        "`business_content`, " +
                        "`special_skill`, " +
                        "`skill`, " +
                        "`remark`) " +
                        "VALUES (" +
                        "'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}'" +
                        ");",
                        user.Id,
                        user.Email,
                        user.EncriptedPassword,
                        TimeAt,
                        TimeAt,
                        user.MemberId,
                        user.FirstName,
                        user.LastName,
                        user.FirstNameKana,
                        user.LastNameKana,
                        user.NickName,
                        user.Director,
                        user.Location,
                        user.BusinessContent,
                        user.SpecialSkill,
                        user.Skill,
                        user.Remark
                        ));
                }
                File.WriteAllText(UserSqlName, userSqlSb.ToString());

                StringBuilder userPointSqlSb = new StringBuilder();
                foreach (var user in list)
                {
                    userPointSqlSb.AppendLine(string.Format(
                        "INSERT INTO `app_development`.`user_points` (" +
                        "`id`, " +
                        "`user_id`, " +
                        "`cp_point`, " +
                        "`sp_point`, " +
                        "`point_avg`, " +
                        "`quest_count`, " +
                        "`owner_point_avg`, " +
                        "`owner_quest_count`, " +
                        "`created_at`, " +
                        "`updated_at`, " +
                        "`exp" +
                        "`) VALUES (" +
                        "'{0}', '{1}', '{2}', '{3}', '0', '0', '0', '0', '{4}', '{5}', '{6}');",
                        userPointIdCount++,
                        user.Id,
                        user.CashPoint,
                        user.SwimmyPoint,
                        TimeAt,
                        TimeAt,
                        user.ExperiencePoint
                        ));
                }
                File.WriteAllText(UserPointSqlName, userPointSqlSb.ToString());

                StringBuilder userCrownSqlSb = new StringBuilder();
                foreach (var user in list)
                {
                    foreach(var crown in user.Crown.Split(','))
                    {
                        if (crown.Length <= 0)
                        {
                            break;
                        }

                        userCrownSqlSb.AppendLine(String.Format(
                            "INSERT INTO `app_development`.`user_crowns` (" +
                            "`id`, " +
                            "`user_id`, " +
                            "`crown_name`, " +
                            "`created_at`, " +
                            "`updated_at`" +
                            ") VALUES (" +
                            "'{0}', '{1}', '{2}', '{3}', '{4}');",
                            userCrownIdCount++,
                            user.Id,
                            crown,
                            TimeAt,
                            TimeAt));
                    }
                }
                File.WriteAllText(UserCrownSqlName, userCrownSqlSb.ToString());

                StringBuilder userPasswordListSb = new StringBuilder();
                userPasswordListSb.AppendLine("ユーザーEmail,パスワード");
                foreach (var user in list)
                {
                    userPasswordListSb.AppendLine($"{user.Email},{user.PasswordText}");
                }
                File.WriteAllText(UserPasswordList, userPasswordListSb.ToString());

                Console.WriteLine("AddingUser.sqlとそれに付随するファイルを生成しました。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("エラー");
                Console.WriteLine(ex);
            }
        }
    }
}