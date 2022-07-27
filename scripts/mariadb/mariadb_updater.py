#update site database
#Usage:
#mysql_upater -> create db if not present and update all games
#mysql_updater 50 -> limit number of logs updated to 50
#mysql_updater 50 stat/forum->now also wipe stats/forum tables and recreate(MUST SUPPLY NUM)
#NB MUST SUPPLY NUMBER IN LAST EXAMPLE.
import json, requests, mysql.connector, sys
import os
from progress_updater import *
from schema import *
from steam.steamid import SteamID
from dotenv import dotenv_values
#UPDATE GAME STATS FROM LOGSTF
TEAM_ID = {
    "Blue"  : 0,
    "Red"   : 1
}

CLASS_ID = {
    "special"       : 0, #Tracks overall stats that can't be attributed to one class
    "scout"         : 1,
    "soldier"       : 2,
    "pyro"          : 3,
    "demoman"       : 4,
    "heavyweapons"  : 5,
    "engineer"      : 6,
    "medic"         : 7,
    "sniper"        : 8,
    "spy"           : 9
}

#THRESHOLDS FOR IGNORING GAMES
MIN_GAME_LENGTH = 15
DMG_THRESH = 17500
HPM_THRESH = 1300

def UpdatePlayerNames(cursor):
    Steam_API_Key = os.environ["TF2SA_STEAM_API_KEY"]
    cursor.execute("SELECT SteamID from Players")
    SteamIDTuple = cursor.fetchall()
    IDList = []

    print("Updating player steam usernames.")

    for ID in range(0, len(SteamIDTuple)):
        IDList.append(SteamIDTuple[ID][0])

        if len(IDList) == 100 or ID == len(SteamIDTuple):
            PlayerInfo = json.loads(requests.get("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={}&steamids={}".format(Steam_API_Key, IDList)).text)

            UpdatedPlayerNames = {}

            for Player in PlayerInfo["response"]["players"]:
                UpdatedPlayerNames[Player["steamid"]] = Player["personaname"]

            for SteamID, PlayerName in UpdatedPlayerNames.items():
                qry = "UPDATE Players SET PlayerName = %s WHERE SteamID = %s"
                cursor.execute(qry,(PlayerName, SteamID))

            IDList = []
            UpdatedPlayerNames = {}

    print("done")

#deletes game, adds to blacklist
def BlackListLog(LogID, reason, cursor):
    cursor.execute("INSERT INTO BlacklistGames (GameID, Reason) VALUES({},'{}')".format(LogID, reason))
    cursor.execute("DELETE FROM Games WHERE GameID = {}".format(LogID))
    print(reason, '. game blacklisted')

#Add Player if not added.
def AddPlayer(steamid, name, cursor):
    cursor.execute("Select SteamID from Players where SteamID = {}".format(steamid))
    PlayerExists = cursor.fetchone()
    if PlayerExists:
        return
    else:
        qry = "INSERT INTO Players VALUES (%s, %s)"
        cursor.execute(qry,(steamid, name))

def AddGame(LogID, Log, cursor):
    global MIN_GAME_LENGTH
    if (Log["length"] / 60) < MIN_GAME_LENGTH:
        BlackListLog(LogID,"match too short", cursor)
        return

    #Add Game Record
    cursor.execute("""
    INSERT INTO Games
    VALUES ({},{},{},'{}',{},{})
    """.format(LogID, Log["info"]["date"], Log["info"]["total_length"], Log["info"]["map"], Log["teams"]["Blue"]["score"], Log["teams"]["Red"]["score"]))

    #Update Players
    for PlayerID, Name in Log["names"].items():
        AddPlayer(SteamID(PlayerID).as_64, Name, cursor)

    global CLASS_ID
    global TEAM_ID
    global DMG_THRESH
    global HPM_THRESH
    for Player,PlayerStats in Log["players"].items():
        #PLAYERSTATS
        PlayerID        = SteamID(Player).as_64
        TeamID          = TEAM_ID[PlayerStats["team"]]
        DamageTaken     = PlayerStats["dt"]
        HealsReceived   = PlayerStats["hr"]
        MedkitsHP       = PlayerStats["medkits_hp"]
        Airshots        = PlayerStats["as"]
        Headshots       = PlayerStats["headshots_hit"]
        Backstabs       = PlayerStats["backstabs"]
        Drops           = PlayerStats["drops"]
        Heals           = PlayerStats["heal"]
        Ubers           = PlayerStats["ubers"]
        Damage          = PlayerStats["dmg"] #stored here for validation

        #1st validity check
        if Damage > DMG_THRESH:
            BlackListLog(LogID,"damage thresh exceeded", cursor)
            return

        #Add PlayerStats record
        cursor.execute("""INSERT INTO PlayerStats
        (GameID, SteamID, TeamID, DamageTaken, HealsReceived, MedkitsHP, Airshots, Headshots, Backstabs, Drops, Heals, Ubers)
        VALUES ({},{},{},{},{},{},{},{},{},{},{},{})""".format(
        LogID,PlayerID,TeamID,DamageTaken,HealsReceived,MedkitsHP,Airshots,Headshots,Backstabs,Drops,Heals,Ubers))

        #Fetch PlayerStatsID
        cursor.execute("""SELECT PlayerStatsID FROM PlayerStats
        WHERE GameID = {} AND SteamID = {}""".format(LogID,PlayerID))
        PlayerStatsID = cursor.fetchone()[0]

        #Add class stats for player
        for Class_stats in PlayerStats["class_stats"]:
            if Class_stats["type"] not in CLASS_ID:
                continue

            Class       = CLASS_ID[Class_stats["type"]]
            Kills       = Class_stats["kills"]
            Assists     = Class_stats["assists"]
            Deaths      = Class_stats["deaths"]
            Damage      = Class_stats["dmg"]
            Duration    = Class_stats["total_time"]

            if Duration < 0:
                continue

            if (Class == 7) and (Duration > 0) and ((Heals/Duration)*60 > HPM_THRESH):
                BlackListLog(LogID,"heals thresh exceeded", cursor)
                return

            #Add record
            cursor.execute("""INSERT INTO ClassStats
            (PlayerStatsID, ClassID, Playtime, Kills, Assists, Deaths, Damage)
            VALUES ({},{},{},{},{},{},{})""".format(
            PlayerStatsID, Class,Duration,Kills,Assists,Deaths,Damage))

    print("done")

