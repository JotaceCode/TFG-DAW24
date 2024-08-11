docker-compose up -d MongoDBHotel SQLServerSecurity

sleep 20

docker-compose up -d hotel security gateway web

sleep 30

start https://localhost:4200
open https://localhost:4200
