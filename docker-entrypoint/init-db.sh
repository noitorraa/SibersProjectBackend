#!/bin/bash

DB_PATH="/app/Database/sibersDB.db"

# If database does not exist, create it from script
if [ ! -f "$DB_PATH" ]; then
    echo "Database not found, creating from script..."
    sqlite3 "$DB_PATH" < /app/Database/script.sql
    echo "Database created successfully."
else
    echo "Database already exists."
fi

# Start the application
exec "$@"