def deSmurf(cursors, originals, smurfs):
    print('clearing smurfs...', end="")
    for i, _ in enumerate(originals):
        cursor.execute("UPDATE PlayerStats SET SteamID={} WHERE SteamID={}".format(originals[i],smurfs[i]))
    print('done')
    return

if __name__ == "__main__":
    env = dotenv_values()
    db = mysql.connector.connect(
        host        = os.environ['TF2SA_MYSQL_HOST'],
        user        = os.environ['TF2SA_MYSQL_USR'],
        password    = os.environ['TF2SA_MYSQL_PWD'],
        database    = os.environ['TF2SA_MYSQL_DB'],
        charset     = 'utf8mb4',
        collation   = 'utf8mb4_unicode_ci'
    )
    cursor = db.cursor()
    cursor.autocommit = True
    #Fetch command line args
    LIMIT = -1
    if len(sys.argv) > 1:
        LIMIT = int(sys.argv[1])
        if "stat" in sys.argv:
            cursor.execute("SET FOREIGN_KEY_CHECKS = 0")
            cursor.execute("DROP TABLE IF EXISTS WeaponStats, Weapons, ClassStats, PlayerStats, Players, Games, BlacklistGames;")
            cursor.execute("SET FOREIGN_KEY_CHECKS = 1")
        if "forum" in sys.argv:
            cursor.execute("SET FOREIGN_KEY_CHECKS = 0")
            cursor.execute("DROP TABLE IF EXISTS PollVotes, PollOptions, Polls, Images, Comments, Threads, Users")
            cursor.execute("SET FOREIGN_KEY_CHECKS = 1")

    DBInit(cursor)

    UpdatePlayerNames(cursor)

    #get blacklisted logs
    cursor.execute("SELECT GameID FROM BlacklistGames")
    BlackListedLogs = [i[0] for i in list(cursor.fetchall())]
    print(str(len(BlackListedLogs)) + " blacklisted matches have been recorded.")

    #get already stored logs
    cursor.execute("SELECT GameID FROM Games")
    StoredLogs = [i[0] for i in list(cursor.fetchall())]
    print(len(StoredLogs), " matches already stored in database.\nFetching Logs...", end="")

    UPLOADERS = env['LOG_UPLOADERS'].split(',')
    AllLogs = []
    #Generate list of LogIDs
    UpdatePlayerNames(cursor)
    for x in UPLOADERS:
        UploaderLogs = json.loads(requests.get("https://logs.tf/api/v1/log?uploader={}".format(x)).text)
        for x in UploaderLogs["logs"]:
            AllLogs.append(x["id"])

    print(len(AllLogs), "Total logs")

    LogsList = set(set(AllLogs).difference(StoredLogs)).difference(BlackListedLogs)
    print(len(LogsList),  "logs to be processed")

    #Add each Log record
    for idx, LogID in enumerate(LogsList, start=1):
        print("Updating log (" + str(idx) + "/" + str(len(LogsList)) + ") - " + str(LogID) + '... ', end='')
        Log = json.loads(requests.get("http://logs.tf/api/v1/log/{}".format(LogID)).text)
        AddGame(LogID, Log, cursor)
        db.commit()
        if idx == LIMIT:
            break

    deSmurf(cursor, env['ORIGINALS'].split(','), env['SMURFS'].split(','))
    updateProgress(cursor)

    UpdatePlayerNames(cursor)
    db.commit()
    db.close()
