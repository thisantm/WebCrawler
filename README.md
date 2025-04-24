# WebCrawler

## Overview

The WebCrawler project is designed to scrape data from web pages using Playwright and HtmlAgilityPack. It processes the data, saves snapshots, and stores execution logs in a PostgreSQL database.

## Prerequisites

- .NET 8.0 SDK
- A valid PostgreSQL database connection string for storing execution logs.

## Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/thisantm/WebCrawler.git
   cd WebCrawler
   ```

2. Create the database table:
   Execute the following SQL command in your PostgreSQL database to create the `CrawlerExecutionLogs` table:

   ```sql
   CREATE TABLE "CrawlerExecutionLogs" (
       "Id" BIGSERIAL PRIMARY KEY,
       "StartTime" TIMESTAMP NOT NULL,
       "EndTime" TIMESTAMP NOT NULL,
       "PageCount" INT NOT NULL,
       "TotalRows" INT NOT NULL,
       "JsonFilePath" VARCHAR(255) NOT NULL
   );
   ```

3. Configure environment variables:

   - Open the `Properties/launchSettings.json` file.
   - Update the `CONNECTION_STRING_CRAWLER_DB` environment variable with your database connection string:
     ```json
     "CONNECTION_STRING_CRAWLER_DB": "YourDatabaseConnectionStringHere"
     ```

4. Install dependencies:

   ```bash
   dotnet restore
   ```

5. Run the project:
   ```bash
   dotnet run
   ```

## Environment Variables

The following environment variables are required for the application to function correctly:

- `MAX_THREADS`: Maximum number of threads for concurrent crawling.
- `BASE_URL`: Base URL for the web pages to crawl.
- `HEADER_XPATH`: XPath for the table header.
- `ROWS_XPATH`: XPath for the table rows.
- `PAGINATION_XPATH`: XPath for pagination links.
- `JSON_FOLDER_PATH`: Path to save JSON files.
- `SNAPSHOT_FOLDER_PATH`: Path to save snapshots.
- `CONNECTION_STRING_CRAWLER_DB`: Connection string for the PostgreSQL database.

## Output

- JSON files are saved in the directory specified by `JSON_FOLDER_PATH`.
- Snapshots are saved in the directory specified by `SNAPSHOT_FOLDER_PATH`.

## Notes

Ensure that the PostgreSQL database connection string is valid and the database is accessible before running the application.
