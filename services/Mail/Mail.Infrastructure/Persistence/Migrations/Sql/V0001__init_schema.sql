-- V0001__create_email_logs.sql
-- Create email_logs table for Mail service

CREATE TABLE IF NOT EXISTS email_logs
(
    id          BIGSERIAL PRIMARY KEY,

    purpose     VARCHAR(50)  NOT NULL,
    status      VARCHAR(20)  NOT NULL,

    to_email    VARCHAR(320) NOT NULL,

    created_at  TIMESTAMPTZ  NOT NULL,
    sent_at     TIMESTAMPTZ  NULL
);

-- Indexes for common queries
CREATE INDEX IF NOT EXISTS ix_email_logs_to_email
    ON email_logs (to_email);

CREATE INDEX IF NOT EXISTS ix_email_logs_status_created_at
    ON email_logs (status, created_at DESC);

CREATE INDEX IF NOT EXISTS ix_email_logs_purpose_created_at
    ON email_logs (purpose, created_at DESC);
