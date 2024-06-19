using System;
using Npgsql;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = "Host=ep-late-rain-a58675ir.us-east-2.aws.neon.tech;Username=AgendaBD_owner;Password=kMdtCTNS0P7q;Database=AgendaBD;SslMode=Require";

        await using var con = new NpgsqlConnection(connectionString);
        await con.OpenAsync();

        /*await using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = con;
            cmd.CommandText = "INSERT INTO persona (nombre, apellido, fechanacimiento, ci) VALUES ('Osama', 'Bin Laden', '2002-04-11', '73711838');";
            await cmd.ExecuteNonQueryAsync();
        }*/

        await using (var cmd = new NpgsqlCommand("SELECT * FROM persona", con))
        {
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine("id: " + reader["id"]);
                    Console.WriteLine("nombre: " + reader["nombre"]);
                    Console.WriteLine("apellido: " + reader["apellido"]);
                    Console.WriteLine("fechanacimiento: " + reader["fechanacimiento"]);
                    Console.WriteLine("ci: " + reader["ci"]);
                }
            }
        }

        Console.ReadLine();
    }
}