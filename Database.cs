using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MaterializeSignalR
{
    public static class Database
    {
        static readonly string databasePath;

        static Database()
        {
            databasePath = Path.Combine("wwwroot", "database", "Database.db");
        }

        public static void SaveToDatabase(UserData user)
        {
            try
            {
                List<UserData> all = GetAllFromDatabase() ?? new List<UserData>();
                XmlSerializer xml = new XmlSerializer(typeof(List<UserData>));

                using FileStream fs = new FileStream(databasePath, FileMode.Create);

                if (!all.Any(p => p.UserName == user.UserName))
                {
                    all.Add(user);
                }

                xml.Serialize(fs, all);
            }
            catch { }
        }

        public static List<UserData> GetAllFromDatabase()
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<UserData>));
                using FileStream fs = new FileStream(databasePath, FileMode.Open);
                return (List<UserData>)xml.Deserialize(fs);
            }
            catch
            {
                return new List<UserData>();
            }
        }

        public static UserData GetFromDatabase(string email)
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<UserData>));
                using FileStream fs = new FileStream(databasePath, FileMode.Open);
                List<UserData> list = null;

                try
                {
                    list = (List<UserData>)xml.Deserialize(fs);
                }
                catch { }

                return list.Where(p => p.Email == email).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public static bool ExistInDatabase(string email)
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<UserData>));
                using FileStream fs = new FileStream(databasePath, FileMode.Open);
                List<UserData> list = null;

                try
                {
                    list = (List<UserData>)xml.Deserialize(fs);
                }
                catch
                {
                    return false;
                }

                return list.Any(p => p.Email == email);
            }
            catch
            {
                return false;
            }
        }
    }
}
