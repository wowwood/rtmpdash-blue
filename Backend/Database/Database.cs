using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using RTMPDash.Backend.Database.Tables;

namespace RTMPDash.Backend.Database;

public class Database {
	public class ConnectionStringSettings : IConnectionStringSettings {
		public string ConnectionString { get; set; }
		public string Name             { get; set; }
		public string ProviderName     { get; set; }
		public bool   IsGlobal         => false;
	}

	public class Settings : ILinqToDBSettings {
		public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

		public string DefaultConfiguration => "SQLite";
		public string DefaultDataProvider  => "SQLite";

		public IEnumerable<IConnectionStringSettings> ConnectionStrings {
			get { yield return new ConnectionStringSettings { Name = "db", ProviderName = "SQLite", ConnectionString = @"Data Source=app.db;" }; }
		}
	}

	public class DbConn : DataConnection {
		public DbConn() : base("db") { }

		public ITable<User>   Users   => GetTable<User>();
		public ITable<Invite> Invites => GetTable<Invite>();
		public ITable<DbInfo> DbInfo  => GetTable<DbInfo>();
	}
}
