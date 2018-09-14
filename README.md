# NFive
[![License](https://img.shields.io/github/license/NFive/NFive.svg)](LICENSE)
[![Build Status](https://img.shields.io/appveyor/ci/NFive/nfive.svg)](https://ci.appveyor.com/project/NFive/nfive)
[![Release Version](https://img.shields.io/github/release/NFive/NFive/all.svg)](https://github.com/NFive/NFive/releases)
<a href="https://discord.gg/uNFzvHk"><img src="https://discordapp.com/api/guilds/392156382830264334/widget.png" alt="NFive's server"></a>	

Complete plugin framework for GTAV [FiveM](https://fivem.net/) servers built entirely in managed C#.
This project aims to replace existing FiveM server resources with a single managed framework to build upon.

On its own NFive provides no extra game mechanics or functionality, all extra features are introduced via plugins.

**Currently a work in progress**

This project is still subject to breaking changes at anytime, use at your own risk!

## Usage
It is strongly recommended that you use [NFive Package Manager](https://github.com/NFive/nfpm) to install and use NFive.

### Quick Start

1. Download [nfpm](https://github.com/NFive/nfpm) and place it in an empty directory.

2. Run `nfpm setup` and answer the prompts to automatically download and install a FiveM server and NFive.

3. Install the plugins you want to use on your server with `nfpm install <plugin>`, see `nfpm search` and the documentation for details.

4. Run `nfpm start` to boot the server and try connecting to `localhost` with your FiveM client.

## Development
Building the project will require [Visual Studio 2017](https://www.visualstudio.com/). A MySQL database is required for storage, [MariaDB](https://mariadb.org/) is recommended.

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

Note: For full Unicode support you will need to manually preconfigure your MySQL server's default character set. For MySQL/MariaDB add `--character-set-server=utf8mb4 --collation-server=utf8mb4_unicode_520_ci` to the server arguments before the database is created.

### Migrations
1. Drop the database.

2. Open the Package Manager Console in Visual Studio: `View > Other Windows > Package Manager Console`

3. Run following command with your database connection information:
    ```ps
    Add-Migration -Name Init -Force -ProjectName Server -ConnectionString "Host=db;Port=3306;Database=fivem;User Id=root;Password=password;CharSet=utf8mb4;SSL Mode=None" -ConnectionProviderName MySql.Data.MySqlClient
    ```
