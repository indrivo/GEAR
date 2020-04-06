plink -ssh -pw Gear2020 -t nlupei@192.168.1.227 "sudo systemctl stop kestrel-savecrypto.service; sudo rm -r /usr/www/savecrypto; sudo mkdir /usr/www/savecrypto; chmod 777 /usr/www/savecrypto"
dotnet restore
dotnet build
rmdir /Q /S ./dist
dotnet publish -c Release -o ./dist
scp -r ./dist/ nlupei@192.168.1.227:/usr/www/savecrypto
color 2
echo Deploy with success
PAUSE
