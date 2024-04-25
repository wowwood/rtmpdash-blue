using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using RTMPDash.Backend.Database.Tables;
using RTMPDash.Pages;

namespace RTMPDash.Backend;

public static class Migrations {
	private const int DbVer = 2;

	// ReSharper disable once InconsistentNaming
	private static readonly List<Migration> _migrations = new() {
		new Migration(1, "ALTER TABLE Users ADD IsPrivate INTEGER DEFAULT 0 NOT NULL"),
		new Migration(1, "ALTER TABLE Users ADD PrivateAccessKey TEXT"),
		new Migration(2, "ALTER TABLE Users ADD PronounPlural INTEGER DEFAULT 0 NOT NULL"),
		new Migration(2, "UPDATE Users SET PronounPlural = 1 WHERE PronounSubject = 'they'")
	};

	public static void RunMigrations() {
		using var db     = new Database.Database.DbConn();
		var       ccolor = Console.ForegroundColor;

		if (!db.DataProvider.GetSchemaProvider().GetSchema(db).Tables.Any()) {
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write("Running migration: ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Initialize Database");

			db.CreateTable<User>();
			db.CreateTable<Invite>();
			db.CreateTable<DbInfo>();
			db.InsertWithIdentity(new DbInfo { DbVer = DbVer });
			var password = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..12];
			db.InsertWithIdentity(new User {
									  Username          = "admin",
									  Password          = password.Sha256(),
									  StreamKey         = Guid.NewGuid().ToString(),
									  PronounSubject    = "they",
									  PronounPossessive = "their",
									  PronounPlural     = true,
									  AllowRestream     = true,
									  IsAdmin           = true
								  });
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("The user ");
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			Console.Write("admin ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("has been created with the password ");
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			Console.WriteLine(password);
		}
		else if (db.DataProvider.GetSchemaProvider().GetSchema(db).Tables.All(t => t.TableName != "DbInfo")) {
			db.CreateTable<DbInfo>();
			db.InsertWithIdentity(new DbInfo { DbVer = 0 });
		}

		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine($"Database version: {db.DbInfo.ToList().First().DbVer}");

		var migrationsToRun = _migrations.FindAll(p => p.IntroducedWithDbVer > db.DbInfo.First().DbVer);
		if (migrationsToRun.Count == 0) {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("No migrations to run.");
		}
		else {
			new Migration(0, "BEGIN TRANSACTION").Run(db);
			try {
				migrationsToRun.ForEach(p => p.Run(db));
			}
			catch {
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine($"Migrating to database version {DbVer} failed.");
				new Migration(0, "ROLLBACK TRANSACTION").Run(db);
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("Rolled back migrations.");
				Environment.Exit(1);
			}

			new Migration(0, "COMMIT TRANSACTION").Run(db);

			var newdb  = new Database.Database.DbConn();
			var dbinfo = newdb.DbInfo.First();
			dbinfo.DbVer = DbVer;
			newdb.Update(dbinfo);

			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine($"Database version is now: {DbVer}");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Finished running migrations.");
		}

		Console.ForegroundColor = ccolor;
	}

	private class Migration {
		private readonly string _sql;
		public readonly  int    IntroducedWithDbVer;

		public Migration(int introducedWithDbVer, string sql) {
			IntroducedWithDbVer = introducedWithDbVer;
			_sql                = sql;
		}

		public void Run(DataConnection db) {
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write("Running migration: ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(_sql);
			db.Execute(_sql);
		}
	}
}
