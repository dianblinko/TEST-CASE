BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Driver" (
	"Id"	INTEGER,
	"DateTime"	TEXT,
	"Name"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Car" (
	"Id"	INTEGER,
	"DateTime"	TEXT,
	"Name"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
COMMIT;