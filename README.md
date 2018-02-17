# Climb
## Setup
### Visual Studio
#### Import Config
* Download from Google Drive
* Move to ClimbSource/Climb/…
#### Setup SSL
* Open Climb project settings
* Go to Debug
* Check Enable SSL
* Copy url and paste into App URL above
* Save
#### Setup local DB
* Open View/Other Windows/Package Manager Console
* Run “Update-Database”
#### Run without Debug
* Shift F5
* Add exception for this url
### Different environments
* Development
  * Local DB
  * Local CDN
* MemoryDB
  * In memory DB
  * Local CDN
### Azure Functions
* Download [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409)
* Run Azure Storage Emulator
* Add RunOnStartup to TimerTrigger attribute if you want to test function without waiting for timer
* Run Climb.Jobs project in VS