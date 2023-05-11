// See https://aka.ms/new-console-template for more information
using FastMember;
using Kyklos.Kernel.Core.KLinq;
using Kyklos.Kernel.Core.Objects;
using Kyklos.Kernel.Core.Reflection;
using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;
using System.Data;
using System.Reflection;

FastMemberExtension.TestAnonymousObj();
var a = true;

public static class FastMemberExtension
{
    public static void TestAnonymousObj()
    {
var anon = new
{
    Prop1 = "a",
    Prop2 = "b"
};

MemberSet members =
    TypeAccessor
        .Create(anon.GetType())
        .GetMembers();
    }

    public static Member[] GetOrderedPropertyList(this Type type) =>
        TypeAccessor
            .Create(type)
            .GetOrderedPropertyList(type);

    public static Member[] GetOrderedPropertyList(this TypeAccessor typeAccessor, Type type)
    {
        Dictionary<string, Member> memberDict =
            typeAccessor
                .GetMembers()
                .ToDictionary(x => x.Name);

        string[]? baseClassProperties =
            type
                .BaseType
                ?.GetPropertiesForType()
                ?.Select(x => x.Name)
                ?.ToArray();

        HashSet<string> baseClassPropertiesSet = new(baseClassProperties.ToEmptyIfNull(), StringComparer.OrdinalIgnoreCase);
        List<Member> baseClassePropList = new();
        List<Member> concreteClassePropList = new();

        foreach (PropertyInfo prop in type.GetPropertiesForType())
        {
            Member? member = memberDict.GetValueOrDefault(prop.Name);
            if (member.IsReferenceNull())
            {
                continue;
            }

            if (baseClassPropertiesSet.Contains(prop.Name))
            {
                baseClassePropList.Add(member);
            }
            else
            {
                concreteClassePropList.Add(member);
            }
        }

        return
            baseClassePropList
                .Union(concreteClassePropList)
                .ToArray();
    }
}



public class BaseExcelExportEntity : IBaseEntity
{
    [EntityPropertyInfo(ColumnName = "RMX_CompanyCode", DbType = PropertyDbType.Integer, IsNullable = false, ColumnOrder = 10)]
    public int RMX_CompanyCode { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_Brand", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 3, ColumnOrder = 20)]
    public string RMX_Brand { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_PracticeYear", DbType = PropertyDbType.Short, IsNullable = false, ColumnOrder = 30)]
    public short RMX_PracticeYear { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_PracticeDepartment", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 4, ColumnOrder = 40)]
    public string RMX_PracticeDepartment { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_PracticeNumber", DbType = PropertyDbType.Integer, IsNullable = false, ColumnOrder = 50)]
    public int RMX_PracticeNumber { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_PracticeSplit", DbType = PropertyDbType.Short, IsNullable = true, ColumnOrder = 60)]
    public short? RMX_PracticeSplit { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_CustomerCode", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 6, ColumnOrder = 70)]
    public string RMX_CustomerCode { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_CustomerTypology", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 10, ColumnOrder = 80)]
    public string RMX_CustomerTypology { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_CreationDate", DbType = PropertyDbType.DateTime, IsNullable = true, ColumnOrder = 90)]
    public DateTime? RMX_CreationDate { get; set; }

    [EntityPropertyInfo(ColumnName = "RMX_UserCreation", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 100, ColumnOrder = 100)]
    public string RMX_UserCreation { get; set; }
}



[EntityObjectInfo(TableName = "ReportDefectsBoxesImages", IsView = true)]
public class ReportDefectsBoxesImageDbItem : BaseExcelExportEntity
{
    [EntityPropertyInfo(ColumnName = "PraticaAPE", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 50, ColumnOrder = 110)]
    public string PraticaAPE { get; set; }

    [EntityPropertyInfo(ColumnName = "CodiceMittente", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 50, ColumnOrder = 120)]
    public string CodiceMittente { get; set; }

    [EntityPropertyInfo(ColumnName = "DescrizioneMittente", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 500, ColumnOrder = 130)]
    public string DescrizioneMittente { get; set; }

    [EntityPropertyInfo(ColumnName = "DescrizioneDestinatario", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 500, ColumnOrder = 140)]
    public string DescrizioneDestinatario { get; set; }

    [EntityPropertyInfo(ColumnName = "Collo", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 50, ColumnOrder = 150)]
    public string Collo { get; set; }

    [EntityPropertyInfo(ColumnName = "ImmagineDifettoCollo", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 1, ColumnOrder = 160)]
    public string ImmagineDifettoCollo { get; set; }

    [EntityPropertyInfo(ColumnName = "RACompany", DbType = PropertyDbType.Integer, IsNullable = true, ColumnOrder = 170)]
    public int? RACompany { get; set; }

    [EntityPropertyInfo(ColumnName = "RABrand", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 3, ColumnOrder = 180)]
    public string RABrand { get; set; }

    [EntityPropertyInfo(ColumnName = "RAYear", DbType = PropertyDbType.Short, IsNullable = true, ColumnOrder = 190)]
    public short? RAYear { get; set; }

    [EntityPropertyInfo(ColumnName = "RADepartment", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 4, ColumnOrder = 200)]
    public string RADepartment { get; set; }

    [EntityPropertyInfo(ColumnName = "RANumber", DbType = PropertyDbType.Integer, IsNullable = true, ColumnOrder = 210)]
    public int? RANumber { get; set; }

    [EntityPropertyInfo(ColumnName = "RASplit", DbType = PropertyDbType.Short, IsNullable = true, ColumnOrder = 220)]
    public short? RASplit { get; set; }

}