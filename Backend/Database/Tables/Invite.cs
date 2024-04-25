using LinqToDB.Mapping;

namespace RTMPDash.Backend.Database.Tables; 

[Table(Name = "Invites")]
public class Invite {
	[Column(Name = "Code"), PrimaryKey, NotNull] public string Code { get; set; }
}
