using Microsoft.Data.Sqlite;
using System.Data;
using System.Text;

namespace RestfulProcessControl.Util;

public class DatabaseConnection : IDisposable
{
	public abstract class DbTaskBase
	{
		protected SqliteCommand? Command { get; set; }
		protected bool Edited { get; set; }
		protected DatabaseConnection Manager { get; }

		protected DbTaskBase(DatabaseConnection manager)
		{
			Command = null;
			Edited = true;
			Manager = manager;
		}

		protected abstract bool GenerateCommand();
	}

	public class DbInsertTask : DbTaskBase
	{
		protected string? TableName { get; set; }
		protected List<(string, object)> Parameters { get; }

		public DbInsertTask(DatabaseConnection manager) : base(manager)
		{
			TableName = null;
			Parameters = new List<(string, object)>();
		}

		public DbInsertTask SetTable(string tableName)
		{
			TableName = tableName;
			Edited = true;
			return this;
		}

		public DbInsertTask AddParameter(string column, object value)
		{
			Parameters.Add((column, value));
			Edited = true;
			return this;
		}

		protected override bool GenerateCommand()
		{
			if (TableName is null) return false;
			try
			{
				StringBuilder commandTextBuilder = new();
				commandTextBuilder.Append("INSERT INTO ").Append(TableName).Append(" (");
				for (var i = 0; i < Parameters.Count; i++)
				{
					commandTextBuilder.Append(Parameters[i].Item1);
					if (i != Parameters.Count - 1) commandTextBuilder.Append(", ");
				}
				commandTextBuilder.Append(") VALUES (");
				for (var i = 0; i < Parameters.Count; i++)
				{
					commandTextBuilder.Append("$value").Append(Parameters[i].Item1);
					if (i != Parameters.Count - 1) commandTextBuilder.Append(", ");
				}
				commandTextBuilder.Append(')');

				Command = Manager.Connection.CreateCommand();
				Command.CommandText = commandTextBuilder.ToString();

				for (var i = 0; i < Parameters.Count; i++)
					Command.Parameters.AddWithValue($"$value{Parameters[i].Item1}", Parameters[i].Item2);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool TryExecute()
		{
			try
			{
				Manager.CheckConnection();
				if (Edited && !GenerateCommand()) return false;
				Edited = false;
				return Command!.ExecuteNonQuery() > 0;
			}
			catch
			{
				return false;
			}
		}
	}

	public class DbDeleteTask : DbTaskBase
	{
		protected string? TableName { get; set; }
		protected List<(string, object)> Equality { get; }

		public DbDeleteTask(DatabaseConnection manager) : base(manager)
		{
			TableName = null;
			Equality = new List<(string, object)>();
		}

		public DbDeleteTask SetTable(string tableName)
		{
			TableName = tableName;
			Edited = true;
			return this;
		}

		public DbDeleteTask IfEqual(string column, object value)
		{
			Equality.Add((column, value));
			Edited = true;
			return this;
		}

		protected override bool GenerateCommand()
		{
			if (TableName is null) return false;
			try
			{
				StringBuilder commandTextBuilder = new();
				commandTextBuilder.Append("DELETE FROM ").Append(TableName);
				if (Equality.Count > 0)
				{
					commandTextBuilder.Append(" WHERE");
					for (var i = 0; i < Equality.Count; i++)
					{
						if (i != 0) commandTextBuilder.Append(" AND");
						commandTextBuilder.Append(' ').Append(Equality[i].Item1).Append(" = $equal")
							.Append(Equality[i].Item1);
					}
				}

				Command = Manager.Connection.CreateCommand();
				Command.CommandText = commandTextBuilder.ToString();

				for (var i = 0; i < Equality.Count; i++)
					Command.Parameters.AddWithValue($"$equal{Equality[i].Item1}", Equality[i].Item2);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool TryExecute()
		{
			try
			{
				Manager.CheckConnection();
				if (Edited && !GenerateCommand()) return false;
				Edited = false;
				return Command!.ExecuteNonQuery() > 0;
			}
			catch
			{
				return false;
			}
		}
	}

	public class DbGetTask : DbTaskBase
	{
		protected List<string> Tables { get; }
		protected List<string> Columns { get; }
		protected List<(string, object)> Equality { get; }

		public DbGetTask(DatabaseConnection manager) : base(manager)
		{
			Tables = new List<string>();
			Columns = new List<string>();
			Equality = new List<(string, object)>();
		}

		public DbGetTask AddTable(string tableName)
		{
			Tables.Add(tableName);
			Edited = true;
			return this;
		}

		public DbGetTask AddColumn(string columnName)
		{
			Columns.Add(columnName);
			Edited = true;
			return this;
		}

		public DbGetTask IfEqual(string column, object value)
		{
			Equality.Add((column, value));
			Edited = true;
			return this;
		}

		protected override bool GenerateCommand()
		{
			if (Tables.Count < 1 || Columns.Count < 1) return false;
			try
			{
				StringBuilder commandTextBuilder = new();
				commandTextBuilder.Append("SELECT ");
				for (var i = 0; i < Columns.Count; i++)
				{
					commandTextBuilder.Append(Columns[i]);
					if (i != Columns.Count - 1) commandTextBuilder.Append(", ");
				}
				commandTextBuilder.Append(" FROM ");
				for (var i = 0; i < Tables.Count; i++)
				{
					commandTextBuilder.Append(Tables[i]);
					if (i != Tables.Count - 1) commandTextBuilder.Append(", ");
				}
				if (Equality.Count > 0)
				{
					commandTextBuilder.Append(" WHERE");
					for (var i = 0; i < Equality.Count; i++)
					{
						if (i != 0) commandTextBuilder.Append(" AND");
						commandTextBuilder.Append(' ').Append(Equality[i].Item1).Append(" = $equal")
							.Append(Equality[i].Item1);
					}
				}

				Command = Manager.Connection.CreateCommand();
				Command.CommandText = commandTextBuilder.ToString();

				for (var i = 0; i < Equality.Count; i++)
					Command.Parameters.AddWithValue($"$equal{Equality[i].Item1}", Equality[i].Item2);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool TryExecute(out Dictionary<string, List<object>> output)
		{
			output = new Dictionary<string, List<object>>();
			try
			{
				Manager.CheckConnection();
				if (Edited && !GenerateCommand()) return false;
				Edited = false;
				using var reader = Command!.ExecuteReader();
				for (var i = 0; i < reader.FieldCount; i++)
					output.Add(reader.GetName(i), new List<object>());
				while (reader.Read())
				{
					for (var i = 0; i < reader.FieldCount; i++)
						output[reader.GetName(i)].Add(reader.GetValue(i));
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}

	public class DbEditTask : DbTaskBase
	{
		protected string? TableName { get; set; }
		protected List<(string, object)> Edits { get; set; }
		protected List<(string, object)> Equality { get; set; }

		public DbEditTask(DatabaseConnection manager) : base(manager)
		{
			TableName = null;
			Edits = new List<(string, object)>();
			Equality = new List<(string, object)>();
		}

		public DbEditTask AddEdit(string columnName, object value)
		{
			Edits.Add((columnName, value));
			Edited = true;
			return this;
		}

		public DbEditTask SetTable(string tableName)
		{
			TableName = tableName;
			Edited = true;
			return this;
		}

		public DbEditTask IfEqual(string columnName, object value)
		{
			Equality.Add((columnName, value));
			Edited = true;
			return this;
		}

		protected override bool GenerateCommand()
		{
			if (TableName is null || Edits.Count < 1) return false;
			try
			{
				StringBuilder commandTextBuilder = new();
				commandTextBuilder.Append("UPDATE ").Append(TableName).Append(" SET ");
				for (var i = 0; i < Edits.Count; i++)
				{
					commandTextBuilder.Append(Edits[i].Item1).Append(" = $edit").Append(Edits[i].Item1);
					if (i != Edits.Count - 1) commandTextBuilder.Append(", ");
				}
				if (Equality.Count > 0)
				{
					commandTextBuilder.Append(" WHERE");
					for (var i = 0; i < Equality.Count; i++)
					{
						if (i != 0) commandTextBuilder.Append(" AND");
						commandTextBuilder.Append(' ').Append(Equality[i].Item1).Append(" = $equal")
							.Append(Equality[i].Item1);
					}
				}

				Command = Manager.Connection.CreateCommand();
				Command.CommandText = commandTextBuilder.ToString();

				for (var i = 0; i < Edits.Count; i++)
					Command.Parameters.AddWithValue($"$edit{Edits[i].Item1}", Edits[i].Item2);

				for (var i = 0; i < Equality.Count; i++)
					Command.Parameters.AddWithValue($"$equal{Equality[i].Item1}", Equality[i].Item2);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool TryExecute()
		{
			try
			{
				Manager.CheckConnection();
				if (Edited && !GenerateCommand()) return false;
				Edited = false;
				return Command!.ExecuteNonQuery() > 0;
			}
			catch
			{
				return false;
			}
		}
	}

	protected readonly SqliteConnection Connection;

	public DatabaseConnection(string connectionString) => Connection = new SqliteConnection(connectionString);

	public void Dispose()
	{
		Connection.Dispose();
		GC.SuppressFinalize(this);
	}

	public void CheckConnection()
	{
		if (Connection.State == ConnectionState.Open) return;
		Connection.Open();
	}

	public DbInsertTask Insert() => new(this);
	public DbDeleteTask Delete() => new(this);
	public DbGetTask Get() => new(this);
	public DbEditTask Edit() => new(this);
}