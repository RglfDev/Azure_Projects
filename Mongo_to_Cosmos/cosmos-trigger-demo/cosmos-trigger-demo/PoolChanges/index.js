const { MongoClient } = require("mongodb");

const uri = process.env.MONGO_URI;
const client = new MongoClient(uri);

// Guardamos el último timestamp fuera de la función para persistir mientras corre la función
let lastCheckTimestamp = Math.floor(Date.now() / 1000) - 60; // Iniciamos con 60 seg atrás

module.exports = async function (context, myTimer) {
    try {
        if (!client.topology?.isConnected()) {
            await client.connect();
        }

        const db = client.db("mydatabase");
        const collection = db.collection("products");

        let lastCheckDate = new Date(Date.now() - 60 * 1000); // inicializar hace 1 minuto

        // Consultamos documentos modificados desde la última ejecución

        const recentDocs = await collection.find({
            updatedAt: { $gte: lastCheckDate }
        }).toArray();

        if (recentDocs.length > 0) {
            context.log(`🟡 Documentos modificados recientemente (${recentDocs.length}):`);
            recentDocs.forEach(doc => context.log(JSON.stringify(doc)));
        } else {
            context.log("✅ Sin cambios recientes.");
        }

        // Actualizamos el timestamp para la próxima ejecución
        lastCheckTimestamp = Math.floor(Date.now() / 1000);

    } catch (err) {
        context.log.error("❌ Error en PollChanges:", err);
    }
};
