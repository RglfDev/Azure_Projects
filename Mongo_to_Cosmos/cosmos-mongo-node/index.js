require('dotenv').config();
const { MongoClient } = require('mongodb');
 
const uri = process.env.MONGO_URI;
const client = new MongoClient(uri);
 
async function run() {
 
    try {
 
        
        await client.connect();
        console.log("✅ Conectado a CosmosDB Mongo");
 
        const db = client.db("mydatabase");
        const collection = db.collection("products");
 
        const partitionKeyValue = "electronics";
 
        // -----------------------------
        // ⏱ TTL INDEX
        // -----------------------------
        await collection.createIndex(
            { createdAt: 5 },
            { expireAfterSeconds: 3600 } // 1 hora
        );
 
        // -----------------------------
        // 🟢 CREATE
        // -----------------------------
        const insertResult = await collection.insertOne({
            name: "Tablet",
            price: 499.99,
            category: partitionKeyValue,
            createdAt: new Date()
        });
 
        console.log("🟢 Producto insertado con ID:", insertResult.insertedId);
 
        // -----------------------------
        // 🔵 READ
        // -----------------------------
        const products = await collection
            .find({ category: partitionKeyValue })
            .toArray();
 
        console.log("📄 Productos en categoría electronics:");
        console.log(products);
 
        // -----------------------------
        // 🟡 UPDATE
        // -----------------------------
        const updateResult = await collection.updateOne(
            { _id: insertResult.insertedId, category: partitionKeyValue },
            { $set: { price: 399.99 } }
        );
 
        console.log("🟡 Productos modificados:", updateResult.modifiedCount);
 
        // -----------------------------
        // 🔴 DELETE
        // -----------------------------
        const deleteResult = await collection.deleteOne({
            _id: insertResult.insertedId,
            category: partitionKeyValue
        });
 
        console.log("🔴 Productos eliminados:", deleteResult.deletedCount);
 
    }
    catch (err) {
 
        console.error("❌ Error:", err.message);
 
    }
    finally {
 
        await client.close();
        console.log("🔌 Conexión cerrada");
 
    }
}
 
run();