from tkinter import W


def DBInit(cursor):
    print("confirming/recreating tables... ", end='')
    cursor.execute("SELECT DATABASE();")
    current_db = cursor.fetchone()[0]
    cursor.execute("""
        ALTER DATABASE {} 
        CHARACTER SET = utf8mb4 
        COLLATE = utf8mb4_unicode_ci;
    """.format(current_db))

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Users(
        UserID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        SteamID BIGINT UNSIGNED,
        UserName VARCHAR(32) NOT NULL,
        PassHash CHAR(64) NOT NULL,
        SessionID CHAR(64),
        JoinDate INT UNSIGNED,
        PRIMARY KEY(UserID, Username)
    ) ENGINE = InnoDB DEFAULT CHARSET=utf8mb4;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Threads (
        ThreadID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        UserID INT UNSIGNED NOT NULL,
        Topic VARCHAR(300),
        Date INT UNSIGNED,
        PRIMARY KEY (ThreadID),
        CONSTRAINT `fk_user_id`
            FOREIGN KEY (UserID) REFERENCES Users (UserID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    ) ENGINE = InnoDB DEFAULT CHARSET=utf8mb4;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Comments (
        CommentID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        ThreadID INT UNSIGNED NOT NULL,
        UserID INT UNSIGNED,
        Date INT UNSIGNED,
        Content TEXT(21000),
        PRIMARY KEY (CommentID),
        CONSTRAINT `fk_thread_id`
            FOREIGN KEY (ThreadID) REFERENCES Threads (ThreadID)
            ON DELETE CASCADE
            ON UPDATE CASCADE,
        CONSTRAINT `fk_user_comment_id`
            FOREIGN KEY (UserID) REFERENCES Users (UserID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    ) ENGINE = InnoDB DEFAULT CHARSET=utf8mb4;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Images (
        ImageID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        CommentID INT UNSIGNED NOT NULL,
        ImageHash CHAR(64) NOT NULL,
        PRIMARY KEY (ImageID),
        CONSTRAINT `fk_image_commentid`
            FOREIGN KEY (CommentID) REFERENCES Comments (CommentID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    ) ENGINE = InnoDB DEFAULT CHARSET=utf8mb4;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Polls (
        PollID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        CommentID INT UNSIGNED NOT NULL,
        Topic VARCHAR(300),
        PRIMARY KEY (PollID),
        CONSTRAINT `fk_poll_commentid`
            FOREIGN KEY (CommentID) REFERENCES Comments (CommentID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    ) ENGINE = InnoDB DEFAULT CHARSET=utf8mb4;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS PollOptions (
        PollOptionID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        PollID INT UNSIGNED NOT NULL,
        Option VARCHAR(300),
        PRIMARY KEY (PollOptionID),
        CONSTRAINT `fk_polloptions_pollid`
            FOREIGN KEY (PollID) REFERENCES Polls (PollID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    ) ENGINE = InnoDB DEFAULT CHARSET=utf8mb4;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS PollVotes (
        UserID INT UNSIGNED NOT NULL,
        PollID INT UNSIGNED NOT NULL,
        PollOptionID INT UNSIGNED NOT NULL,
        PRIMARY KEY (UserID, PollID),
        CONSTRAINT `fk_user_pollvotes`
            FOREIGN KEY (UserID) REFERENCES Users (UserID)
            ON DELETE CASCADE
            ON UPDATE CASCADE,
        CONSTRAINT `fk_poll_pollvotes`
            FOREIGN KEY (PollID) REFERENCES Polls (PollID)
            ON DELETE CASCADE
            ON UPDATE CASCADE,
        CONSTRAINT `fk_polloption_pollvotes`
            FOREIGN KEY (PollOptionID) REFERENCES PollOptions (PollOptionID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    ) ENGINE = InnoDB DEFAULT CHARSET=utf8mb4;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Games (
        GameID INT UNSIGNED NOT NULL,
        Date INT UNSIGNED,
        Duration SMALLINT,
        Map VARCHAR(32),
        BluScore TINYINT UNSIGNED,
        RedScore TINYINT UNSIGNED,
        PRIMARY KEY (GameID)
    ) ENGINE = InnoDB;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Players (
        SteamID BIGINT UNSIGNED NOT NULL,
        PlayerName VARCHAR(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
        PRIMARY KEY (SteamID)
    ) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS PlayerStats (
        PlayerStatsID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        GameID INT UNSIGNED NOT NULL,
        SteamID BIGINT UNSIGNED NOT NULL,
        TeamID TINYINT UNSIGNED NOT NULL,
        DamageTaken MEDIUMINT UNSIGNED,
        HealsReceived MEDIUMINT UNSIGNED,
        MedkitsHP MEDIUMINT UNSIGNED,
        Airshots TINYINT UNSIGNED,
        Headshots TINYINT UNSIGNED,
        Backstabs TINYINT UNSIGNED,
        Drops TINYINT UNSIGNED,
        Heals MEDIUMINT UNSIGNED,
        Ubers TINYINT UNSIGNED,
        PRIMARY KEY (PlayerStatsID),
        CONSTRAINT `fk_game_id`
            FOREIGN KEY (GameID) REFERENCES Games (GameID)
            ON DELETE CASCADE
            ON UPDATE CASCADE,
        CONSTRAINT `fk_player_id`
            FOREIGN KEY (SteamID) REFERENCES Players (SteamID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    ) ENGINE = InnoDB;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS ClassStats (
        ClassStatsID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        PlayerStatsID INT UNSIGNED NOT NULL,
        ClassID TINYINT UNSIGNED NOT NULL,
        Playtime SMALLINT UNSIGNED,
        Kills TINYINT UNSIGNED,
        Assists TINYINT UNSIGNED,
        Deaths TINYINT UNSIGNED,
        Damage MEDIUMINT UNSIGNED,
        PRIMARY KEY (ClassStatsID),
        CONSTRAINT `fk_playerstats_id`
            FOREIGN KEY (PlayerStatsID) REFERENCES PlayerStats (PlayerStatsID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    ) ENGINE = InnoDB;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Weapons (
        WeaponID SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
        WeaponName VARCHAR(32),
        PRIMARY KEY (WeaponID)
    ) ENGINE = InnoDB;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS WeaponStats (
        PlayerStatsID INT UNSIGNED NOT NULL,
        WeaponID SMALLINT UNSIGNED NOT NULL,
        Accuracy DOUBLE,
        PRIMARY KEY (PlayerStatsID, WeaponID),
        CONSTRAINT `fk_player_stats`
            FOREIGN KEY (PlayerStatsID) REFERENCES PlayerStats (PlayerStatsID)
            ON DELETE CASCADE
            ON UPDATE RESTRICT,
        CONSTRAINT `fk_weapon_id`
            FOREIGN KEY (WeaponID) REFERENCES Weapons (WeaponID)
            ON DELETE CASCADE
            ON UPDATE RESTRICT
    ) ENGINE = InnoDB;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS BlacklistGames (
        GameID INT UNSIGNED NOT NULL,
        Reason VARCHAR(32),
        PRIMARY KEY (GameID)
    ) ENGINE = InnoDB;
    """)

    cursor.execute("""
    CREATE TABLE IF NOT EXISTS Progress(
        ProgressID INT UNSIGNED NOT NULL AUTO_INCREMENT,
        EndDate INT UNSIGNED,
        SteamID BIGINT UNSIGNED,
        Matches INT,
        Hours DOUBLE,
        Kills DOUBLE,
        Deaths DOUBLE,
        Assists DOUBLE,
        Backstabs DOUBLE,
        Headshots DOUBLE,
        Airshots DOUBLE,
        DPM DOUBLE,
        DTM DOUBLE,
        HRM DOUBLE,
        PRIMARY KEY (ProgressID),
        CONSTRAINT `fk_progress_steamid`
            FOREIGN KEY (SteamID) REFERENCES Players (SteamID)
            ON DELETE CASCADE
            ON UPDATE CASCADE
    )ENGINE = InnoDB DEFAULT CHARSET=utf8mb4;
    """)
    print("done")

    return
