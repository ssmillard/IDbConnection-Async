﻿// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class SqlServer
    {
        private static string connectionString;

        [ClassInitialize()]
        public static void ClassInit(TestContext testcontext)
        {
            connectionString = Utilities.CreateTestDatabase();
        }

        [TestMethod]
        public async Task Reader_NoToken()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Name FROM Person;";
                using (IDataReader reader = await command.ExecuteReaderAsync())
                {
                    do
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!await reader.IsDBNullAsync(0))
                            {
                                var name = reader.GetFieldValueAsync<string>(0);
                                Assert.IsNotNull(name);
                            }
                        }
                    } while (await reader.NextResultAsync());
                }
            }
        }

        [TestMethod]
        public async Task Reader_WithToken()
        {
            var token = CancellationToken.None;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(token);

                IDbCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Name FROM Person;";
                using (IDataReader reader = await command.ExecuteReaderAsync(token))
                {
                    do
                    {
                        while (await reader.ReadAsync(token))
                        {
                            if (!await reader.IsDBNullAsync(0,token))
                            {
                                var name = reader.GetFieldValueAsync<string>(0,token);
                                Assert.IsNotNull(name);
                            }
                        }
                    } while (await reader.NextResultAsync(token));
                }
            }
        }


        [TestMethod]
        public async Task Reader_With_Behavior_Token()
        {
            var token = CancellationToken.None;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(token);

                IDbCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Name FROM Person;";
                using (IDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, token))
                {
                    do
                    {
                        while (await reader.ReadAsync(token))
                        {
                            if (!await reader.IsDBNullAsync(0, token))
                            {
                                var name = reader.GetFieldValueAsync<string>(0, token);
                                Assert.IsNotNull(name);
                            }
                        }
                    } while (await reader.NextResultAsync(token));
                }
            }
        }
        [TestMethod]
        public async Task Reader_With_Behavior_NoToken()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = "SELECT Name FROM Person;";
                using (IDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    do
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!await reader.IsDBNullAsync(0))
                            {
                                var name = reader.GetFieldValueAsync<string>(0);
                                Assert.IsNotNull(name);
                            }
                        }
                    } while (await reader.NextResultAsync());
                }
            }
        }


        [TestMethod]
        public async Task Scalar_With_NoToken()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = "SELECT TOP(1) Name FROM Person;";
                var value = await command.ExecuteScalarAsync();
                Assert.AreEqual("Adam",value);
            }
        }

        [TestMethod]
        public async Task Scalar_With_Token()
        {
            var token = CancellationToken.None;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(token);

                IDbCommand command = connection.CreateCommand();
                command.CommandText = "SELECT TOP(1) Name FROM Person;";
                var value = await command.ExecuteScalarAsync(token);
                Assert.AreEqual("Adam", value);
            }
        }


        [TestMethod]
        public async Task NoQuery_With_NoToken()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = "DECLARE @Name NVARCHAR(MAX) = 'Adam';";
                var value = await command.ExecuteNonQueryAsync();
                Assert.AreEqual(-1, value);
            }
        }

        [TestMethod]
        public async Task NoQuery_With_Token()
        {
            var token = CancellationToken.None;
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(token);

                IDbCommand command = connection.CreateCommand();
                command.CommandText = "DECLARE @Name NVARCHAR(MAX) = 'Adam';";
                var value = await command.ExecuteNonQueryAsync(token);
                Assert.AreEqual(-1, value);
            }
        }



        //[TestMethod]
        //public void PerfCompare()
        //{
        //    var maxConnections = 50;

        //    var direct = new List<SqlConnection>();
        //    for (var i = 0; i < maxConnections; i++)
        //    {
        //        direct.Add(new SqlConnection(connectionString));
        //    }

        //    var shim = new List<IDbConnection>();
        //    for (var i = 0; i < maxConnections; i++)
        //    {
        //        shim.Add(new SqlConnection(connectionString));
        //    }

        //    var directStopWatch = Stopwatch.StartNew();
        //    for (var i = 0; i < direct.Count; i++)
        //    {
        //        direct[i].OpenAsync().Wait();
        //    }
        //    directStopWatch.Stop();

        //    var shimStopWatch = Stopwatch.StartNew();
        //    for (var i = 0; i < shim.Count; i++)
        //    {
        //        shim[i].OpenAsync().Wait();
        //    }
        //    shimStopWatch.Stop();


        //    Console.WriteLine("Direct:{0}", directStopWatch.Elapsed);
        //    Console.WriteLine("Shim:{0}", shimStopWatch.Elapsed);
        //    Console.WriteLine("Diff (ms):{0}", shimStopWatch.ElapsedMilliseconds - directStopWatch.ElapsedMilliseconds);


        //    for (var i = 0; i < direct.Count; i++)
        //    {
        //        direct[i].Close();
        //    }
        //    directStopWatch.Stop();

        //    for (var i = 0; i < shim.Count; i++)
        //    {
        //        shim[i].Close();
        //    }
        //    shimStopWatch.Stop();


        //}
    }
}
