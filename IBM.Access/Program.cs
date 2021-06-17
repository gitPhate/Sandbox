using IBM.Data.DB2.iSeries;
using System;
using System.Data;

namespace IBM.Access
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                throw new Exception("Specify EMEA server name");
            }

            DataTable dt = new DataTable();

            using (iDB2Connection conn = new iDB2Connection($"Data Source={args[0]}.emea.guccigroup.dom;Persist Security Info=True;User Id=KKMIRROR;Password=DBMR21K3;"))
            {
                conn.Open();

                using (iDB2Command cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from ST3IMPRE.RECAU5FF";

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
