IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Savr')
BEGIN
    CREATE DATABASE Savr;
END

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SavrIdentity')
BEGIN
    CREATE DATABASE SavrIdentity;
END