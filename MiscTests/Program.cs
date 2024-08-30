using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.Support;
using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;
using Medallion.Threading;
using Medallion.Threading.Postgres;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

IAsyncDao dao =
    AsyncDaoFactory
        .CreateAsyncDao
        (
            "User ID=kering-stg-apeevo;Password=jXKEVvxFJ6sj9DrHQBLi;Host=kkapestgdb.kyklos.local;Port=5432;Database=kering_dev_apeevo;Include Error Detail=True;",
            "Postgres",
            "public"
        );


IDistributedLockProvider synchronizationProvider = new PostgresDistributedSynchronizationProvider(dao.ConnectionString);
List<Task<long>> tasks = [];
foreach (var i in Enumerable.Range(0, 30))
{
    Task<long> t = GetNextCounterAsync(dao, synchronizationProvider);

    tasks.Add(t);
}

await Task.WhenAll(tasks);

var results =
    tasks
        .Select(x => x.Result)
        .ToArray();

var ordered = results.OrderBy(x => x).ToArray();

bool x = true;

static async Task<long> GetNextCounterAsync(IAsyncDao dao, IDistributedLockProvider synchronizationProvider)
{
    DateTime now = DateTime.Now;

    CounterDbItem counterItem =
            await dao
                .GetItemByExampleAsync<CounterDbItem>
                (
                    x => x.IsActive && x.CounterCode == "test"
                );

    bool toBeReset = counterItem.ResetByYear && counterItem.LastChangeDate.Year < now.Year;
    if (toBeReset)
    {
        await using (await synchronizationProvider.AcquireLockAsync("_counters_"))
        {
            counterItem =
                await dao
                    .GetItemByExampleAsync<CounterDbItem>
                    (
                        x => x.IsActive && x.CounterCode == "test"
                    );

            toBeReset = counterItem.ResetByYear && counterItem.LastChangeDate.Year < now.Year;
        }
    }

    long data = -1;

    counterItem.LastChangeUser = "gricci";
    counterItem.LastChangeDate = now;

    if (toBeReset)
    {
        await dao
            .DoInTransactionAsync
            (async tdao =>
            {
                await ResetSequenceAsync(dao, counterItem.SequenceName, counterItem.InitialValue);
                await tdao
                    .UpdateEntityAsync(counterItem);
            }
            );
    }

    await dao
        .DoInTransactionAsync
        (async tdao =>
        {
            data =
                await dao
                .GetNextValueForSequenceAsync(counterItem.SequenceName);

            await tdao
                .UpdateEntityAsync(counterItem);
        });

    return data;
}

static async Task ResetSequenceAsync(IAsyncDao dao, string sequenceName, int resetTo)
{
    string escapedSeqName = dao.DbProviderHelper.EscapeTableName(sequenceName) ?? sequenceName;

    string sql =
        dao.DbProviderHelper.DbProviderName switch
        {
            "SqlServer" => $"ALTER SEQUENCE {escapedSeqName} RESTART WITH {resetTo}",
            "Oracle" => $"alter sequence {escapedSeqName} restart start with {resetTo};",
            "PostgreSQL" => $"ALTER SEQUENCE {escapedSeqName} RESTART WITH {resetTo + 1};",
            _ => throw new NotSupportedException($"The provider {dao.DbProviderHelper.DbProviderName} is not supported")
        };

    await dao
        .ExecuteNonQueryAsync(sql)
        .ConfigureAwait(false);
}



[EntityObjectInfo(TableName = "Counters")]
[EntityUniqueConstraint(ConstraintName = "Counters_UX", Properties = [nameof(CounterCode)])]
public class CounterDbItem : IBaseEntity
{
    [EntityPropertyInfo(ColumnName = "IdCounter", DbType = PropertyDbType.Long, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 0)]
    public long IdCounter { get; set; }

    [EntityPropertyInfo(ColumnName = "CounterCode", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 10, ColumnOrder = 10)]
    public string CounterCode { get; set; }

    [EntityPropertyInfo(ColumnName = "CounterDescr", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 30, ColumnOrder = 20)]
    public string CounterDescr { get; set; }

    [EntityPropertyInfo(ColumnName = "SequenceName", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 20, ColumnOrder = 30)]
    public string SequenceName { get; set; }

    [EntityPropertyInfo(ColumnName = "ResetByYear", DbType = PropertyDbType.Boolean, IsNullable = false, ColumnOrder = 40)]
    public bool ResetByYear { get; set; }

    [EntityPropertyInfo(ColumnName = "InitialValue", DbType = PropertyDbType.Integer, IsNullable = false, ColumnOrder = 50)]
    public int InitialValue { get; set; }

    [EntityPropertyInfo(ColumnName = "CounterMask", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 100, ColumnOrder = 60)]
    public string CounterMask { get; set; }

    [EntityPropertyInfo(ColumnName = "IsActive", DbType = PropertyDbType.Boolean, IsNullable = false, ColumnOrder = 50)]
    public bool IsActive { get; set; }

    [EntityPropertyInfo(ColumnName = "CreationDate", DbType = PropertyDbType.DateTime, IsNullable = false, ColumnOrder = 60)]
    public DateTime CreationDate { get; set; }

    [EntityPropertyInfo(ColumnName = "LastChangeDate", DbType = PropertyDbType.DateTime, IsNullable = false, ColumnOrder = 70)]
    public DateTime LastChangeDate { get; set; }

    [EntityPropertyInfo(ColumnName = "CreationUser", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 100, ColumnOrder = 80)]
    public string CreationUser { get; set; }

    [EntityPropertyInfo(ColumnName = "LastChangeUser", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 100, ColumnOrder = 90)]
    public string LastChangeUser { get; set; }
}