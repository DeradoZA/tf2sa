import json, requests, mysql.connector, sys
import os
from steam.steamid import SteamID
from dotenv import dotenv_values
from schema import *
import datetime
import time

WINDOW = 6*60*60*24*30
MIN_MATCHES = 10

def get_Overall(cursor, low, count, MIN_MATCHES):
    res = cursor.execute("""
    SELECT p.SteamID, a.Matches, ROUND(b.Hours, 1) Hours, ROUND(IFNULL(c.Kills, 0), 2) AS 'Kills', ROUND(IFNULL(c.Deaths, 0),2) AS 'Deaths', ROUND(IFNULL(c.Assists, 0),2) AS 'Assists', ROUND(IFNULL(e.Backstabs, 0),1) AS 'Backstabs', ROUND(IFNULL(f.Headshots, 0),1) AS 'Headshots', ROUND(IFNULL(d.Airshots, 0),2) AS 'Airshots', ROUND(IFNULL(c.DPM, 0),2) AS 'DPM', ROUND(a.DTM, 2) DTM, ROUND(a.HRM ,2) HRM FROM Players p
    LEFT JOIN
    (
            SELECT p.SteamID, COUNT(p.GameID) AS 'Matches', SUM(p.DamageTaken) * 60 / SUM(g.Duration) AS 'DTM', SUM(p.HealsReceived) * 60 / SUM(g.Duration) AS 'HRM' FROM Games g
            JOIN PlayerStats p ON g.GameID = p.GameID
      WHERE Date > {low} AND Date < {high}
            GROUP BY p.SteamID
    ) AS a ON a.SteamID = p.SteamID
    LEFT JOIN
    (
            SELECT p.SteamID, SUM(c.Playtime) / 3600 AS 'Hours' FROM Games g
            JOIN PlayerStats p ON g.GameID = p.GameID
            JOIN ClassStats c ON c.PlayerStatsID = p.PlayerStatsID
      WHERE Date > {low} AND Date < {high}
      GROUP BY p.SteamID
    ) AS b ON b.SteamID = p.SteamID
    LEFT JOIN
    (
            SELECT p.SteamID, SUM(c.Kills) / Count(DISTINCT(g.GameID)) AS 'Kills', SUM(c.Deaths) / Count(DISTINCT(g.GameID)) AS 'Deaths', SUM(c.Assists) / Count(DISTINCT(g.GameID)) AS 'Assists', SUM(c.Damage) * 60 / SUM(c.Playtime) AS 'DPM' FROM Games g
            JOIN PlayerStats p ON g.GameID = p.GameID
            JOIN ClassStats c ON c.PlayerStatsID = p.PlayerStatsID
            WHERE c.ClassID != 7 AND Date > {low} AND Date < {high}
            GROUP BY p.SteamID
    ) AS c ON c.SteamID = p.SteamID
    LEFT JOIN
    (
            SELECT p.SteamID, Sum(Airshots) / Count(DISTINCT(g.GameID)) AS 'Airshots' FROM Games g
            JOIN PlayerStats p ON g.GameID = p.GameID
            JOIN ClassStats c ON c.PlayerStatsID = p.PlayerStatsID
            WHERE Date > {low} AND Date < {high} AND ClassID IN (2,4)
            GROUP BY p.SteamID
    ) AS d ON d.SteamID = p.SteamID
    LEFT JOIN
    (
            SELECT p.SteamID, Sum(Backstabs) / Count(g.GameID)  AS 'Backstabs' FROM Games g
            JOIN PlayerStats p ON g.GameID = p.GameID
            JOIN ClassStats c ON c.PlayerStatsID = p.PlayerStatsID
            WHERE ClassID = 9 AND Date > {low} AND Date < {high}
            GROUP BY p.SteamID
    ) AS e ON e.SteamID = p.SteamID
    LEFT JOIN
    (
            SELECT p.SteamID, Sum(Headshots) / Count(g.GameID)  AS 'Headshots' FROM Games g
            JOIN PlayerStats p ON g.GameID = p.GameID
            JOIN ClassStats c ON c.PlayerStatsID = p.PlayerStatsID
            WHERE ClassID = 8 AND Date > {low} AND Date < {high}
            GROUP BY p.SteamID
    ) AS f ON f.SteamID = p.SteamID
    HAVING Matches > {min_matches}
    ORDER BY DPM DESC
    """.format(low=low, high=count, min_matches=MIN_MATCHES))
    return res

def updateProgress(cursor, reset=False) :
    if reset == True:
        cursor.execute("DELETE FROM Progress")
    print("updating player progress...", end="")
    #set start and end dates
    cursor.execute("SELECT MIN(Date) FROM Games")
    unix_first  = int(cursor.fetchall()[0][0]) + 60*60*24*30;
    unix_now    = time.time()
    date_first  = datetime.datetime.utcfromtimestamp(unix_first)
    date_now    = datetime.datetime.utcfromtimestamp(unix_now)

    startyear   = date_first.year
    startmonth  = date_first.month + 1
    endyear     = date_now.year
    endmonth    = date_now.month
    dates = [time.mktime(datetime.date(m//12, m%12+1, 1).timetuple()) for m in range(startyear*12+startmonth-1, endyear*12+endmonth)]

    for date in dates:
        check = cursor.execute("SELECT COUNT(*) FROM Progress WHERE EndDate = {}".format(int(date)))
        check = cursor.fetchone()[0]
        if check == 0:
            print('...',datetime.datetime.utcfromtimestamp(date).strftime('%Y-%m-%d'), end="")
            low = date - WINDOW
            res = get_Overall(cursor, low, int(date), MIN_MATCHES)
            res = cursor.fetchall()
            for r in res:
                cursor.execute("""INSERT INTO Progress
                (EndDate, SteamID, Matches, Hours, Kills, Deaths, Assists, Backstabs, Headshots, Airshots, DPM, DTM, HRM)
                VALUES
                ({},{},{},{},{},{},{},{},{},{},{},{},{})
                """.format(int(date), r[0], r[1],r[2],r[3],r[4],r[5],r[6],r[7],r[8],r[9],r[10],r[11]))
    print("done")

if __name__ == "__main__":
    env = dotenv_values()
    db = mysql.connector.connect(
        host        = os.environ['TF2SA_MYSQL_HOST'],
        user        = os.environ['TF2SA_MYSQL_USR'],
        password    = os.environ['TF2SA_MYSQL_PWD'],
        database    = os.environ['TF2SA_MYSQL_DB'],
    )
    cursor = db.cursor(buffered=True)
    cursor.autocommit = True
    DBInit(cursor)

    updateProgress(cursor)

    db.commit()
