# 🚀 Guía de Ejecución Local - TerraNova API

Guía paso a paso para ejecutar y probar la TerraNova API en tu máquina local.

---

## 📋 Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

1. **.NET SDK 9.0** o superior
   - Descarga desde: https://dotnet.microsoft.com/download
   - Verifica la instalación ejecutando: `dotnet --version`

2. **MongoDB** (versión 4.4 o superior)
   - Opción 1: MongoDB Community Server (local)
     - Descarga desde: https://www.mongodb.com/try/download/community
   - Opción 2: MongoDB Atlas (cloud - gratuito)
     - Crea cuenta en: https://www.mongodb.com/cloud/atlas

---

## 🔧 Paso 1: Verificar Instalaciones

Abre una terminal (PowerShell, CMD, o Git Bash) y verifica que tienes todo instalado:

```bash
# Verificar .NET SDK
dotnet --version

# Verificar MongoDB (si está instalado localmente)
mongod --version
```

---

## 🗄️ Paso 2: Configurar MongoDB

### Opción A: MongoDB Local

**Windows:**
1. Descarga e instala MongoDB Community Server
2. Inicia el servicio MongoDB desde "Services" (busca "MongoDB")
3. O ejecuta manualmente: `mongod`

**Verificar que MongoDB está corriendo:**
```bash
# En otra terminal
mongosh
# O si usas versión antigua:
mongo
```

Si te conecta, MongoDB está funcionando correctamente.


### Opción B: MongoDB Atlas (Cloud)

> **📖 Guía Completa:** Consulta la [Guía de Conexión a MongoDB Atlas](./CONFIGURAR_MONGODB_ATLAS.md) para instrucciones detalladas.

**Resumen rápido:**

1. Obtén las credenciales de conexión del administrador del proyecto
2. Configura la cadena de conexión en `appsettings.json` o usa User Secrets
3. Ejecuta la aplicación y verifica la conexión

**Ejemplo de configuración:**
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority"
  },
  "MongoDB": {
    "DatabaseName": "PropertyDB"
  }
}
```

> **⚠️ Importante:** No subas credenciales reales a Git. Usa User Secrets o variables de entorno.

---

## ⚙️ Paso 3: Configurar la Aplicación

### Verificar Configuración

Abre el archivo `src/PropertyAPI/appsettings.json` y verifica la configuración:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "MongoDB": {
    "DatabaseName": "PropertyDB"
  }
}
```

**Si usas MongoDB Atlas**, actualiza la cadena de conexión con la que te proporcionó Atlas.

---

## 🏃 Paso 4: Ejecutar la API

### Método 1: Desde la Terminal (Recomendado)

1. **Navega al directorio del proyecto:**
```bash
cd "C:\Ruta en donde clonaste el proyecto\src\PropertyAPI"
```

2. **Restaura las dependencias:**
```bash
dotnet restore
```

3. **Compila el proyecto:**
```bash
dotnet build
```

4. **Ejecuta la aplicación:**
```bash
dotnet run
```

O si prefieres ejecutar en modo watch (recarga automática al cambiar código):
```bash
dotnet watch run
```

**¡Listo!** La API debería estar corriendo. Verás un mensaje similar a:
```
Now listening on: http://localhost:5000
```

> **Nota**: En desarrollo, solo se usa HTTP. HTTPS está deshabilitado para evitar problemas con CORS.

### Método 2: Desde Visual Studio

1. Abre el archivo `src/PropertyAPI/PropertyAPI.csproj` en Visual Studio
2. Presiona `F5` o haz clic en "Run"
3. La API se iniciará automáticamente

### Método 3: Desde VS Code

