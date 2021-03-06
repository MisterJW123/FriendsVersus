using api.FriendsVersus.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using api.FriendsVersus.Dto;
using System.Threading.Tasks;

namespace FriendsVersusTests.Data_Tests
{
    [TestClass]
    public class UserDBTests
    {
        public const string connectionString = "Data Source=D:/TestDBEnvironment/FriendsVersus/FriendsVersus.db;";

        [TestMethod]
        public void TestAllWorksFine()
        {
            TestUserTableCanBeCreated();
            TestUsernameIndexCanBeCreated();
            TestUserCanBeCreated();
            TestUserCanBeGottenByUserId();
            TestUserCanBeGottenByUserName();
            TestCanPasswordBeGotByUserId();
            TestCanPasswordBeGotByUserName();
            TestUsernameCanBeUpdated();
            TestPasswordCanBeUpdated();
            TestCanUserEmailBeUpdated();
            TestUserCanBeDeleted();
        }

        [TestMethod]
        public void TestUserTableCanBeCreated()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(SchemaQueries.createUsersQuery, conn);
                command.ExecuteScalar();
                conn.Close();
                conn.Open();
                SqliteCommand command2 = new SqliteCommand(SchemaQueries.createUserVerificationLinkTableQuery, conn);
                command2.ExecuteScalar();
                conn.Close();
            }
        }

        [TestMethod]
        public void TestUsernameIndexCanBeCreated()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.createUsersUsernameIndex, conn);
                command.ExecuteScalar();
                conn.Close();
            }
        }

        [TestMethod]
        public async Task TestUserCanBeCreated()
        {
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.SetupGet(config => config.GetSection("connectionStrings")["AppData"])
                    .Returns("Data Source=D:/TestDBEnvironment/FriendsVersus/FriendsVersus.db");
            UserData data = new UserData(mockConfig.Object);
            await data.CreateUserAsync(new UserCreationRequest()
            {
                Username = "NewUser",
                Password = "SuperSpecialNumber1Password",
                Email = "SuperSpecialNumber1Email"
            }, new System.Threading.CancellationToken());
        }
        [TestMethod]
        public async Task TestUserCanBeGottenByUserId()
        {
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.SetupGet(config => config.GetSection("connectionStrings")["AppData"])
                    .Returns("Data Source=D:/TestDBEnvironment/FriendsVersus/FriendsVersus.db");
            UserData data = new UserData(mockConfig.Object);

            User user = await data.GetUserIfExists("NewUser");
        }
        [TestMethod]
        public void TestUserCanBeGottenByUserName()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.getUserByUsernameQuery, conn);
                command.Parameters.AddWithValue("$Username", "Test1");

                SqliteDataReader result = command.ExecuteReader();
                Assert.AreEqual(result.FieldCount, 4);

                conn.Close();
            }
        }
        [TestMethod]
        public void TestCanPasswordBeGotByUserId()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.getPasswordByUserIdQuery, conn);
                command.Parameters.AddWithValue("$UserId", 1);

                SqliteDataReader result = command.ExecuteReader();
                Assert.AreEqual(result.FieldCount, 1);

                conn.Close();
            }
        }
        [TestMethod]
        public void TestCanPasswordBeGotByUserName()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.getPasswordByUsernameQuery, conn);
                command.Parameters.AddWithValue("$Username", "NewUser");

                SqliteDataReader result = command.ExecuteReader();
                Assert.AreEqual(result.FieldCount, 1);

                conn.Close();
            }
        }
        [TestMethod]
        public void TestUsernameCanBeUpdated()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.updateUsernameQuery, conn);
                command.Parameters.AddWithValue("$Username", "Test6");
                command.Parameters.AddWithValue("$UserId", 1);

                command.ExecuteScalar();
                SqliteCommand command2 = new SqliteCommand(UserQueries.getUserByUsernameQuery, conn);
                command2.Parameters.AddWithValue("$Username", "Test6");

                SqliteDataReader result = command2.ExecuteReader();
                if (result.Read())
                {
                    Assert.AreEqual(result.GetString(1), "Test6");
                }
                conn.Close();
            }
        }
        [TestMethod]
        public void TestPasswordCanBeUpdated()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.updatePasswordQuery, conn);
                command.Parameters.AddWithValue("$Passwd", "Test7");
                command.Parameters.AddWithValue("$UserId", "0");

                command.ExecuteScalar();
                SqliteCommand command2 = new SqliteCommand(UserQueries.getPasswordByUserIdQuery, conn);
                command2.Parameters.AddWithValue("$UserId", "0");

                SqliteDataReader result = command2.ExecuteReader();
                if (result.Read())
                {
                    Assert.AreEqual(result.GetString(0), "Test7");
                }
                conn.Close();
            }
        }
        [TestMethod]
        public void TestCanUserEmailBeUpdated()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.updateEmailQuery, conn);
                command.Parameters.AddWithValue("$Email", "Test8");
                command.Parameters.AddWithValue("$UserId", 1);
                command.ExecuteScalar();
                SqliteCommand command2 = new SqliteCommand(UserQueries.getUserByUserIdQuery, conn);
                command2.Parameters.AddWithValue("$UserId", 1);

                SqliteDataReader results = command2.ExecuteReader();
                if (results.Read())
                {
                    Assert.AreEqual(results.GetString(2), "Test8");
                }

                conn.Close();
            }
        }

        [TestMethod]
        public void TestCanUserPrivilegesBeUpdated()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.updateUserIsAdminQuery, conn);
                command.Parameters.AddWithValue("$UserId", 1);

                command.ExecuteScalar();

                SqliteCommand command2 = new SqliteCommand(UserQueries.getUserByUserIdQuery, conn);
                command2.Parameters.AddWithValue("$UserId", 1);

                SqliteDataReader result = command2.ExecuteReader();

                result.Read();

                Assert.AreEqual(result.GetInt32(5), 1);
                conn.Close();
            }
        }

        [TestMethod]
        public void TestCanUserBannedBeUpdated()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.updateUserIsBannedQuery, conn);
                command.Parameters.AddWithValue("$UserId", 1);

                command.ExecuteScalar();

                SqliteCommand command2 = new SqliteCommand(UserQueries.getUserByUserIdQuery, conn);
                command2.Parameters.AddWithValue("$UserId", 1);

                SqliteDataReader result = command2.ExecuteReader();

                result.Read();

                Assert.AreEqual(result.GetInt32(4), 1);
                conn.Close();
            }
        }

        [TestMethod]
        public void TestUserCanBeDeleted()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                SqliteCommand command = new SqliteCommand(UserQueries.deleteUserQuery, conn);
                command.Parameters.AddWithValue("$UserId", 1);

                command.ExecuteScalar();
                conn.Close();
            }
        }

        [TestMethod]
        public async Task TestCanAllUsersBeGotten()
        {
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(config => config.GetSection("connectionStrings")["AppData"])
                .Returns("Data Source=D:/TestDBEnvironment/FriendsVersus/FriendsVersus.db");

            UserData userData = new UserData(mockConfig.Object);

            await userData.GetUsers(new System.Threading.CancellationToken());
        }
    }
}
