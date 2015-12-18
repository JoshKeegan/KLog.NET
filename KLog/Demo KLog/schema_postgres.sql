-- Schema used in the Demo of the DbLog w/ Postgres
-- Authors:
--	Josh Keegan 28/03/2015

CREATE TABLE demo
(
  id bigserial NOT NULL,
  message text NOT NULL,
  "logLevel" text NOT NULL,
  "callingMethodFullName" text NOT NULL,
  "eventDate" timestamp with time zone NOT NULL,
  CONSTRAINT "PK_demo" PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE demo
  OWNER TO "klogDemoUser";
