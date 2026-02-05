CREATE TABLE users (
    id UUID PRIMARY KEY,
    username TEXT UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT now()
);

CREATE TABLE user_stats (
    user_id UUID PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
    wins INT NOT NULL DEFAULT 0
);

CREATE TABLE saves (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    title TEXT,
    data JSONB NOT NULL,
    created_at TIMESTAMP DEFAULT now()
);

CREATE INDEX idx_saves_user_id ON saves(user_id);
