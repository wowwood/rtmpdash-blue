using LinqToDB.Mapping;

namespace RTMPDash.Backend.Database.Tables;

[Table(Name = "Users")]
public class User {
	[Column(Name = "Username"), PrimaryKey] public string Username          { get; set; }
	[Column(Name = "Password")]             public string Password          { get; set; }
	[Column(Name = "IsAdmin")]              public bool   IsAdmin           { get; set; }
	[Column(Name = "AllowRestream")]        public bool   AllowRestream     { get; set; }
	[Column(Name = "StreamKey")]            public string StreamKey         { get; set; }
	[Column(Name = "PronounSubject")]       public string PronounSubject    { get; set; }
	[Column(Name = "PronounPossessive")]    public string PronounPossessive { get; set; }
	[Column(Name = "ChatUrl")]              public string ChatUrl           { get; set; }
	[Column(Name = "AnnouncementUrl")]      public string AnnouncementUrl   { get; set; }
	[Column(Name = "RestreamTargets")]      public string RestreamTargets   { get; set; }
	[Column(Name = "RestreamUrls")]         public string RestreamUrls      { get; set; }
	[Column(Name = "IsPrivate")]            public bool   IsPrivate         { get; set; }
	[Column(Name = "PrivateAccessKey")]     public string PrivateAccessKey  { get; set; }
	[Column(Name = "PronounPlural")]        public bool   PronounPlural     { get; set; }
}