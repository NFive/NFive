# NFive
[![License](https://img.shields.io/github/license/NFive/NFive.svg)](LICENSE)
[![Build Status](https://img.shields.io/appveyor/ci/NFive/nfive.svg)](https://ci.appveyor.com/project/NFive/nfive)
[![Release Version](https://img.shields.io/github/release/NFive/NFive/all.svg)](https://github.com/NFive/NFive/releases)

Complete plugin framework for GTAV [FiveM](https://fivem.net/) servers built entirely in managed C#.
This project aims to replace existing FiveM server resources with a single managed framework to build upon.

**Currently a work in progress**

## Development
Building the project will require [Visual Studio 2017](https://www.visualstudio.com/). A MySQL database is required for storage, [MariaDB](https://mariadb.org/) is recommended.

This resource currently replaces *all* stock server resources; make sure you remove them from your configuration. The server will always try to load ``sessionmanager``, even if it is not in your configuration, so you must delete or rename the resource.

1. Clone this repo inside of your FiveM server's ``resources`` directory:
  ```sh
  git clone https://github.com/NFive/NFive.git nfive
  cd nfive
  ```

2. Build the project in Visual Studio.

3. Edit your ``server.cfg`` file to include the following line below your existing configuration:
  ```
  start nfive
  ```

4. Edit ``config\database.yml`` with your database connection information as needed.

Note: For full Unicode support you will need to manually preconfigure your MySQL server's default character set. For MySQL/MariaDB add ``--character-set-server=utf8mb4 --collation-server=utf8mb4_unicode_520_ci`` to the server arguments before the database is created.

### Migrations
1. Drop the database.

2. Open the Package Manager Console in Visual Studio: ``View > Other Windows > Package Manager Console``

3. Run following command with your database connection information:
  ```
  Add-Migration -Name Init -Force -ProjectName Server -ConnectionString "Host=db;Port=3306;Database=fivem;User Id=root;Password=password;CharSet=utf8mb4;SSL Mode=None" -ConnectionProviderName MySql.Data.MySqlClient
  ```
