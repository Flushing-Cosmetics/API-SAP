using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WS_SAP;

/// <summary>
/// Provides parameterized SQL Server access and owns the connection, command and transaction resources.
/// </summary>
public sealed class DB : IDisposable, IAsyncDisposable
{
    private static readonly Lazy<IConfiguration> configuration = new(CargarConfiguracion);
    private readonly string connectionString;
    private readonly int commandTimeout;
    private SqlConnection? connection;
    private SqlCommand? command;
    private SqlTransaction? transaction;

    public DB()
        : this(configuration.Value)
    {
    }

    public DB(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        IConfigurationSection database = configuration.GetRequiredSection("Database");
        string server = database["Server"]
            ?? throw new InvalidOperationException("Missing required configuration value: Database:Server");
        string name = database["Name"]
            ?? throw new InvalidOperationException("Missing required configuration value: Database:Name");
        string user = database["User"]
            ?? throw new InvalidOperationException("Missing required configuration value: Database:User");
        string password = database["Password"]
            ?? throw new InvalidOperationException("Missing required configuration value: Database:Password");

        connectionString = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = name,
            UserID = user,
            Password = password,
            Encrypt = true,
            TrustServerCertificate = database.GetValue("TrustServerCertificate", true)
        }.ConnectionString;
        commandTimeout = database.GetValue("CommandTimeout", 30);
    }

    public void Conectar()
    {
        EnsureConnection().Open();
    }

    public Task ConectarAsync(CancellationToken cancellationToken = default)
    {
        return EnsureConnection().OpenAsync(cancellationToken);
    }

    public void Desconectar()
    {
        transaction?.Dispose();
        transaction = null;
        command?.Dispose();
        command = null;

        if (connection?.State != ConnectionState.Closed)
        {
            connection?.Close();
        }
    }

    public void CrearComando(string consulta, CommandType commandType = CommandType.Text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(consulta);

        SqlConnection openConnection = EnsureConnection();
        if (openConnection.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connect to the database before creating a command.");
        }

        command?.Dispose();
        command = new SqlCommand(consulta, openConnection, transaction)
        {
            CommandType = commandType,
            CommandTimeout = commandTimeout
        };
    }

    public void AsignarParametro(string nombre, SqlDbType tipo, object? valor, int? size = null)
    {
        SqlCommand currentCommand = GetCommand();
        SqlParameter parameter = currentCommand.Parameters.Add(nombre, tipo);
        parameter.Value = valor ?? DBNull.Value;

        if (size.HasValue)
        {
            parameter.Size = size.Value;
        }
    }

    public void LimpiarParametros()
    {
        GetCommand().Parameters.Clear();
    }

    public int EjecutarConsulta()
    {
        return GetCommand().ExecuteNonQuery();
    }

    public Task<int> EjecutarConsultaAsync(CancellationToken cancellationToken = default)
    {
        return GetCommand().ExecuteNonQueryAsync(cancellationToken);
    }

    public object? EjecutarScalar()
    {
        object? result = GetCommand().ExecuteScalar();
        return result is DBNull ? null : result;
    }

    public async Task<T?> EjecutarScalarAsync<T>(CancellationToken cancellationToken = default)
    {
        object? result = await GetCommand().ExecuteScalarAsync(cancellationToken);
        return result is null or DBNull ? default : (T)Convert.ChangeType(result, typeof(T));
    }

    public DataTable EjecutarDataTable()
    {
        DataTable table = new();
        using SqlDataAdapter adapter = new(GetCommand());
        adapter.Fill(table);
        return table;
    }

    public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        SqlConnection openConnection = EnsureConnection();
        if (openConnection.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connect to the database before starting a transaction.");
        }

        transaction ??= openConnection.BeginTransaction(isolationLevel);
        if (command is not null)
        {
            command.Transaction = transaction;
        }
    }

    public void Commit()
    {
        transaction?.Commit();
        DisposeTransaction();
    }

    public void Rollback()
    {
        transaction?.Rollback();
        DisposeTransaction();
    }

    public void Dispose()
    {
        Desconectar();
        connection?.Dispose();
        connection = null;
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        Desconectar();
        if (connection is not null)
        {
            await connection.DisposeAsync();
            connection = null;
        }

        GC.SuppressFinalize(this);
    }

    private SqlConnection EnsureConnection()
    {
        return connection ??= new SqlConnection(connectionString);
    }

    private SqlCommand GetCommand()
    {
        return command ?? throw new InvalidOperationException("Create a command before executing it.");
    }

    private void DisposeTransaction()
    {
        transaction?.Dispose();
        transaction = null;

        if (command is not null)
        {
            command.Transaction = null;
        }
    }

    private static IConfiguration CargarConfiguracion()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("webconfig.json", optional: false, reloadOnChange: false)
            .Build();
    }
}
