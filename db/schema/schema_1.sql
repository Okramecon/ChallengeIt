ALTER TABLE Challenges
ADD COLUMN minimal_activity_timer INT NOT NULL DEFAULT 0,
ADD COLUMN minimal_activity_description VARCHAR(280) DEFAULT '' ,
ADD COLUMN theme_code INT NOT NULL DEFAULT 0,
ADD COLUMN exp INT NOT NULL DEFAULT 0,
ADD COLUMN anti_exp INT NOT NULL DEFAULT 0,
ADD COLUMN goal VARCHAR(120) NOT NULL DEFAULT '',
ADD COLUMN motivation VARCHAR(255) DEFAULT '',
ADD COLUMN is_private BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE CheckIns
ADD COLUMN notes VARCHAR(255) NULL DEFAULT NULL,
ADD COLUMN is_last BOOLEAN NOT NULL DEFAULT FALSE;
