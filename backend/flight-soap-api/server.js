const express = require('express');
const soap = require('soap');
const mysql = require('mysql2/promise');
const cors = require('cors');

const app = express();
app.use(cors());

const dbConfig = {
    host: 'localhost',
    user: 'root',
    password: '12345', // MySQL root şifrenizi buraya yazın
    database: 'flightdb',
    port: 3306 // MySQL port numaranız (genellikle 3306'dır)
};

app.get('/api/users', async (req, res) => {
    try {
        console.log('API isteği alındı');
        const connection = await mysql.createConnection(dbConfig);
        console.log('Veritabanına bağlanıldı');
        
        const [rows] = await connection.execute('SELECT * FROM adminuserview');
        console.log('Veritabanından gelen veriler:', rows);
        
        await connection.end();
        
        res.json(rows);
    } catch (error) {
        console.error('API Hatası:', error);
        res.status(500).json({ error: error.message });
    }
});

app.listen(3000, () => {
    console.log('Server running on port 3000');
    // Test veritabanı bağlantısı
    testDatabaseConnection();
});

// Veritabanı bağlantı testi
async function testDatabaseConnection() {
    try {
        const connection = await mysql.createConnection(dbConfig);
        console.log('Veritabanı bağlantısı başarılı');
        await connection.end();
    } catch (error) {
        console.error('Veritabanı bağlantı hatası:', error);
    }
}