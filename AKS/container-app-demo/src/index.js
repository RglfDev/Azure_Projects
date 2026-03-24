 
const express = require('express');
const app = express();
const port = process.env.PORT || 3000;
 
app.get('/ping', (req, res) => {
   res.send('pong');
});
 
app.listen(port, () => {
   console.log(`Servidor corriendo en puerto ${port}`);
});
 