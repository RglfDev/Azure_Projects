# Azure Function Demo — HelloFunction

Función HTTP sencilla en C# (.NET 6) lista para publicar en Azure.

---

## ¿Qué hace?

Responde a peticiones GET y POST en `/api/hello` con un JSON de saludo.

| Petición | Respuesta |
|---|---|
| `GET /api/hello` | Saludo genérico |
| `GET /api/hello?name=Maria` | `¡Hola, Maria!` |
| `POST /api/hello` body `{"name":"Maria"}` | `¡Hola, Maria!` |

Ejemplo de respuesta:
```json
{
  "message": "¡Hola, Maria! Bienvenido/a a Azure Functions 🎉",
  "timestamp": "2024-01-15T10:30:00Z",
  "status": "ok"
}
```

---

## Ejecutar en local

### Requisitos
- [.NET 6 SDK](https://dotnet.microsoft.com/download)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local)

### Pasos
```bash
cd AzureFunctionDemo
func start
```
La función estará en: `http://localhost:7071/api/hello`

---

## Publicar en Azure

### Opción A — Visual Studio
1. Clic derecho en el proyecto → **Publish**
2. Selecciona **Azure** → **Azure Function App (Windows)**
3. Crea o selecciona una Function App existente
4. Clic en **Publish**

### Opción B — Azure CLI
```bash
# 1. Login
az login

# 2. Crear grupo de recursos (si no tienes)
az group create --name MiGrupo --location eastus

# 3. Crear storage account (requerido)
az storage account create --name mistoragelobo --location eastus --resource-group MiGrupo --sku Standard_LRS

# 4. Crear la Function App
az functionapp create \
  --resource-group MiGrupo \
  --consumption-plan-location eastus \
  --runtime dotnet \
  --functions-version 4 \
  --name MiFunctionAppLobo \
  --storage-account mistoragelobo

# 5. Publicar
dotnet publish -c Release
func azure functionapp publish MiFunctionAppLobo
```

### Opción C — VS Code
1. Instala la extensión **Azure Functions**
2. Panel Azure → Function Apps → Deploy to Function App

---

## URL final en Azure
```
https://<tu-function-app>.azurewebsites.net/api/hello?name=TuNombre
```
