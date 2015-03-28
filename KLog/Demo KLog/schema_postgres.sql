-- Schema used in the Demo of the DbLog w/ Postgres
-- Authors:
--	Josh Keegan 28/03/2015

CREATE TABLE demo
(
  "fieldA" text,
  "fieldB" text,
  id bigserial NOT NULL,
  CONSTRAINT "PK_demo" PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE demo
  OWNER TO "klogDemoUser";
