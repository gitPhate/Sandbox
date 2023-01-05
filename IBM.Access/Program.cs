using IBM.Data.DB2.iSeries;
using System;
using System.Configuration;
using System.Data;
using System.IO;

namespace IBM.Access
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable dt = new DataTable();

            using (iDB2Connection conn = new iDB2Connection(ConfigurationManager.ConnectionStrings["AS400"]?.ConnectionString))
            {
                conn.Open();

                string query = File.ReadAllText(ConfigurationManager.AppSettings["QueryFilePath"]);
                using (iDB2Command cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    using (iDB2DataAdapter da = new iDB2DataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            foreach (DataRow dataRow in dt.Rows)
            {
                Console.WriteLine("Row:");

                foreach (var item in dataRow.ItemArray)
                {
                    Console.WriteLine(item);
                }

                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}