1. Abre la carpeta del proyecto en VS Code
2. Abre la terminal integrada (Ctrl + `)
3. Ejecuta: `dotnet run` desde `src/PropertyAPI`

---

## 📊 Paso 5: Poblar la Base de Datos con Datos de Ejemplo

Una vez que la API esté corriendo, necesitas poblar la base de datos con datos de prueba.

### Opción 1: Usando Swagger UI (Más Fácil)

1. Abre tu navegador en: `http://localhost:5000`
2. Verás la interfaz de Swagger
3. Busca el endpoint `POST /api/seed`
4. Haz clic en "Try it out"
5. Haz clic en "Execute"
6. Deberías ver una respuesta exitosa con un resumen de datos insertados

**Opciones disponibles:**
- `POST /api/seed` - Poblar base de datos (solo si está vacía)
- `POST /api/seed?repopulate=true` - Repoblar (elimina y vuelve a poblar)
- `POST /api/seed/add-more?count=100` - Agregar más propiedades sin eliminar existentes

### Opción 2: Usando cURL

Abre una nueva terminal y ejecuta:

```bash
# Poblar base de datos (solo si está vacía)
curl -X POST http://localhost:5000/api/seed

# Repoblar (elimina y vuelve a poblar)
curl -X POST "http://localhost:5000/api/seed?repopulate=true"

# Agregar más propiedades
curl -X POST "http://localhost:5000/api/seed/add-more?count=50"
```

### Opción 3: Usando PowerShell

```powershell
# Poblar base de datos
Invoke-RestMethod -Uri "http://localhost:5000/api/seed" -Method Post

# Repoblar
Invoke-RestMethod -Uri "http://localhost:5000/api/seed?repopulate=true" -Method Post

# Agregar más propiedades
Invoke-RestMethod -Uri "http://localhost:5000/api/seed/add-more?count=50" -Method Post
```

### Opción 4: Usando Postman

1. Crea una nueva request
2. Método: `POST`
3. URL: `http://localhost:5000/api/seed`
4. Haz clic en "Send"

**Respuesta esperada:**
```json
{
  "message": "Base de datos poblada exitosamente con datos de ejemplo",
  "summary": {
    "properties": 9,
    "owners": 9,
    "images": 15,
    "traces": 18
  }
}
```

> **Nota**: Si la base de datos ya contiene datos y no usas `repopulate=true`, el seed no se ejecutará y retornará un mensaje indicando que se omitió.

---

## 🧪 Paso 6: Probar los Endpoints

### Usando Swagger UI (Recomendado)

1. Abre: `http://localhost:5000`
2. Verás todos los endpoints disponibles
3. Prueba cada endpoint haciendo clic en "Try it out" y luego "Execute"
4. Nota: Las búsquedas por `name` y `address` ignoran acentos (ej: "medellin" encuentra "Medellín")

### Usando cURL

#### Obtener todas las propiedades
```bash
curl http://localhost:5000/api/properties
```

#### Filtrar por nombre (sin acentos)
```bash
curl "http://localhost:5000/api/properties?name=medellin"
```

#### Filtrar por dirección (sin acentos)
```bash
curl "http://localhost:5000/api/properties?address=bogota"
```

#### Filtrar por rango de precio
```bash
curl "http://localhost:5000/api/properties?minPrice=200000000&maxPrice=1000000000"
```

#### Obtener propiedad por ID
```bash
# Primero obtén un ID de la lista anterior, luego:
curl "http://localhost:5000/api/properties/{id}"
```

### Usando PowerShell

```powershell
# Obtener todas las propiedades
Invoke-RestMethod -Uri "http://localhost:5000/api/properties" -Method Get

# Filtrar por nombre (sin acentos)
$uri = "http://localhost:5000/api/properties?name=medellin"
Invoke-RestMethod -Uri $uri -Method Get

# Filtrar por dirección (sin acentos)
$uri = "http://localhost:5000/api/properties?address=bogota"
Invoke-RestMethod -Uri $uri -Method Get

# Filtrar por precio
$uri = "http://localhost:5000/api/properties?minPrice=200000000&maxPrice=1000000000"
Invoke-RestMethod -Uri $uri -Method Get
```
```

### Usando Postman

1. Importa la colección desde Swagger:
   - Ve a `http://localhost:5000/swagger/v1/swagger.json`
   - Copia el JSON y úsalo en Postman

2. O crea requests manualmente:
   - `GET http://localhost:5000/api/properties`
   - `GET http://localhost:5000/api/properties?name=Madrid`
   - `GET http://localhost:5000/api/properties/{id}`

---

## ✅ Verificación de Funcionamiento

### Test 1: Verificar que la API está corriendo

```bash
curl http://localhost:5000/api/properties
```

**Resultado esperado:** Lista de propiedades (puede estar vacía si no has ejecutado el seed)

### Test 2: Verificar Swagger UI

Abre en el navegador: `http://localhost:5000`

**Resultado esperado:** Interfaz de Swagger con todos los endpoints

### Test 3: Probar filtros

```bash
# Filtrar por nombre (sin acentos)
curl "http://localhost:5000/api/properties?name=medellin"

# Filtrar por dirección (sin acentos)
curl "http://localhost:5000/api/properties?address=bogota"

# Filtrar por precio
curl "http://localhost:5000/api/properties?minPrice=200000000&maxPrice=1000000000"
```

**Resultado esperado:** Propiedades filtradas según los criterios. Las búsquedas ignoran acentos, por lo que "medellin" encontrará propiedades con "Medellín".

---

## 🔍 Ejecutar Tests Unitarios

Para ejecutar los tests unitarios:

```bash
# Desde la raíz del proyecto
cd "C:\Proyecto .net\tests\PropertyAPI.Tests"
dotnet test
```

O desde la raíz:

```bash
dotnet test tests/PropertyAPI.Tests/PropertyAPI.Tests.csproj
```

---

## ⚠️ Solución de Problemas

### Error: "MongoDB connection string is not configured"

**Causa:** La cadena de conexión no está configurada.

**Solución:**
1. Verifica que `appsettings.json` tenga la configuración correcta
2. Asegúrate de que MongoDB esté corriendo

### Error: "Cannot connect to MongoDB"

**Causa:** MongoDB no está corriendo o la cadena de conexión es incorrecta.

**Solución:**
1. Verifica que MongoDB esté corriendo:
   ```bash
   # Windows
   net start MongoDB
   
2. Verifica el puerto (por defecto 27017)
3. Si usas MongoDB Atlas, verifica la cadena de conexión

### Error: "Port already in use"

**Causa:** El puerto 5000 o 5001 ya está en uso (probablemente una instancia anterior de la API).

**Solución:**

**Opción 1: Detener el proceso que usa el puerto (Recomendado)**

1. **Encuentra el proceso que usa el puerto:**
   ```powershell
   # Para puerto 5000
   netstat -ano | findstr :5000
   
   # Para puerto 5001
   netstat -ano | findstr :5001
   ```

2. **Identifica el PID (número en la última columna)**

3. **Verifica qué proceso es:**
   ```powershell
   tasklist /FI "PID eq <PID>"
   ```

4. **Detén el proceso:**
   ```powershell
   taskkill /PID <PID> /F
   ```

**Ejemplo completo:**
```powershell
# 1. Buscar proceso en puerto 5000
netstat -ano | findstr :5000
# Resultado: TCP    127.0.0.1:5000         0.0.0.0:0              LISTENING       6828

# 2. Ver qué proceso es (PID 6828)
tasklist /FI "PID eq 6828"
# Resultado: PropertyAPI.exe               6828

# 3. Detener el proceso
taskkill /PID 6828 /F
# Resultado: Correcto: se terminó el proceso con PID 6828
```

**Opción 2: Cambiar el puerto**

1. Edita `src/PropertyAPI/Properties/launchSettings.json`
2. Cambia los puertos a otros valores (ej: 5002, 5003)

### Error de certificado SSL

**Causa:** Certificado autofirmado en HTTPS.

**Solución:**
- Usa `http://localhost:5000` en lugar de `https://localhost:5001`
- O desactiva la verificación SSL en tu cliente HTTP

### La API no responde

**Causa:** La aplicación no se inició correctamente.

**Solución:**
1. Verifica que no haya errores en la consola
2. Verifica que MongoDB esté corriendo
3. Verifica que el puerto no esté en uso
4. Revisa los logs de la aplicación

### No aparecen datos después del seed

**Causa:** El seed no se ejecutó o ya había datos.

**Solución:**
1. Verifica la respuesta del endpoint `/api/seed`
2. Si ya había datos, el seed no se ejecuta (comportamiento esperado)
3. Para limpiar y volver a poblar, elimina la base de datos en MongoDB:
   ```bash
   mongosh
   use PropertyDB
   db.dropDatabase()
   ```
   Luego ejecuta el seed nuevamente

---

## 📝 Comandos Útiles

### Conectar a MongoDB
```bash
mongosh
# O versión antigua:
mongo
```

### Ver bases de datos en MongoDB
```bash
mongosh
show dbs
use PropertyDB
show collections
# Verás: Properties, Owners, PropertyImages, PropertyTraces

# Ver propiedades
db.Properties.find().pretty()

# Ver propietarios
db.Owners.find().pretty()

# Ver imágenes
db.PropertyImages.find().pretty()

# Ver trazas
db.PropertyTraces.find().pretty()
```

### Limpiar base de datos
```bash
mongosh
use PropertyDB
db.Properties.deleteMany({})
db.Owners.deleteMany({})
db.PropertyImages.deleteMany({})
db.PropertyTraces.deleteMany({})
```

---

## 🎯 Checklist de Ejecución

- [ ] .NET SDK 9.0 instalado
- [ ] MongoDB instalado y corriendo (o MongoDB Atlas configurado)
- [ ] Configuración de `appsettings.json` correcta
- [ ] API compilada sin errores
- [ ] API corriendo en `http://localhost:5000`
- [ ] Swagger UI accesible
- [ ] Base de datos poblada con datos de ejemplo (Properties, Owners, PropertyImages, PropertyTraces)
- [ ] Endpoints respondiendo correctamente
- [ ] Filtros funcionando (incluyendo búsqueda sin acentos)
- [ ] CORS configurado correctamente para frontend

---

## 📚 Recursos Adicionales

- **Documentación de .NET**: https://docs.microsoft.com/dotnet
- **Documentación de MongoDB**: https://docs.mongodb.com
- **Documentación de Swagger**: https://swagger.io/docs
- **Documentación de la API**: [API_DOCUMENTACION.md](./API_DOCUMENTACION.md)
- **Conectar a MongoDB Atlas**: [CONFIGURAR_MONGODB_ATLAS.md](./CONFIGURAR_MONGODB_ATLAS.md)

---

## 💡 Tips

1. **Modo Watch**: Usa `dotnet watch run` para recarga automática al cambiar código
2. **Swagger UI**: Es la forma más fácil de probar los endpoints
3. **Logs**: Revisa los logs en la consola para depurar problemas
4. **Postman**: Crea una colección de Postman para pruebas más avanzadas

---

**¡Listo!** Ahora deberías tener la API corriendo y funcionando correctamente en tu máquina local.

