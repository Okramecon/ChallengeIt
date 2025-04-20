ALTER TABLE CheckIns
ADD COLUMN time_zone VARCHAR(100) NULL DEFAULT NULL;

CREATE INDEX idx_checkins_time_zone ON CheckIns(time_zone);
