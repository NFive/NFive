# NFive

<img align="right" alt="NFive Logo" src="https://user-images.githubusercontent.com/43646/51492915-5558b200-1dab-11e9-9032-c79621407df7.png">

[![License](https://img.shields.io/github/license/NFive/NFive.svg)](LICENSE)
[![Discord](https://img.shields.io/discord/525451790876016651.svg)](https://discord.io/n5-htb)
[![CI Status](https://github.com/HTB-5M/NFive/actions/workflows/ci.yml/badge.svg)](https://github.com/HTB-5M/NFive/actions/workflows/ci.yml)
[![Releases](https://img.shields.io/github/release/NFive/NFive/all.svg)](https://github.com/HTB-5M/NFive/releases)

[NFive](https://nfive.io/) is a complete plugin framework for GTAV [FiveM](https://fivem.net/) servers built entirely in managed C#.
This project aims to replace existing FiveM server resources with a single managed framework to build upon.

On its own NFive provides no extra game mechanics or functionality, all extra features are introduced via plugins.

**Currently a work in progress**

This project is still subject to breaking changes at anytime, use at your own risk!

## Usage
It is strongly recommended that you use [NFive Package Manager](https://github.com/NFive/nfpm) (`nfpm`) to install and manage NFive.

> See the [NFive *Getting Started* documentation](https://nfive.io/docs/overview) for more information.

### Quick Start
1. [Download nfpm](https://dl.nfive.io/nfpm.exe) and place it in an empty directory.

2. Run `nfpm setup .` and answer the prompts to automatically download and install a FiveM server and NFive into the current directory.

3. Install the plugins you want to use on your server with `nfpm install <plugin>`, see `nfpm search` and the [NFive plugin hub](https://hub.nfive.io/) for available plugins.

4. Run `nfpm start` to boot the server and try connecting to `localhost` with your FiveM client.

    > See the [NFive *Database Setup* documentation](https://nfive.io/docs/database) for how to correctly configure the your MySQL server.

## Development
Building NFive will require [Visual Studio 2017](https://visualstudio.microsoft.com/). A MySQL database is required for storage, [MariaDB](https://mariadb.org/) is recommended.

This resource currently replaces *all* stock server resources; make sure you remove them from your configuration. The server will always try to load `sessionmanager`, even if it is not in your configuration, so you must delete or rename the resource folder.

1. Clone this repository inside of your FiveM server's `resources` directory:
    ```sh
    git clone https://github.com/NFive/NFive.git nfive
    cd nfive
    ```

2. Build the project in Visual Studio.

3. Edit your `server.cfg` file to include the following line below your existing configuration:
    ```cfg
    start nfive
    ```

4. Edit `config\database.yml` with your database connection information as needed.

Note: For full Unicode support you will need to manually preconfigure your MySQL server's default character set. For MySQL/MariaDB add `--character-set-server=utf8mb4 --collation-server=utf8mb4_unicode_520_ci` to the server arguments before the database is created. See the [documentation](https://nfive.io/docs/database) for details.

### Migrations
Its strongly recommended you use [`nfpm migrate`](https://nfive.io/docs/nfpm/command-reference) for automated migrations.

#### Manual
1. Drop the existing database.

2. Open the Package Manager Console in Visual Studio: `View > Other Windows > Package Manager Console`

3. Run following command with your database connection information:
    ```ps
    Add-Migration -Name Init -Force -ProjectName Server -ConnectionString "Host=db;Port=3306;Database=fivem;User Id=root;Password=password;CharSet=utf8mb4;SSL Mode=None" -ConnectionProviderName MySql.Data.MySqlClient
    ```
