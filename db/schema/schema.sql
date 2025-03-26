-- Users Table
CREATE TABLE Users (
    id BIGSERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    is_external BOOLEAN NOT NULL DEFAULT FALSE,
    password_hash VARCHAR(100) NULL,
    first_name VARCHAR(50) NULL,
    last_name VARCHAR(50) NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamptz NULL
);

CREATE INDEX idx_users_email ON Users(email);
CREATE INDEX idx_users_username ON Users(username);

CREATE TABLE RefreshTokens (
    id UUID PRIMARY KEY,
    user_id BIGSERIAL NOT NULL REFERENCES Users(id),
    token VARCHAR(50) NOT NULL,
    expires_at timestamptz NOT NULL
);

CREATE INDEX idx_refresh_tokens_token ON RefreshTokens(token);

-- Challenges Table
CREATE TABLE Challenges (
    id UUID PRIMARY KEY,
    user_id BIGSERIAL NOT NULL REFERENCES Users(id),
    title VARCHAR(255) NOT NULL,
    bet_amount DECIMAL(10, 2) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    missed_days_count INT DEFAULT 0 NOT NULL,
    max_allowed_missed_days INT NOT NULL,
    status VARCHAR(50) NOT NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL
);

-- CheckIns Table
CREATE TABLE CheckIns (
    id UUID PRIMARY KEY,
    user_id BIGSERIAL NOT NULL REFERENCES Users(id),
    challenge_id UUID NOT NULL REFERENCES Challenges(id),
    date DATE NOT NULL,
    checked BOOLEAN DEFAULT FALSE NOT NULL,
    CONSTRAINT unique_checkin UNIQUE (user_id, challenge_id, date)
);

CREATE INDEX idx_checkins_date ON CheckIns(date);

