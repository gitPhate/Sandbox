using Alfa.Data.DB2.iSeries;
using Kyklos.Kernel.Core.KLinq;
using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.SqlBuilders;
using Kyklos.Kernel.Settings.Database;
using Sisifo.Framework.Data.Async;
using System.Configuration;
using System.Data;

iDB2ConnectionStringBuilder builder = new()
{
    DataSource = "GGEURO.emea.guccigroup.dom",
    UserID = "KKMIRROR",
    Password = "DBMR21K3",
    PersistSecurityInfo = true
};

CryptedDatabaseInfoSettings dbInfoSettings = new
(
    "GGEURO",
    builder.ConnectionString,
    "DB2iSeries",
    string.Empty,
    "GGEURO"
);

IAsyncDao dao = SisifoAsyncDaoFactory.NewAsyncDao(dbInfoSettings, null, "MYV3RLIB");

var res =
    await dao
        .NewQueryBuilder()
        .CustomSql("select ASNUM from MYV3RLIB.APSUPX x")
        .GetItemsArrayAsync<string>();

res.ForEach(x => Console.WriteLine(x));

//---------------------//

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