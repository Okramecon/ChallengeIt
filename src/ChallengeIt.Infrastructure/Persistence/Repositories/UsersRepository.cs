﻿using System.Data;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.User;
using ChallengeIt.Infrastructure.Persistence.Dapper;
using Dapper;

namespace ChallengeIt.Infrastructure.Persistence.Repositories;

public class UsersRepository(ISqlDbContext context) : IUsersRepository
{
    private readonly IDbConnection _connection = context.CurrentConnection;

    private const string CreateUserQuery =
        """
        INSERT INTO users (username, email, password_hash, created_at, first_name, last_name, updated_at)
        VALUES (@UserName, @Email, @PasswordHash, @CreatedAt, @FirstName, @LastName, @UpdatedAt)
        RETURNING id
        """;

    private const string GetUserByIdQuery =
        """
        SELECT id, username, email, password_hash, first_name, last_name
        FROM users 
        WHERE id = @UserId;
        """;

    private const string GetUserByEmailQuery =
        """
        SELECT id, username, email, password_hash, first_name, last_name
        FROM users 
        WHERE Email = @Email;
        """;

    private const string GetUserByUserNameQuery =
        """
        SELECT id, username, email, password_hash, first_name, last_name
        FROM users 
        WHERE UserName = @Username;
        """;

    private const string GetByLoginQuery =
        """
        SELECT id, username, email, password_hash, first_name, last_name
        FROM users 
        WHERE username = @Username OR email = @Email;
        """;

    private const string CheckExistsByIdQuery =
        """
        SELECT COUNT(1) 
        FROM users 
        WHERE id = @Id;
        """;

    private const string CheckUsedEmailQuery =
        """
        SELECT COUNT(1) 
        FROM users 
        WHERE email = @Email;
        """;

    private const string CheckUsedUserNameQuery =
        """
        SELECT COUNT(1) 
        FROM users 
        WHERE username = @Username;
        """;

    private const string UpsertRefreshTokenQuery =
        """
        INSERT INTO refreshtokens (id, token, expires_at, user_id)
        VALUES (@Id, @Token, @ExpiresAt, @UserId)
        ON CONFLICT (id) 
        DO UPDATE SET 
            token = EXCLUDED.token,
            expires_at = EXCLUDED.expires_at;
        """;

    private const string GetRefreshTokenQuery =
        """
        SELECT token, expires_at, user_id, id
        FROM refreshtokens 
        WHERE token = @RefreshToken;
        """;

    private const string GetRefreshTokenByUserIdQuery =
        """
        SELECT token, expires_at, user_id, id
        FROM refreshtokens 
        WHERE user_id = @UserId;
        """;

    public async Task<long> AddAsync(User user, CancellationToken cancellationToken = default)
        => await _connection.QuerySingleAsync<long>(CreateUserQuery, user);

    public async Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default)
        => await _connection.QuerySingleOrDefaultAsync<User>(GetUserByIdQuery, new { UserId = userId });

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _connection.QuerySingleOrDefaultAsync<User>(GetUserByEmailQuery, new { Email = email });

    public async Task<User?> GetByUserNameAsync(string username, CancellationToken cancellationToken = default)
        => await _connection.QuerySingleOrDefaultAsync<User>(GetUserByUserNameQuery, new { Username = username });

    public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
        => await _connection.QuerySingleOrDefaultAsync<User>(GetByLoginQuery, new { Username = login, Email = login });

    public async Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken = default)
        => await _connection.ExecuteScalarAsync<bool>(CheckExistsByIdQuery, new { UserId = userId });

    public async Task<bool> IsUsedEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _connection.ExecuteScalarAsync<bool>(CheckUsedEmailQuery, new { Email = email });

    public async Task<bool> IsUsedUsernameAsync(string userName, CancellationToken cancellationToken = default)
        => await _connection.ExecuteScalarAsync<bool>(CheckUsedUserNameQuery, new { UserName = userName });

    public async Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
        => await _connection.ExecuteAsync(UpsertRefreshTokenQuery, new
        {
            id = token.Id,
            token.Token,
            token.ExpiresAt,
            token.UserId,
        });

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return await _connection.QuerySingleOrDefaultAsync<RefreshToken>(GetRefreshTokenQuery,
            new { RefreshToken = refreshToken });
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(long userId,
        CancellationToken cancellationToken = default)
    {
        return await _connection.QueryFirstOrDefaultAsync<RefreshToken>(GetRefreshTokenByUserIdQuery,
            new { UserId = userId });
    }

    public const string RemoveRefreshTokenQuery =
        """
        DELETE FROM refreshtokens
        WHERE id = @TokenId::uuid;
        """;

    public async Task RemoveRefreshTokenAsync(Guid refreshTokenId, CancellationToken _)
        => await _connection.ExecuteAsync(RemoveRefreshTokenQuery, new { TokenId = refreshTokenId });

    public const string RemoveRefreshTokenByUserIdQuery =
        """
        DELETE FROM refreshtokens
        WHERE user_id = @UserId;
        """;

    public async Task RemoveRefreshTokenAsync(long userId, CancellationToken _)
        => await _connection.ExecuteAsync(RemoveRefreshTokenByUserIdQuery, new { UserId = userId });

    public const string FindUserQuery =
        """
        SELECT id, username, first_name, last_name
        FROM users 
        WHERE Lower(username) LIKE '%' || Lower(@UserName) || '%';
        """
    ;

    public async Task<List<SearchUserProfileModel>> FindUsersByNameAsync(string userName,
        CancellationToken cancellationToken = default)
    {
        var profiles = await _connection.QueryAsync<SearchUserProfileModel>(FindUserQuery, new { UserName = userName });
        return [.. profiles];
    }
}
