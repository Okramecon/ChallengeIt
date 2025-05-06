ALTER TABLE checkins 
ADD time_zone varchar(100) NULL;

ALTER TABLE checkins
ADD failed_challenge boolean default false;