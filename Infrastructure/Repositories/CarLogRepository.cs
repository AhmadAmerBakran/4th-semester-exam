using Core.Interfaces;
using Core.Models;
using Dapper;
using Npgsql;

namespace Infrastructure.Repositories;

public class CarLogRepository : ICarLogRepository
{
        private readonly NpgsqlDataSource _dataSource;

        public CarLogRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string query = "SELECT * FROM car_log.users";
            using (var connection = _dataSource.CreateConnection())
            {
                return await connection.QueryAsync<User>(query);
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            const string query = @$"SELECT id as {nameof(User.Id)}, nickname as {nameof(User.NickName)} FROM car_log.users WHERE id = @Id";
            using (var connection = _dataSource.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
            }
        }

        public async Task<User> AddUserAsync(Guid userId, string nickname)
        {
            const string query = @$"
                INSERT INTO car_log.users (id, nickname) 
                VALUES (@UserId, @NickName) 
                RETURNING id as {nameof(User.Id)}, nickname as {nameof(User.NickName)}";
            using (var connection = _dataSource.CreateConnection())
            {
                return await connection.QueryFirstAsync<User>(query, new {UserId = userId, NickName = nickname});
            }
        }

        public async Task<IEnumerable<CarNotification>> GetNotificationsByUserIdAsync(int userId)
        {
            const string query = @"
                SELECT user_id, from_topic, to_topic, message, message_at 
                FROM car_log.car_notifications 
                WHERE user_id = @UserId";
            using (var connection = _dataSource.CreateConnection())
            {
                return await connection.QueryAsync<CarNotification>(query, new { UserId = userId });
            }
        }

        public async Task<CarNotification> AddNotificationAsync(Guid userId, string fromTopic, string toTopic, string message)
        {
            const string query = @"
                INSERT INTO car_log.car_notifications (user_id, from_topic, to_topic, message, message_at) 
                VALUES (@UserId, @FromTopic, @ToTopic, @Message, @MessageAt) 
                RETURNING *;";
            using (var connection = _dataSource.CreateConnection())
            {
                return await connection.QueryFirstAsync<CarNotification>(query, new
                {
                    UserId = userId,
                    FromTopic = fromTopic,
                    ToTopic = toTopic,
                    Message = message,
                    MessageAt = DateTime.Now
                });
            }
        }

        public async Task<IEnumerable<CarNotification>> GetCarLog()
        {
            const string query = @$"SELECT user_id as {nameof(CarNotification.UserId)}, from_topic as {nameof(CarNotification.FromTopic)},
         to_topic as {nameof(CarNotification.ToTopic)}, message as {nameof(CarNotification.Message)}, 
         message_at as {nameof(CarNotification.MessageAt)} FROM car_log.car_notifications ORDER BY message_at DESC LIMIT 10";
            using (var connection = _dataSource.CreateConnection())
            {
                return await connection.QueryAsync<CarNotification>(query);
            }
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            const string query = "DELETE FROM car_log.users WHERE id = @Id";
            using (var connection = _dataSource.CreateConnection())
            {
                return await connection.ExecuteAsync(query, new { Id = id });
            }
        }
}