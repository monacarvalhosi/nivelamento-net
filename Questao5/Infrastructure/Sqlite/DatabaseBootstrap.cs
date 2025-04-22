using System.Data.Common;
using System.Transactions;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Questao5.Infrastructure.Sqlite
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            connection.Open(); // Abre a conexão explicitamente

            // Inicia uma transação
            using var transaction = connection.BeginTransaction();

            try
            {
                // Verifica se as tabelas já existem
                var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND (name = 'contacorrente' or name = 'movimento' or name = 'idempotencia');");
                var tableName = table.FirstOrDefault();

                // Se as tabelas não existirem, cria as tabelas
                if (string.IsNullOrEmpty(tableName) || (tableName != "contacorrente" && tableName != "movimento" && tableName != "idempotencia"))
                {
                    connection.Execute("CREATE TABLE contacorrente ( " +
                                       "idcontacorrente TEXT(37) PRIMARY KEY," +
                                       "numero INTEGER(10) NOT NULL UNIQUE," +
                                       "nome TEXT(100) NOT NULL," +
                                       "ativo INTEGER(1) NOT NULL default 0," +
                                       "CHECK(ativo in (0, 1)) " +
                                       ");", transaction: transaction);

                    connection.Execute("CREATE TABLE movimento ( " +
                        "idmovimento TEXT(37) PRIMARY KEY," +
                        "idcontacorrente TEXT(37) NOT NULL," +
                        "datamovimento TEXT(25) NOT NULL," +
                        "tipomovimento TEXT(1) NOT NULL," +
                        "valor REAL NOT NULL," +
                        "CHECK(tipomovimento in ('C', 'D')), " +
                        "FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente) " +
                        ");", transaction: transaction);

                    connection.Execute("CREATE TABLE idempotencia (" +
                                       "chave_idempotencia TEXT(37) PRIMARY KEY," +
                                       "requisicao TEXT(1000)," +
                                       "resultado TEXT(1000));", transaction: transaction);
                }

                // Inserir dados na tabela contacorrente
                connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('B6BAFC09-6967-ED11-A567-055DFA4A16C9', 123, 'Katherine Sanchez', 1);", transaction: transaction);
                connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('FA99D033-7067-ED11-96C6-7C5DFA4A16C9', 456, 'Eva Woodward', 1);", transaction: transaction);
                connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('382D323D-7067-ED11-8866-7D5DFA4A16C9', 789, 'Tevin Mcconnell', 1);", transaction: transaction);
                connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('F475F943-7067-ED11-A06B-7E5DFA4A16C9', 741, 'Ameena Lynn', 0);", transaction: transaction);
                connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('BCDACA4A-7067-ED11-AF81-825DFA4A16C9', 852, 'Jarrad Mckee', 0);", transaction: transaction);
                connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('D2E02051-7067-ED11-94C0-835DFA4A16C9', 963, 'Elisha Simons', 0);", transaction: transaction);
               
                // Inserir dados na tabela idempotencia
                connection.Execute("INSERT INTO idempotencia(chave_idempotencia, requisicao, resultado) VALUES('I1I1I1I1-1I1I-11II-1III-1I1I1I1I1I1', 'req_123', 'success');", transaction: transaction);
                
                // Confirma as alterações feitas na transação
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Em caso de erro, desfaz todas as alterações feitas na transação
                transaction.Rollback();
                throw new Exception("Erro ao configurar o banco de dados", ex);
            }
        }
    }
}
