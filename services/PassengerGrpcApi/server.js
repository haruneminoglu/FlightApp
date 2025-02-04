const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const mysql = require('mysql2/promise');

// Proto dosyasını yükle
const PROTO_PATH = './protos/passenger.proto';
const packageDefinition = protoLoader.loadSync(PROTO_PATH, {
    keepCase: true,
    longs: String,
    enums: String,
    defaults: true,
    oneofs: true
});

const protoDescriptor = grpc.loadPackageDefinition(packageDefinition);
const passengerProto = protoDescriptor.passenger;

// MySQL bağlantı bilgileri
const dbConfig = {
    host: 'localhost',
    user: 'root',
    password: '12345',
    database: 'flightdb'
};

// Passenger servisini implement et
const getPassengers = async (call, callback) => {
    try {
        const connection = await mysql.createConnection(dbConfig);
        const [rows] = await connection.execute('SELECT * FROM adminpassengerview');
        await connection.end();

        const response = {
            passengers: rows.map(row => ({
                passengerId: row.PassengerId,
                gender: row.Gender,
                maskedTcNo: row.MaskedTcNo,
                passengerName: row.PassengerName,
                passengerSurname: row.PassengerSurname,
                dateOfBirth: row.DateOfBirth.toISOString()
            }))
        };
        callback(null, response);
    } catch (error) {
        callback({
            code: grpc.status.INTERNAL,
            details: 'Internal Server Error'
        });
    }
};

// gRPC sunucusunu başlat
function main() {
    const server = new grpc.Server();
    server.addService(passengerProto.PassengerService.service, { getPassengers });
    server.bindAsync('0.0.0.0:50051', grpc.ServerCredentials.createInsecure(), () => {
        server.start();
        console.log('gRPC Server running on port 50051');
    });
}

main();