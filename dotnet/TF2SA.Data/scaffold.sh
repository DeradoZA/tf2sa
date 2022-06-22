#!/bin/sh
dotnet ef dbcontext scaffold \
    "Server=${TF2SA_MYSQL_HOST};User=${TF2SA_MYSQL_USR};Password=${TF2SA_MYSQL_PWD};Database=${TF2SA_MYSQL_DB}" \
    "Pomelo.EntityFrameworkCore.MySql" \
    --context TF2SADbContext \
    --output-dir Entities \
    --context-dir . \
    --json \
    --no-onconfiguring \
    --table BlacklistGames \
    --table ClassStats \
    --table Games \
    --table PlayerStats \
    --table Players \
    --table WeaponStats \
    --table Weapons