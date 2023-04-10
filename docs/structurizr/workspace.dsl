workspace {

    model {
		logsTFApi = softwareSystem "LogsTF Public API" "Servers running the LogsTF plugin upload log data from competitive games here."

        tf2saGamer = person "TF2SA Gamer" "Player who would like to analyze their performance in TF2SA competitve games."

        tf2saStatsSystem = softwareSystem "TF2SA Stats System" "A player statistics platform for South African TF2 Games. Handles the ingestion and presentation." {
			db = container "Database" "MariaDB"

			tf2saWebApi = container "TF2SA Web API" "ASP.NET Core Web Application" {
				repositories = component "Repositories" "Entity-Framework driven data repositories" {
					this -> db "Queries" "SQL"
				}
				playersQueryHandlers = component "Players Query Handlers" "CQRS Query Handlers" {
					this -> repositories "Reads"
				}
				playersController = component "Players Controller"  "Returns player data." "ASP.NET API Controller" {
					this -> playersQueryHandlers "Queries" "CQRS"
				}
				statsQueryHandlers = component "Stats Query Handlers" "CQRS Query Handlers" {
					this -> repositories "Reads"
				}
				statsController = component "Stats Controller"  "Returns stats data." "ASP.NET API Controller" {
					this -> statsQueryHandlers "Queries" "CQRS"
				}
				statsCommandHandlers = component "Stats Command Handlers" "CQRS Command Handlers" {
					this -> repositories "Writes"
				}
				etlService = component "ETL Service" "Orchestrates the ingestion, ETL of game log data." "Background Service" {
					this -> logsTFApi "Fetches TF2 Log Data" "HTTPS/JSON"
					this -> statsCommandHandlers "Writes ingested TF2 Game data"
				}
			}

			frontend = container "Frontend Web Application" "Angular SPA" {
				playersService = component "Players Service" {
					this -> playersController "Makes API calls to" "HTTPS/JSON"
				}
				playersPage = component "Players Page" {
					this -> playersService "Queries"
					tf2saGamer -> this "Uses"
				}
				statsService = component "Stats Service" {
					this -> statsController "Makes API calls to" "HTTPS/JSON"
				}
				statsPage = component "Stats Page" {
					this -> statsService "Queries"
					tf2saGamer -> this "Uses"
				}
			}
        }
    }

    views {
        systemContext tf2saStatsSystem {
            include *
            autolayout lr
        }

        container tf2saStatsSystem {
            include *
            autolayout lr
        }

        component frontend {
            include *
            autolayout lr
        }

        component tf2saWebApi {
            include *
            autolayout lr
        }

        theme default
    }

}