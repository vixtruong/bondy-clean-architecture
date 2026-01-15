-- V0001__create_email_outboxes.sql
-- Create email_outboxes table for Mail service

CREATE TABLE IF NOT EXISTS email_outboxes
(
    id               BIGSERIAL PRIMARY KEY,

    purpose          VARCHAR(50)  NOT NULL,
    status           VARCHAR(20)  NOT NULL,

    to_email         VARCHAR(320) NOT NULL,

    subject          VARCHAR(255) NOT NULL,
    html             TEXT         NULL,
    payload_json     TEXT         NOT NULL,

    dedup_key        VARCHAR(255) NOT NULL,

    attempt_count    INT          NOT NULL DEFAULT 0,
    last_attempt_at  TIMESTAMPTZ  NULL,

    created_at       TIMESTAMPTZ  NOT NULL,
    sent_at          TIMESTAMPTZ  NULL
    );

-- ========= Indexes =========

CREATE INDEX IF NOT EXISTS ix_email_outboxes_to_email
    ON email_outboxes (to_email);

CREATE INDEX IF NOT EXISTS ix_email_outboxes_status_created_at
    ON email_outboxes (status, created_at DESC);

CREATE INDEX IF NOT EXISTS ix_email_outboxes_purpose_created_at
    ON email_outboxes (purpose, created_at DESC);

-- Deduplication: only Pending & Sending
CREATE UNIQUE INDEX IF NOT EXISTS ux_email_outboxes_dedup_key_active
    ON email_outboxes (dedup_key)
    WHERE status IN ('Pending', 'Sending', 'Sent', 'Failed');
