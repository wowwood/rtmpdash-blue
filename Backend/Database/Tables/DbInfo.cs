using LinqToDB.Mapping;

namespace RTMPDash.Backend.Database.Tables;

[Table(Name = "DbInfo")]
public class DbInfo {
	[Column(Name = "ID"), PrimaryKey, Identity, NotNull] public int Id    { get; set; }
	[Column(Name = "DbVer"), NotNull]                    public int DbVer { get; set; }
}
